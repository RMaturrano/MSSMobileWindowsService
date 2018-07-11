using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WServMobile
{
    public partial class Main : ServiceBase
    {
        private const double STARTUP_TIME = 25000.0d;
        private const double CYCLE_INTERVAL = 25000.0d;
        private Timer timer;
        private MainProcess main;

        public Main()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer(STARTUP_TIME);
            timer.Interval = CYCLE_INTERVAL;
            timer.AutoReset = false;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;

            if (main == null)
                main = new MainProcess();

            main.ejecutarProcesos();

            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            timer.Dispose();
            main = null;
        }
    }
}
