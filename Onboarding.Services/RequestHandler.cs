using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Models;
using Onboarding.Utils;
using Onboarding.Utils.ReviewService;

namespace Onboarding.Services
{
    public partial class RequestHandler : ServiceBase
    {
        private static ReviewServiceClient _rClient;
        public RequestHandler()
        {
            InitializeComponent();
            _rClient = new ReviewServiceClient();
        }

        protected override void OnStart(string[] args)
        {
            using (var db = new OnboardingDbContext())
            {
                try
                {
                    foreach (var request in DbHelpers.SelectAllRequest(db))
                    {
                        CodeFlowHelpers.SubmitCodeReviewFromRequest(db, _rClient, request);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        protected override void OnStop()
        {
            _rClient.Close();
        }
    }
}
