using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using log4net.Repository.Hierarchy;
using Timer = System.Timers.Timer;

namespace Onboarding.Services
{
    public partial class RequestHandler : ServiceBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
            Logger.Debug("Timer Start"); 
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _cancelTokenSource.Cancel();
            _timer = null;
            eventLog1.WriteEntry("Timer Stop");
            Logger.Debug("Timer Stop"); 
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Debug("Service worker starts running"); 
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
                Logger.Error("An error occurred: " + ex.Message);
            }
        }
    }
}
