using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using System;
using System.Globalization;

namespace Onboarding.Models
{
    public class OnboardingRequestContextProvider : EFContextProvider<OnboardingDbContext>
    {

        protected override bool BeforeSaveEntity(EntityInfo entityInfo)
        {
            if (entityInfo.EntityState == EntityState.Added &&
                entityInfo.Entity is OnboardingRequest)
            {
                // Get the instance of the OnboardingRequest class.
                var onboardingRequest = (OnboardingRequest)entityInfo.Entity;

                // Set CreatedDate & ModifiedDate to Now.
                SetCreatedTimeOnInitialization(onboardingRequest);

                // Set RequestState to "Created".
                //onboardingRequest.State = RequestState.Created;
                onboardingRequest.State = "Created";

                //// Write string formatted xml in to an file and assign it to Blob field.
                //string filename = SetSptFilename(onboardingRequest);
                //onboardingRequest.Blob = SystemHelpers.SaveStringToXml(onboardingRequest.TempXmlStore, filename);


                //// Add xml to (local) source depot.
                //SystemHelpers.AddFileToDepot(filename);


                //// Create and submit a code review from information provided in a request
                //// with the file attached to be reviewed.
                //SubmitCodeReviewFromRequest(rClient, onboardingRequest, filename);


                //// Revert file (after review completed?)
                //SystemHelpers.RevertFile(filename);


                //// Close clients.
                //rClient.Close();


                return true;
            }
            return false;
        }

        private void SetCreatedTimeOnInitialization(OnboardingRequest onboardingRequest)
        {
            string dateFormat = "M/d/yyyy h:mm tt";
            onboardingRequest.CreatedDate = DateTime.UtcNow;
            onboardingRequest.DisplayCreatedDate = onboardingRequest.CreatedDate.ToLocalTime().ToString(dateFormat,
                  CultureInfo.CreateSpecificCulture("en-US"));
            onboardingRequest.ModifiedDate = onboardingRequest.CreatedDate;
            onboardingRequest.DisplayModifiedDate = onboardingRequest.DisplayCreatedDate;
        }

        private string SetSptFilename(OnboardingRequest onboardingRequest)
        {
            return onboardingRequest.DisplayName + "_" + onboardingRequest.CreatedBy + ".xml";
        }

        //private void SubmitCodeReviewFromRequest(ReviewServiceClient rClient, OnboardingRequest onboardingRequest, string filename)
        //{
        //    // Step 1 - Create a code review
        //    // Assign ReviewId to the corresponding field in OnboardingRequest
        //    onboardingRequest.CodeFlowId = CodeFlowHelpers.CreateReview(rClient, onboardingRequest.CreatedBy, "Chengkan Huang",
        //        GenerateEmailAddress(onboardingRequest), GenerateReivewName(onboardingRequest), CodeFlowHelpers.ProjectShortName);

        //    // Step 2 - Create a code package and add it to the review
        //    CodeFlowHelpers.AddCodePackage(rClient, onboardingRequest.CodeFlowId, CodeFlowHelpers.CreateCodePackage("testing pack", onboardingRequest.CreatedBy, onboardingRequest.CreatedBy, 
        //        CodePackageFormat.SourceDepotPack, new Uri(SystemHelpers.DepotPath + filename + ".dpk")));


        //    // Step 3 - Add reviewers to the review
        //    CodeFlowHelpers.AddReviewers(rClient, onboardingRequest.CodeFlowId, new ReviewService.Reviewer[]
        //            {
        //                CodeFlowHelpers.CreateReviewer(onboardingRequest.CreatedBy, "Chengkan Huang", GenerateEmailAddress(onboardingRequest), true)
        //            });

        //    // Step 4 - Publish the review
        //    CodeFlowHelpers.PublishReview(rClient, onboardingRequest.CodeFlowId, "meesage from author");
        //}

        //private string GenerateReivewName(OnboardingRequest onboardingRequest)
        //{
        //    return onboardingRequest.Type + "-RequestId-" + onboardingRequest.RequestId;
        //}

        //private string GenerateEmailAddress(OnboardingRequest onboardingRequest)
        //{
        //    return onboardingRequest.CreatedBy + CodeFlowHelpers.EmailDomain;
        //}
    }
}