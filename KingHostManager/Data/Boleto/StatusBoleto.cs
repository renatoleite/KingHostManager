using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingHostManager.Data.Boleto
{
    /// <summary>
    /// Status do boleto.
    /// </summary>
    public enum StatusBoleto
    {
        /// <summary>
        /// Boletos em aberto.
        /// </summary>
        Aberto = 0,

        /// <summary>
        /// Boleto pago.
        /// </summary>
        Pago = 1
    }
}
