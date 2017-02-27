using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingHostManager.Data.Boleto
{
    /// <summary>
    /// Informações sobre o boleto.
    /// </summary>
    public class DadosBoleto
    {
        public int Numero { get; set; }
        public string Banco { get; set; }
        public string Valor { get; set; }
        public Nullable<DateTime> Pagamento { get; set; }
        public string Url { get; set; }
    }
}
