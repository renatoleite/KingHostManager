using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingHostManager.Data.Cliente
{
    public class DadosCliente
    {
        public int IdCliente { get; set; }
        public string ClienteTipo { get; set; }
        public string ClienteEmpresa { get; set; }
        public string ClienteNome { get; set; }
        public string ClienteCPFCNPJ { get; set; }
        public string ClienteEmail { get; set; }
        public string ClienteEmailCobranca { get; set; }
        public string ClienteFone { get; set; }
        public string ClienteFax { get; set; }
        public string ClienteEndereco { get; set; }
        public string ClienteBairro { get; set; }
        public string ClienteCidade { get; set; }
        public string ClienteEstado { get; set; }
        public string ClienteCEP { get; set; }
        public int ClienteLimiteMapeamento { get; set; }
        public int ClienteLimiteSubdominio { get; set; }
        public int ClienteLimiteMysql { get; set; }
        public int ClienteLimiteMssql { get; set; }
        public int ClienteLimitePgsql { get; set; }
        public int ClienteLimiteFirebird { get; set; }
        public int ClienteLimiteFTPADD { get; set; }
        public string ClienteUniBox { get; set; }
        public string ClienteAcessoFTP { get; set; }
        public string ClienteAcessoDownloadBackup { get; set; }
        public Nullable<DateTime> ClienteDtVerificacaoCpfCnpj { get; set; }
    }
}
