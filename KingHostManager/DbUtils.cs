using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingHostManager.Data.Boleto;
using MySql.Data.MySqlClient;

namespace KingHostManager
{
    /// <summary>
    /// Classe de utilidades do banco de dados.
    /// </summary>
    public class DbUtils
    {
        /// <summary>
        /// Classe de utilidades do banco de dados.
        /// </summary>
        /// <param name="connectionString"></param>
        public DbUtils(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");

            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// String de conexão.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Retorna a string de conexão com o banco de dados.
        /// </summary>
        /// <returns></returns>
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(this.ConnectionString);
        }

        /// <summary>
        /// Insere históico que um boleto foi enviado.
        /// </summary>
        /// <param name="idBoleto"></param>
        /// <param name="dadosBoleto"></param>
        public void InsereHistorico(int idBoleto, DadosBoleto dadosBoleto)
        {
            const string Sql = @"
                        INSERT INTO historicoboleto (
                            Id_Boleto,
                            CodigoBoleto,
                            DtHrEnvio,
                            Url,
                            Pago
                        ) VALUES (
                            @IdBoleto,
                            @CodigoBoleto,
                            @DtHrEnvio,
                            @Url,
                            0
                        )";

            using (var connection = this.GetConnection())
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(Sql))
                {
                    command.Connection = connection;
                    command.Parameters.Add("IdBoleto", MySqlDbType.Int32).Value = idBoleto;
                    command.Parameters.Add("CodigoBoleto", MySqlDbType.Int32).Value = dadosBoleto.Numero;
                    command.Parameters.Add("DtHrEnvio", MySqlDbType.DateTime).Value = DateTime.Now;
                    command.Parameters.Add("Url", MySqlDbType.String).Value = dadosBoleto.Url;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Busca os clientes que devem receber e-mail.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ClienteEnvioBoleto> GetClientesParaEnvio()
        {
            const string Sql = @"
                SELECT
                    boleto.Id As IdBoleto,
                    Valor,
                    DiaEnvio,
                    DtHrUltimoEnvio,
                    dominio.CodigoKingHost As CodigoDominio,
                    dominio.Dominio,
                    cliente.Nome,
                    cliente.Email,
                    cliente.CodigoKingHost As CodigoCliente
                FROM
                    boleto
                LEFT JOIN dominio ON (dominio.Id = boleto.Id_Dominio)
                LEFT JOIN cliente ON (cliente.Id = dominio.Id_Cliente)
                WHERE
                    dominio.IsAtivo = 1";

            using (var connection = this.GetConnection())
            {
                // Abre a conexão com o banco de dados.
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(Sql))
                {
                    command.Connection = connection;
                    var dataReader = command.ExecuteReader();

                    var clientesEnvioBoleto = new List<ClienteEnvioBoleto>();

                    while (dataReader.Read())
                        clientesEnvioBoleto.Add(new ClienteEnvioBoleto()
                        {
                            IdBoleto = (int)dataReader["IdBoleto"],
                            Valor = (float)dataReader["Valor"],
                            DiaEnvio = (int)dataReader["DiaEnvio"],
                            DtHrUltimoEnvio = dataReader["DtHrUltimoEnvio"] as DateTime?,
                            CodigoDominio = (int)dataReader["CodigoDominio"],
                            Dominio = (string)dataReader["Dominio"],
                            Nome = (string)dataReader["Nome"],
                            Email = (string)dataReader["Email"],
                            CodigoCliente = (int)dataReader["CodigoCliente"],
                        });

                    // Fecha o leitor.
                    dataReader.Close();

                    return clientesEnvioBoleto;
                }
            }
        }

        /// <summary>
        /// Atualiza a data de envio do boleto.
        /// </summary>
        /// <param name="idBoleto"></param>
        public void AtualizaDataEnvio(int idBoleto)
        {
            const string Sql = "UPDATE boleto SET DtHrUltimoEnvio = @DtHrUltimoEnvio WHERE Id = @Id";

            using (var connection = this.GetConnection())
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(Sql))
                {
                    command.Connection = connection;
                    command.Parameters.Add("Id", MySqlDbType.Int32).Value = idBoleto;
                    command.Parameters.Add("DtHrUltimoEnvio", MySqlDbType.DateTime).Value = DateTime.Now;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Retorna os clientes que possuem boletos em aberto.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ClienteEnvioBoleto> GetClientesEmAberto()
        {
            const string Sql = @"
                SELECT
                    historicoboleto.Id,
                    historicoboleto.CodigoBoleto,
                    cliente.CodigoKingHost As CodigoCliente,
                    dominio.Dominio
                FROM
                    historicoboleto
                LEFT JOIN boleto ON (boleto.Id = historicoboleto.Id_Boleto)
                LEFT JOIN dominio ON (dominio.Id = boleto.Id_Dominio)
                LEFT JOIN cliente ON (cliente.Id = dominio.Id_Cliente)
                WHERE
                    Pago IS NULL OR Pago = 0";

            using (var connection = this.GetConnection())
            {
                // Abre a conexão com o banco de dados.
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(Sql))
                {
                    command.Connection = connection;
                    var dataReader = command.ExecuteReader();

                    var clientesEnvioBoleto = new List<ClienteEnvioBoleto>();

                    while (dataReader.Read())
                        clientesEnvioBoleto.Add(new ClienteEnvioBoleto()
                        {
                            CodigoHistorico = (int)dataReader["Id"],
                            CodigoBoleto = (int)dataReader["CodigoBoleto"],
                            CodigoCliente = (int)dataReader["CodigoCliente"],
                            Dominio = (string)dataReader["Dominio"]
                        });

                    // Fecha o leitor.
                    dataReader.Close();

                    return clientesEnvioBoleto;
                }
            }
        }

        /// <summary>
        /// Atualiza o status do boleto para pago.
        /// </summary>
        /// <param name="idHistorico"></param>
        public void AtualizaBoletoPago(int idHistorico)
        {
            const string Sql = "UPDATE historicoboleto SET Pago = 1 WHERE Id = @Id";

            using (var connection = this.GetConnection())
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(Sql))
                {
                    command.Connection = connection;
                    command.Parameters.Add("Id", MySqlDbType.Int32).Value = idHistorico;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
