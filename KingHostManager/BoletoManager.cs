using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KingHostManager.Data.Boleto;
using KingHostManager.Data.Cliente;
using KingHostManager.Data.Dominio;

namespace KingHostManager
{
    /// <summary>
    /// Gerencia o boleto.
    /// </summary>
    public class BoletoManager
    {
        public BoletoManager()
        {
            const string connectionString = "mysql connection string";
            this.DbUtils = new DbUtils(connectionString);
        }

        private DbUtils DbUtils { get; set; }

        public void EnviaBoletos()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                while (true)
                {
                    try
                    {
                        // Busca os clientes que devem enviar boletos.
                        var clientesEnvioBoleto = this.DbUtils.GetClientesParaEnvio();

                        foreach (var cliente in clientesEnvioBoleto)
                        {
                            // Se o boleto já foi enviado no dia atual, não deve ser enviado novamente.
                            if (cliente.DtHrUltimoEnvio != null && cliente.DtHrUltimoEnvio.Value.Date.Equals(DateTime.Now.Date))
                                continue;

                            // Verifica se a data de envio já passou e verifica se o boleto não foi enviado no mês atual.
                            if ((cliente.DiaEnvio <= DateTime.Now.Day) && (cliente.DtHrUltimoEnvio == null || cliente.DtHrUltimoEnvio.Value.Month != DateTime.Now.Month))
                            {
                                // Gera o boleto.
                                using (var cmd = new KingHostCommands())
                                {
                                    var dadosCliente = new DadosCliente() { IdCliente = cliente.CodigoCliente };
                                    var dadosDominio = new DadosDominio() { Id = cliente.CodigoDominio };

                                    var resultBoleto = cmd.CreateNewBoleto(
                                        dadosCliente,
                                        dadosDominio,
                                        DateTime.Now.AddDays(7),
                                        cliente.Valor,
                                        "Instrução de teste",
                                        "Descrição de teste");

                                    // Espera o boleto ser gerado.
                                    resultBoleto.Wait();

                                    if (!resultBoleto.Result.Success)
                                        throw new Exception("Algum problema aconteceu ao gerar o boleto.");

                                    // Envia o e-mail.
                                    {
                                        string subject = string.Format(
                                            "Boleto para pagamento da hospedagem do domínio: {0}",
                                            cliente.Dominio);

                                        string body = string.Format(@"
                                            Prezado(a) {0},
                                            <br><br>
                                            Seguem os dados para pagamento de sua fatura. Boleto bancário disponível para impressão em:<br>
                                            <a href='{1}'>{1}</a>
                                            <br><br>
                                            VALOR A PAGAR: R$ {2}<br>
                                            DOMÍNIO: {3}
                                            <br><br>
                                            Atenciosamente,<br>
                                            Financeiro Agência WD7
                                            <br><br>
                                            financeiro@agenciawd7.com.br<br>
                                            www.agenciawd7.com.br", cliente.Nome, resultBoleto.Result.Body.Url, cliente.Valor, cliente.Dominio);

                                        string to = string.Format("{0}; financeiro@agenciawd7.com.br", cliente.Email);

                                        Email.Send(to, subject, body);
                                    }

                                    // Atualiza a data de envio.
                                    this.DbUtils.AtualizaDataEnvio(cliente.IdBoleto);

                                    // Salva no histórico que o boleto foi enviado.
                                    this.DbUtils.InsereHistorico(cliente.IdBoleto, resultBoleto.Result.Body);
                                }
                            }
                        }

                        Thread.Sleep(600000);
                    }
                    catch(Exception ex)
                    {
                        this.SaveError(ex);
                        Thread.Sleep(600000);
                    }
                }
            });
        }

        public void CheckBoletosPagos()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                while (true)
                {
                    try
                    {
                        // Buscar todos os clientes no banco de dados.
                        var boletos = this.DbUtils.GetClientesEmAberto();

                        using (var cmd = new KingHostCommands())
                        {
                            foreach (var boleto in boletos)
                            {
                                // Verifica se o boleto já foi pago, caso sim,
                                // atualiza a flag no banco de dados.
                                var boletoAberto = cmd.CheckBoletoPago(
                                    new DadosCliente() { IdCliente = boleto.CodigoCliente },
                                    new DadosBoleto() { Numero = boleto.CodigoBoleto });

                                boletoAberto.Wait();

                                
                                if (boletoAberto.Result)
                                {
                                    // Atualiza falando que o boleto foi pago.
                                    this.DbUtils.AtualizaBoletoPago(boleto.CodigoHistorico);

                                    // Envia um e-mail para o cliente que pagou.
                                    var subject = string.Format("Pagamento do domínio: {0}", boleto.Dominio);
                                    var body = string.Format("Foi realizado o pagamento do domínio: {0}", boleto.Dominio);
                                    Email.Send("financeiro@agenciawd7.com.br", subject, body);
                                }
                            }
                        }

                        Thread.Sleep(600000);
                    }
                    catch(Exception ex)
                    {
                        this.SaveError(ex);
                        Thread.Sleep(600000);
                    }
                }
            });

        }

        public void SaveError(Exception ex)
        {
            string message = ex.Message;

            if (ex.InnerException != null)
                message += Environment.NewLine + ex.InnerException.Message;

            EventLog.WriteEntry("KingHostManager", message, EventLogEntryType.Error);
        }
    }
}
