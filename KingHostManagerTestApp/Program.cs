using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingHostManager;

namespace KingHostManagerTestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var boletoManager = new BoletoManager();
            boletoManager.CheckBoletosPagos();
            Console.ReadLine();
        }
    }
}
