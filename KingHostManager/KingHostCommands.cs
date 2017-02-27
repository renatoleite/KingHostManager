using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KingHostManager.Data;
using System.Web.Script.Serialization;
using KingHostManager.Data.Cliente;
using KingHostManager.Data.Boleto;
using KingHostManager.Data.Dominio;

namespace KingHostManager
{
    /// <summary>
    /// Comando da API da KingHost.
    /// </summary>
    public class KingHostCommands : IDisposable
    {
        /// <summary>
        /// Comando da API da KingHost.
        /// </summary>
        public KingHostCommands()
        {
            string username = "main@domain.com.br";
            string password = "yourpassword";

            // Inicializa a conexão com a API.
            this.InitializeConnection(username, password);
        }

        /// <summary>
        /// Objeto de conexão com a API.
        /// </summary>
        private HttpClient HttpClient { get; set; }

        /// <summary>
        /// Handle de conexão com a API.
        /// </summary>
        private HttpClientHandler HttpClientHandler { get; set; }

        /// <summary>
        /// Inicializa a connexão com a API.
        /// A conexão deve ser inicializa no contructor da classe e não
        /// dentro de cada método utilizador, pois os métodos são assíncronos.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void InitializeConnection(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            // Cria o handle de conexão com a API.
            this.HttpClientHandler = new HttpClientHandler();

            // Define a autenticação.
            this.HttpClientHandler.UseDefaultCredentials = false;
            this.HttpClientHandler.Credentials = new NetworkCredential(username, password);
            this.HttpClientHandler.PreAuthenticate = true;
            this.HttpClientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;

            // Cria o objeto de conexão com a API.
            this.HttpClient = new HttpClient(this.HttpClientHandler);

            // Define o caminho base da API.
            this.HttpClient.BaseAddress = new Uri("https://api.kinghost.net");

            // Define o tipo de requisição a ser utilizada.
            this.HttpClient.DefaultRequestHeaders.Accept.Clear();
            this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Desabilita alguma coisa de proxy (necessário para fazer post).
            this.HttpClient.DefaultRequestHeaders.ExpectContinue = false;
        }

        /// <summary>
        /// Retorna todos os clientes.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResultCliente> GetAllClientes()
        {
            HttpResponseMessage response = await this.HttpClient.GetAsync("https://api.kinghost.net/cliente");

            if (response.IsSuccessStatusCode)
            {
                var clientes = await response.Content.ReadAsStringAsync();
                return new JavaScriptSerializer().Deserialize<JsonResultCliente>(clientes.ToString());
            }

            return null;
        }

        /// <summary>
        /// Retorna todos os dominios do cliente informado.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public async Task<JsonResultDominio> GetDominiosByClient(DadosCliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException("cliente");

            string url = string.Format("https://api.kinghost.net/dominio/{0}", cliente.IdCliente);

            HttpResponseMessage response = await this.HttpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var dominios = await response.Content.ReadAsStringAsync();
                return new JavaScriptSerializer().Deserialize<JsonResultDominio>(dominios.ToString());
            }

            return null;
        }

        /// <summary>
        /// Retorna todos os boletos informando o cliente e status.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="dominio"></param>
        /// <param name="statusBoleto"></param>
        /// <returns></returns>
        public async Task<JsonResultDictionaryBoleto> GetBoletosClienteByStatus(
            DadosCliente cliente,
            DadosDominio dominio,
            StatusBoleto statusBoleto)
        {
            if (cliente == null)
                throw new ArgumentNullException("cliente");

            if (dominio == null)
                throw new ArgumentNullException("dominio");

            string url = string.Format("https://api.kinghost.net/boleto/{0}/{1}/{2}",
                cliente.IdCliente,
                dominio.Id,
                statusBoleto.ToString());

            HttpResponseMessage response = await this.HttpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var boletos = await response.Content.ReadAsStringAsync();
                return new JavaScriptSerializer().Deserialize<JsonResultDictionaryBoleto>(boletos.ToString());
            }

            return null;
        }

        /// <summary>
        /// Cria um novo boleto no banco de dados.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="dominio"></param>
        /// <param name="vencimento"></param>
        /// <param name="valor"></param>
        /// <param name="instrucao"></param>
        /// <param name="descricao"></param>
        public async Task<JsonResultBoleto> CreateNewBoleto(
            DadosCliente cliente,
            DadosDominio dominio,
            DateTime vencimento,
            Double valor,
            string instrucao,
            string descricao)
        {
            if (cliente == null)
                throw new ArgumentNullException("cliente");

            if (dominio == null)
                throw new ArgumentNullException("dominio");

            // Prepara os valores que serão postados.
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("idCliente", Convert.ToString(cliente.IdCliente)));
            postData.Add(new KeyValuePair<string, string>("idDominio", Convert.ToString(dominio.Id)));
            postData.Add(new KeyValuePair<string, string>("idBanco", "3"));
            postData.Add(new KeyValuePair<string, string>("vencimento", vencimento.ToString("yyyy-MM-dd")));
            postData.Add(new KeyValuePair<string, string>("valor", Convert.ToString(valor)));
            postData.Add(new KeyValuePair<string, string>("instrucao", instrucao));
            postData.Add(new KeyValuePair<string, string>("desc", descricao));

            HttpContent content = new FormUrlEncodedContent(postData);

            HttpResponseMessage response = await this.HttpClient.PostAsync("https://api.kinghost.net/boleto/", content);

            if (response.IsSuccessStatusCode)
            {
                var boletos = await response.Content.ReadAsStringAsync();
                return new JavaScriptSerializer().Deserialize<JsonResultBoleto>(boletos.ToString());
            }

            return null;
        }

        /// <summary>
        /// Retorna o status do boleto, se já foi pago ou se ainda está em aberto.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="boleto"></param>
        /// <returns></returns>
        public async Task<bool> CheckBoletoPago(DadosCliente cliente, DadosBoleto boleto)
        {
            if (cliente == null)
                throw new ArgumentNullException("cliente");

            if (boleto == null)
                throw new ArgumentNullException("boleto");

            string url = string.Format("https://api.kinghost.net/boleto/{0}/aberto", cliente.IdCliente);

            HttpResponseMessage response = await this.HttpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var boletos = new JavaScriptSerializer().Deserialize<JsonResultArrayBoleto>(content.ToString());

                // Verifica se dentre os boletos retornados, existe o boleto
                // informado. Caso exista, é porque o boleto ainda não pago.
                return !boletos.Body.Any(x => x.Numero.Equals(boleto.Numero));
            }

            return false;
        }

        /// <summary>
        /// Libera recursos da classe.
        /// </summary>
        public void Dispose()
        {
            this.HttpClient.Dispose();
            this.HttpClientHandler.Dispose();
        }
    }
}
