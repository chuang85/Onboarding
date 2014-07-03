using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Onboarding.DashboardService;
using Onboarding.ReviewService;
using Onboarding.Models;

namespace Onboarding.Utils
{
    public class OnboardingRequestContextProvider : EFContextProvider<OnboardingDbContext>
    {
        public ReviewDashboardServiceClient qClient = new ReviewDashboardServiceClient();
        public ReviewServiceClient rClient = new ReviewServiceClient();

        protected override bool BeforeSaveEntity(EntityInfo entityInfo)
        {
            OnboardingRequest onboardingRequest = (OnboardingRequest)entityInfo.Entity;
            onboardingRequest.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
            //DateTime timeUtc = DateTime.UtcNow;
            //TimeZoneInfo ptZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            //onboardingRequest.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, ptZone);

            //onboardingRequest.Blob = GetBytes(onboardingRequest.TempXmlStore);

            // Step 1 - Create a code review
            // Assign ReviewId to the corresponding field in OnboardingRequest
            onboardingRequest.CodeFlowId = CodeFlowHelpers.CreateReview(rClient, onboardingRequest.CreatedBy, "Chengkan Huang",
                onboardingRequest.CreatedBy + "@microsoft.com", "Testing integration", "MSODS"); ;

            // Step 2 - Create a code package and add it to the review
            CodeFlowHelpers.AddCodePackage(rClient, onboardingRequest.CodeFlowId, CodeFlowHelpers.CreateCodePackage("testing pack", onboardingRequest.CreatedBy, onboardingRequest.CreatedBy,
                CodePackageFormat.SourceDepotPack, new Uri(@"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\2.dpk")));

            // Step 3 - Add reviewers to the review
            CodeFlowHelpers.AddReviewers(rClient, onboardingRequest.CodeFlowId, new ReviewService.Reviewer[]
                {
                    CodeFlowHelpers.CreateReviewer(onboardingRequest.CreatedBy, "Chengkan Huang", onboardingRequest.CreatedBy + "@microsoft.com", true)
                });

            // Step 4 - Publish the review
            CodeFlowHelpers.PublishReview(rClient, onboardingRequest.CodeFlowId, "meesage from author");
            return true;
        }

        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}