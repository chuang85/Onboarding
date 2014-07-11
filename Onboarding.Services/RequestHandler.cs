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
        }

        protected override void OnStart(string[] args)
        {
            
        }

        protected override void OnStop()
        {
        }
    }
}
