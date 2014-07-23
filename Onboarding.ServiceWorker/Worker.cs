using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using System.Timers;
using Onboarding.Models;
using Onboarding.Utils;
using Onboarding.Utils.ReviewService;
using Onboarding.Utils.DashboardService;
using Reviewer = Onboarding.Utils.ReviewService.Reviewer;

namespace Onboarding.ServiceWorker
{
    public class Worker
    {
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
            Console.WriteLine("done");
            Console.WriteLine("Hit enter...");
            Console.Read();
        }

        private static void InitializeClients()
        {
            _rClient = new ReviewServiceClient();
            _qClient = new ReviewDashboardServiceClient();
        }

        private static void CloseClients()
        {
            _rClient.Close();
            _qClient.Close();
        }

        private static void HandleRequests(OnboardingDbContext db)
        {
            foreach (var request in DbHelpers.UncompletedRequests(db))
            {
                //ct.ThrowIfCancellationRequested();
                switch (request.State)
                {
                    case "Created":
                        HandleCreated(request);
                        break;
                    case "PendingReview":
                        HandlePendingReview(request);
                        break;
                }
            }
        }

        private static void UpdateDbInfo(OnboardingDbContext db)
        {
            DbHelpers.AddOrUpdateServiceTypes(db);
            DbHelpers.AddOrUpdateTaskSets(db);
            DbHelpers.AddOrUpdateScopes(db);
        }

        private static void HandleCreated(OnboardingRequest request)
        {
            SystemHelpers.SaveXmlToDisk(request);
            SystemHelpers.AddFileToDepotAndPack(SystemHelpers.GenerateFilename(request));

            // Create a code review.
            var codeFlowId = CodeFlowHelpers.CreateReview(_rClient, request.CreatedBy, MembershipCheckHelper.GetName(request.CreatedBy),
                MembershipCheckHelper.GetEmailAddress(request.CreatedBy), SystemHelpers.GenerateReivewName(request), SystemHelpers.ProjectShortName);
            // Assign ReviewId to the corresponding field in OnboardingRequest
            request.CodeFlowId = codeFlowId;
            // Create a code package and add it to the review
            CodeFlowHelpers.AddCodePackage(_rClient, request.CodeFlowId, CodeFlowHelpers.CreateCodePackage("testing pack", request.CreatedBy, request.CreatedBy,
                CodePackageFormat.SourceDepotPack, new Uri(SystemHelpers.DepotPath + SystemHelpers.GenerateFilename(request) + ".dpk")));
            // Add reviewers to the review
            CodeFlowHelpers.AddReviewers(_rClient, request.CodeFlowId, new Reviewer[]
                        {
                            CodeFlowHelpers.CreateReviewer(request.CreatedBy, MembershipCheckHelper.GetName(request.CreatedBy), MembershipCheckHelper.GetEmailAddress(request.CreatedBy), true)
                        });
            // Publish the review
            CodeFlowHelpers.PublishReview(_rClient, request.CodeFlowId, "meesage from author");
            // Change State from "Created" to "PendingReview"
            request.State = "PendingReview";
            // Revert file to clean the changelist
            SystemHelpers.RevertFile(SystemHelpers.GenerateFilename(request));
        }

        private static void HandlePendingReview(OnboardingRequest request)
        {
            if (CodeFlowHelpers.ReviewCompleted(_qClient, request.CodeFlowId, request.CreatedBy))
            {
                request.State = "ReviewApproved";
            }
        }
    }
}
