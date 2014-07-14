using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Onboarding.Models;
using Onboarding.Utils;
using Onboarding.Utils.ReviewService;
using Onboarding.Utils.DashboardService;
using Reviewer = Onboarding.Utils.ReviewService.Reviewer;

namespace Onboarding.Services
{
    public partial class RequestHandler : ServiceBase
    {
        private ReviewServiceClient _rClient;
        private ReviewDashboardServiceClient _qClient;
        private Timer timer;
        public RequestHandler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _rClient = new ReviewServiceClient();
            _qClient = new ReviewDashboardServiceClient();
            timer = new Timer(5000D);
            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            //using (var db = new OnboardingDbContext())
            //{
            //    try
            //    {
            //        // Handling request in state "Created"
            //        foreach (var request in DbHelpers.RequestsCreated(db))
            //        {
            //            // Create a code review.
            //            var codeFlowId = CodeFlowHelpers.CreateReview(_rClient, request.CreatedBy, "Chengkan Huang",
            //                CodeFlowHelpers.GenerateEmailAddress(request), CodeFlowHelpers.GenerateReivewName(request),
            //                CodeFlowHelpers.ProjectShortName);
            //            // Assign ReviewId to the corresponding field in OnboardingRequest
            //            request.CodeFlowId = codeFlowId;
            //            // Create a code package and add it to the review
            //            CodeFlowHelpers.AddCodePackage(_rClient, request.CodeFlowId,
            //                CodeFlowHelpers.CreateCodePackage("testing pack", request.CreatedBy, request.CreatedBy,
            //                    CodePackageFormat.SourceDepotPack,
            //                    new Uri(SystemHelpers.DepotPath + CodeFlowHelpers.GenerateFilename(request) + ".dpk")));
            //            // Add reviewers to the review
            //            CodeFlowHelpers.AddReviewers(_rClient, request.CodeFlowId, new Reviewer[]
            //            {
            //                CodeFlowHelpers.CreateReviewer(request.CreatedBy, "Chengkan Huang",
            //                    CodeFlowHelpers.GenerateEmailAddress(request), true)
            //            });
            //            // Publish the review
            //            CodeFlowHelpers.PublishReview(_rClient, request.CodeFlowId, "meesage from author");
            //            // Change State from "Created" to "PendingReview"
            //            request.State = "PendingReview";
            //        }
            //        // Handling request in state "PendingReview"
            //        foreach (var request in DbHelpers.RequestsPendingReview(db))
            //        {
            //            if (CodeFlowHelpers.ReviewCompleted(_qClient, request.CodeFlowId, request.CreatedBy))
            //            {
            //                request.State = "ReviewApproved";
            //            }
            //        }
            //        //DoWork(db);
            //        db.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("An error occurred: " + ex.Message);
            //    }
            //}
        }

        protected override void OnStop()
        {
            _rClient.Close();
            _qClient.Close();
            timer.Stop();
            timer = null;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ConsoleTest.Program.Main();
        }

        private void DoWork(OnboardingDbContext db)
        {
            HandleCreated(db);
            HandlePendingReview(db);
        }

        private void HandleCreated(OnboardingDbContext db)
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

        private void HandlePendingReview(OnboardingDbContext db)
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
