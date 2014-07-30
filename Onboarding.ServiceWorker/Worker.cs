using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using log4net;
using Onboarding.Models;
using Onboarding.Utils;
using Onboarding.Config;
using Onboarding.Utils.ReviewService;
using Onboarding.Utils.DashboardService;
using Reviewer = Onboarding.Utils.ReviewService.Reviewer;

namespace Onboarding.ServiceWorker
{
    public class Worker
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReviewServiceClient _rClient;
        private static ReviewDashboardServiceClient _qClient;
        public static void Main()
        {
            InitializeClients();
            using (var db = new OnboardingDbContext())
            {
                UpdateDbInfo(db);
                HandleRequests(db);
                db.SaveChanges();
            }
            CloseClients();
            //Console.WriteLine("done");
            //Console.WriteLine("Hit enter...");
            //Console.Read();
        }

        private static void InitializeClients()
        {
            Logger.Debug("Initializing CodeFlow Review Clients...");
            _rClient = new ReviewServiceClient();
            _qClient = new ReviewDashboardServiceClient();
        }

        private static void CloseClients()
        {
            Logger.Debug("Closing CodeFlow Review Clients...");
            _rClient.Close();
            _qClient.Close();
        }

        private static void HandleRequests(OnboardingDbContext db)
        {
            foreach (var request in DbHelpers.UncompletedRequests(db))
            {
                //ct.ThrowIfCancellationRequested();
                Logger.Info("Handling request with id [" + request.RequestId + "], type [" + request.Type + "] state [" + request.State + "]");
                switch (request.State)
                {
                    case RequestState.Created:
                        HandleCreated(request);
                        break;
                    case RequestState.PendingReview:
                        HandlePendingReview(request);
                        break;
                    case RequestState.ReviewCompleted:
                        HandleReviewCompleted(request);
                        break;
                    case RequestState.RTDQueued:
                        HandleRtdQueued(request);
                        break;
                    case RequestState.RTDApproved:
                        HandleRtdApproved(request);
                        break;
                }
            }
        }

        private static void UpdateDbInfo(OnboardingDbContext db)
        {
            //DbHelpers.AddOrUpdateTaskSets(db);
            //DbHelpers.AddOrUpdateScopes(db);
            DbHelpers.AddOrUpdateDescriptions(db);
            DbHelpers.AddOrUpdateExistingSpts(db);
        }

        private static void HandleCreated(OnboardingRequest request)
        {
            if (request.Type == RequestType.CreateSPT || request.Type == RequestType.UpdateSPT)
            {
                SystemHelpers.SaveXmlToDisk(request);
                SystemHelpers.AddFileToDepotAndPack(SystemHelpers.GenerateFilename(request));

                // Create a code review.
                var codeFlowId = CodeFlowHelpers.CreateReview(_rClient, request.CreatedBy,
                    MembershipCheckHelpers.GetName(request.CreatedBy),
                    MembershipCheckHelpers.GetEmailAddress(request.CreatedBy), SystemHelpers.GenerateReivewName(request),
                    Constants.ProjectShortName);
                // Assign ReviewId to the corresponding field in OnboardingRequest
                request.CodeFlowId = codeFlowId;
                // Create a code package and add it to the review
                CodeFlowHelpers.AddCodePackage(_rClient, request.CodeFlowId,
                    CodeFlowHelpers.CreateCodePackage("testing pack", request.CreatedBy, request.CreatedBy,
                        CodePackageFormat.SourceDepotPack,
                        new Uri(Constants.DepotPath + SystemHelpers.GenerateFilename(request) + ".dpk")));
                // Add reviewers to the review
                CodeFlowHelpers.AddReviewers(_rClient, request.CodeFlowId, new Reviewer[]
                {
                    CodeFlowHelpers.CreateReviewer(request.CreatedBy, MembershipCheckHelpers.GetName(request.CreatedBy),
                        MembershipCheckHelpers.GetEmailAddress(request.CreatedBy), true)
                });
                // Publish the review
                CodeFlowHelpers.PublishReview(_rClient, request.CodeFlowId, "meesage from author");
                Logger.Info("Code review with id [" + request.CodeFlowId + "] has been published");
                // Change State from "Created" to "PendingReview"
                request.State = RequestState.PendingReview;
                Logger.Info("Change the state of request with id [" + request.RequestId + "] to [PendingReview]");
                // Revert file to clean the changelist
                SystemHelpers.RevertFile(SystemHelpers.GenerateFilename(request));
            }
        }

        private static void HandlePendingReview(OnboardingRequest request)
        {
            if (CodeFlowHelpers.ReviewCompleted(_qClient, request.CodeFlowId, request.CreatedBy))
            {
                request.State = RequestState.ReviewCompleted;
                Logger.Info("Change the state of request with id [" + request.RequestId + "] to [ReviewCompleted]");
            }
        }

        private static void HandleReviewCompleted(OnboardingRequest request)
        {
            //TODO: fire off RTD
        }

        private static void HandleRtdQueued(OnboardingRequest request)
        {
            //TODO: run RTD
        }

        private static void HandleRtdApproved(OnboardingRequest request)
        {
            //TODO: mark completed
        }
    }
}
