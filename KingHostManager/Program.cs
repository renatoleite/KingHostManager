using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KingHostManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            // Verifica se o usuário passou parametros.
            if (Environment.UserInteractive)
            {
                string parametro = string.Concat(args);
                switch (parametro)
                {
                    // Caso o parametro seja "--install", instala o serviço.
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;

                    // Caso o parametro seja "--uninstall", desinstala o serviço.
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
            {
                // Se nenhum parametro for passado, executa a aplicação normalmente.
                ServiceBase[] servicesToRun;
                servicesToRun = new ServiceBase[] 
                { 
                new KingHostManagerService() 
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
