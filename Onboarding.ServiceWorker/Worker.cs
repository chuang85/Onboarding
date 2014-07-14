using System;
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
                DoWork(db);
                db.SaveChanges();
            }
            CloseClients();
            Console.WriteLine("done");
            Console.WriteLine("Hit enter...");
            Console.Read();

            //using (var db = new OnboardingDbContext())
            //{
            //    foreach (var r in (from d in db.OnboardingRequests select d))
            //    {
            //        r.DisplayName = "shitty";
            //    }
            //    db.SaveChanges();
            //}
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

        private static void DoWork(OnboardingDbContext db)
        {
            HandleCreated(db);
            HandlePendingReview(db);
        }

        private static void HandleCreated(OnboardingDbContext db)
        {
            foreach (var request in DbHelpers.RequestsCreated(db))
            {
                // Create a code review.
                var codeFlowId = CodeFlowHelpers.CreateReview(_rClient, request.CreatedBy, "Chengkan Huang",
                    CodeFlowHelpers.GenerateEmailAddress(request), CodeFlowHelpers.GenerateReivewName(request), CodeFlowHelpers.ProjectShortName);
                // Assign ReviewId to the corresponding field in OnboardingRequest
                request.CodeFlowId = codeFlowId;
                // Create a code package and add it to the review
                CodeFlowHelpers.AddCodePackage(_rClient, request.CodeFlowId, CodeFlowHelpers.CreateCodePackage("testing pack", request.CreatedBy, request.CreatedBy,
                    CodePackageFormat.SourceDepotPack, new Uri(SystemHelpers.DepotPath + CodeFlowHelpers.GenerateFilename(request) + ".dpk")));
                // Add reviewers to the review
                CodeFlowHelpers.AddReviewers(_rClient, request.CodeFlowId, new Reviewer[]
                        {
                            CodeFlowHelpers.CreateReviewer(request.CreatedBy, "Chengkan Huang", CodeFlowHelpers.GenerateEmailAddress(request), true)
                        });
                // Publish the review
                CodeFlowHelpers.PublishReview(_rClient, request.CodeFlowId, "meesage from author");
                // Change State from "Created" to "PendingReview"
                request.State = "PendingReview";
            }
        }

        private static void HandlePendingReview(OnboardingDbContext db)
        {
            foreach (var request in DbHelpers.RequestsPendingReview(db))
            {
                if (CodeFlowHelpers.ReviewCompleted(_qClient, request.CodeFlowId, request.CreatedBy))
                {
                    request.State = "ReviewApproved";
                }
            }
        }
    }
}
