using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Onboarding.DashboardService;
using Onboarding.ReviewService;
using Onboarding.Utils;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Onboarding.Models
{
    public class OnboardingRequestContextProvider : EFContextProvider<OnboardingDbContext>
    {
        public ReviewDashboardServiceClient qClient = new ReviewDashboardServiceClient();
        public ReviewServiceClient rClient = new ReviewServiceClient();

        protected override bool BeforeSaveEntity(EntityInfo entityInfo)
        {
            if (entityInfo.EntityState == EntityState.Added &&
                entityInfo.Entity is OnboardingRequest)
            {
                // Get the instance of the OnboardingRequest class.
                var onboardingRequest = (OnboardingRequest)entityInfo.Entity;

                // Set CreatedDate & ModifiedDate to Now.
                SetCreatedTimeOnInitialization(onboardingRequest);

                // Write string formatted xml in to an file and assign it to Blob field.
                string filename = SetSptFilename(onboardingRequest);
                onboardingRequest.Blob = SystemHelpers.SavesStringToXml(onboardingRequest.TempXmlStore, filename);

                // Add xml to (local) source depot.
                SystemHelpers.AddFileToDepot(filename);

                // Create and submit a code review from information provided in a request
                // Filename - the file attached to be reviewed.
                SubmitCodeReviewFromRequest(onboardingRequest, filename);

                // Close clients
                rClient.Close();

                return true;
            }
            return false;
        }

        private void SetCreatedTimeOnInitialization(OnboardingRequest onboardingRequest)
        {
            onboardingRequest.CreatedDate = DateTime.UtcNow;
            onboardingRequest.DisplayCreatedDate = onboardingRequest.CreatedDate.ToLocalTime().ToString("M/d/yyyy h:mm tt",
                  CultureInfo.CreateSpecificCulture("en-US"));
            onboardingRequest.ModifiedDate = onboardingRequest.CreatedDate;
            onboardingRequest.DisplayModifiedDate = onboardingRequest.DisplayCreatedDate;
        }

        private string SetSptFilename(OnboardingRequest onboardingRequest)
        {
            return onboardingRequest.DisplayName + "_" + onboardingRequest.CreatedBy + ".xml";
        }

        private void SubmitCodeReviewFromRequest(OnboardingRequest onboardingRequest, string filename)
        {
            // Step 1 - Create a code review
            // Assign ReviewId to the corresponding field in OnboardingRequest
            onboardingRequest.CodeFlowId = CodeFlowHelpers.CreateReview(rClient, onboardingRequest.CreatedBy, "Chengkan Huang",
                onboardingRequest.CreatedBy + "@microsoft.com", "Testing integration", "MSODS");

            // Step 2 - Create a code package and add it to the review
            CodeFlowHelpers.AddCodePackage(rClient, onboardingRequest.CodeFlowId, CodeFlowHelpers.CreateCodePackage("testing pack", onboardingRequest.CreatedBy, onboardingRequest.CreatedBy, CodePackageFormat.SourceDepotPack, new Uri(SystemHelpers.DepotPath + filename)));
            //CodeFlowHelpers.AddCodePackage(rClient, onboardingRequest.CodeFlowId, CodeFlowHelpers.CreateCodePackage("testing pack", onboardingRequest.CreatedBy, onboardingRequest.CreatedBy, new Uri(SystemHelpers.DestPath + filename)));


            // Step 3 - Add reviewers to the review
            CodeFlowHelpers.AddReviewers(rClient, onboardingRequest.CodeFlowId, new ReviewService.Reviewer[]
                    {
                        CodeFlowHelpers.CreateReviewer(onboardingRequest.CreatedBy, "Chengkan Huang", onboardingRequest.CreatedBy + "@microsoft.com", true)
                    });

            // Step 4 - Publish the review
            CodeFlowHelpers.PublishReview(rClient, onboardingRequest.CodeFlowId, "meesage from author");
        }
    }
}