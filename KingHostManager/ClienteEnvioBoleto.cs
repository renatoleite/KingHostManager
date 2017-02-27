using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingHostManager
{
    public class ClienteEnvioBoleto
    {
        public int IdBoleto { get; set; }
        public float Valor { get; set; }
        public int DiaEnvio { get; set; }
        public Nullable<DateTime> DtHrUltimoEnvio { get; set; }
        public int CodigoDominio { get; set; }
        public string Dominio { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public int CodigoCliente { get; set; }
        public int CodigoHistorico { get; set; }
        public int CodigoBoleto { get; set; }
    }
}
