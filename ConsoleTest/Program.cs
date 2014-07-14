using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Models;
using Onboarding.Utils;
using Onboarding.Utils.ReviewService;
using Onboarding.Utils.DashboardService;
using Reviewer = Onboarding.Utils.ReviewService.Reviewer;

namespace ConsoleTest
{
    public class Program
    {
        public static void Main()
        {
            using (var db = new OnboardingDbContext())
            {
                foreach (var r in (from d in db.OnboardingRequests select d))
                {
                    r.DisplayName = "shitty";
                }
                db.SaveChanges();
            }
            //var rClient = new ReviewServiceClient();
            //var qClient = new ReviewDashboardServiceClient();
            //using (var db = new OnboardingDbContext())
            //{
            //    try
            //    {
            //        // Handling request in state "Created"
            //        foreach (var request in DbHelpers.RequestsCreated(db))
            //        {
            //            // Create a code review.
            //            var codeFlowId = CodeFlowHelpers.CreateReview(rClient, request.CreatedBy, "Chengkan Huang",
            //                CodeFlowHelpers.GenerateEmailAddress(request), CodeFlowHelpers.GenerateReivewName(request), CodeFlowHelpers.ProjectShortName);
            //            // Assign ReviewId to the corresponding field in OnboardingRequest
            //            request.CodeFlowId = codeFlowId;
            //            // Create a code package and add it to the review
            //            CodeFlowHelpers.AddCodePackage(rClient, request.CodeFlowId, CodeFlowHelpers.CreateCodePackage("testing pack", request.CreatedBy, request.CreatedBy,
            //                CodePackageFormat.SourceDepotPack, new Uri(SystemHelpers.DepotPath + CodeFlowHelpers.GenerateFilename(request) + ".dpk")));
            //            // Add reviewers to the review
            //            CodeFlowHelpers.AddReviewers(rClient, request.CodeFlowId, new Reviewer[]
            //            {
            //                CodeFlowHelpers.CreateReviewer(request.CreatedBy, "Chengkan Huang", CodeFlowHelpers.GenerateEmailAddress(request), true)
            //            });
            //            // Publish the review
            //            CodeFlowHelpers.PublishReview(rClient, request.CodeFlowId, "meesage from author");
            //            // Change State from "Created" to "PendingReview"
            //            request.State = "PendingReview";
            //        }
            //        // Handling request in state "PendingReview"
            //        foreach (var request in DbHelpers.RequestsPendingReview(db))
            //        {
            //            if (CodeFlowHelpers.ReviewCompleted(qClient, request.CodeFlowId, request.CreatedBy))
            //            {
            //                request.State = "ReviewApproved";
            //            }
            //        }
            //        db.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("An error occurred: " + ex.Message);
            //    }
            //}
            //rClient.Close();
            //qClient.Close();
            //Console.WriteLine("done");

            //Console.WriteLine("Hit enter...");
            //Console.Read();
        }
    }
}
