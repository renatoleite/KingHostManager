using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KingHostManager
{
    public partial class KingHostManagerService : ServiceBase
    {
        public KingHostManagerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var boletoManager = new BoletoManager();
            boletoManager.EnviaBoletos();
            boletoManager.CheckBoletosPagos();
        }

        protected override void OnStop()
        {
        }
    }
}
