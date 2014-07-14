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
        }

        protected override void OnStart(string[] args)
        {
            _timer = new Timer(5000D);
            _timer.AutoReset = true;
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer = null;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ServiceWorker.Worker.Main();
        }
    }
}
