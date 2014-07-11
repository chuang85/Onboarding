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
        private static OnboardingDbContext _obContext;
        public RequestHandler()
        {
            InitializeComponent();
            _rClient = new ReviewServiceClient();
            _obContext = new OnboardingDbContext();
        }

        protected override void OnStart(string[] args)
        {
            foreach (OnboardingRequest request in DbHelpers.SelectAllRequest(_obContext))
            {
                //CodeFlowHelpers.SubmitCodeReviewFromRequest(_rClient, request);
            }
        }

        protected override void OnStop()
        {
            _rClient.Close();
        }
    }
}
