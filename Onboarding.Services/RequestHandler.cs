using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Onboarding.Services
{
    public partial class RequestHandler : ServiceBase
    {
        private Timer _timer;
        CancellationTokenSource _cancelTokenSource = new CancellationTokenSource(); 
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
            _cancelTokenSource.Cancel();
            _timer = null;
            eventLog1.WriteEntry("Timer Stop");
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Task.Run(
                //    () => ServiceWorker.Worker.Main(),
                //    _cancelTokenSource.Token);
                CancellationToken ct = _cancelTokenSource.Token;
                Task.Run(
                    () => ServiceWorker.Worker.Main(), ct);
            }
            catch (Exception ex)
            {
                eventLog1.Log = "An error occurred: " + ex.Message;
            }
        }
    }
}
