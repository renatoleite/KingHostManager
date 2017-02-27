using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingHostManager.Data.Shared
{
    /// <summary>
    /// Classe base de resultados da kinghost.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JsonResultBase<T>
    {
        /// <summary>
        /// Status da requisição
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Retorna se a requisção foi concluída com sucesso.
        /// </summary>
        public bool Success
        {
            get
            {
                if (this.Status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Dicionários contendo os valores retornados pela kinghost.
        /// </summary>
        public T Body { get; set; }
    }
}
