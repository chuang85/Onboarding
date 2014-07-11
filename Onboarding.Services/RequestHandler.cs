using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Utils;
using Onboarding.Utils.ReviewService;

namespace Onboarding.Services
{
    public partial class RequestHandler : ServiceBase
    {
        public static ReviewServiceClient rClient = new ReviewServiceClient();
        public RequestHandler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Step 1 - Create a code review
            string reviewId = CodeFlowHelpers.CreateReview(rClient, "t-chehu", "Chengkan Huang", "t-chehu@microsoft.com", "Testing service", "MSODS");


            // Step 2 - Create a code package and add it to the review
            CodeFlowHelpers.AddCodePackage(rClient, reviewId, CodeFlowHelpers.CreateCodePackage("testing pack", "t-chehu", "t-chehu",
                CodePackageFormat.SourceDepotPack, new Uri(@"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\qwefd_t-chehu.xml.dpk")));


            // Step 3 - Add reviewers to the review
            CodeFlowHelpers.AddReviewers(rClient, reviewId, new Reviewer[]
            {
                CodeFlowHelpers.CreateReviewer("t-chehu", "Chengkan Huang", "t-chehu@microsoft.com", true)
            });


            // Step 4 - Publish the review
            CodeFlowHelpers.PublishReview(rClient, reviewId, "meesage from author");

        }

        protected override void OnStop()
        {
        }
    }
}
