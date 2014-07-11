using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Models;
using Onboarding.Utils;
using Onboarding.Utils.ReviewService;

namespace ConsoleTest
{
    class Program
    {
        private static OnboardingDbContext _obContext;
        static void Main(string[] args)
        {
            var rClient = new ReviewServiceClient();
            using (_obContext = new OnboardingDbContext())
            {
                Console.WriteLine("start");
                try
                {
                    foreach (var request in DbHelpers.SelectAllRequest(_obContext))
                    {
                        ////CodeFlowHelpers.SubmitCodeReviewFromRequest(rClient, request);
                        //string reviewId = CodeFlowHelpers.CreateReview(rClient, "t-chehu", "Chengkan Huang", "t-chehu@microsoft.com", "nice", "MSODS");

                        //// Step 2 - Create a code package and add it to the review
                        //CodeFlowHelpers.AddCodePackage(rClient, reviewId, CodeFlowHelpers.CreateCodePackage("testing pack", "t-chehu", "t-chehu",
                        //    CodePackageFormat.SourceDepotPack, new Uri(@"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\ert_qwe.xml.dpk")));

                        //// Step 3 - Add reviewers to the review
                        //CodeFlowHelpers.AddReviewers(rClient, reviewId, new Reviewer[]
                        //{
                        //    CodeFlowHelpers.CreateReviewer("t-chehu", "Chengkan Huang", "t-chehu@microsoft.com", true)
                        //});

                        //// Step 4 - Publish the review
                        //CodeFlowHelpers.PublishReview(rClient, reviewId, "meesage from author");

                        //Console.WriteLine(reviewId);
                        request.DisplayName = "somename";
                        _obContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            Console.WriteLine("done");
            rClient.Close();

            Console.WriteLine("Hit enter...");
            Console.Read();
        }
    }
}
