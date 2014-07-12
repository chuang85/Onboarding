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

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var rClient = new ReviewServiceClient();
            using (var db = new OnboardingDbContext())
            {
                try
                {
                    foreach (var request in DbHelpers.SelectAllRequest(db))
                    {
                        CodeFlowHelpers.SubmitCodeReviewFromRequest(db, rClient, request);
                    }
                    db.SaveChanges();
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
