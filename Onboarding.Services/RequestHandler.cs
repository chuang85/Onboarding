using System;
using System.ServiceProcess;
using System.Timers;

namespace Onboarding.Services
{
    public partial class RequestHandler : ServiceBase
    {
        private Timer _timer;
        public RequestHandler()
        {
            InitializeComponent();
            AutoLog = false;
            if (!System.Diagnostics.EventLog.SourceExists("OnboardingSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "OnboardingSource", "OnboardingLog");
            }
            eventLog1.Source = "OnboardingSource";
            eventLog1.Log = "OnboardingLog";

        }

        protected override void OnStart(string[] args)
        {
            _timer = new Timer(5000D);
            _timer.AutoReset = true;
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Start();
            eventLog1.WriteEntry("Timer Start");
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer = null;
            eventLog1.WriteEntry("Timer Stop");
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ServiceWorker.Worker.Main();
            }
            catch (Exception ex)
            {
                eventLog1.Log = "An error occurred: " + ex.Message;
            }
            
        }
    }
}
