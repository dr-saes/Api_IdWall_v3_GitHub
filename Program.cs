using System.Globalization;
using System.Text;
using id_wall.Models;
using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using id_wall_v3.Models.Parte1;

public class Program
{
    private const string BaseUrl = "https://api-v3.idwall.co/maestro/profile/";
    private const string Authorization = "Authorization";
    private const string ContentType = "application/json";


    public static async Task Main()
    {
        string filePath = @"C:\Users\Daniel Saes\Pagare\id_wall_V3\src\IN\Resto-Id-Wall.csv";



        //ReadAndSaveCSVFile(filePath);
        //await VerificarExistenciaPerfil();
        //await PreencheProfileFlowId();
        await PreencheResultado();
        // await PreencheResultado(26947);
        //MontaPlanilhaExcel();

    }

    private static List<Pessoa> ReadAndSaveCSVFile(string filePath)
    {
        var pessoas = new List<Pessoa>();
        string connectionString = "conexao com o banco";
        MySqlConnection connection = new MySqlConnection(connectionString);

        try
        {
            connection.Open();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                int contador = 0;
                StringBuilder logBuilder = new StringBuilder();

                while ((line = sr.ReadLine()) != null)
                {
                    contador++;
                    var values = line.Split(';');

                    if (values.Length >= 1)
                    {
                        if (values[0].Length < 11)
                        {
                            string cpfComZeros = values[0].PadLeft(11, '0');
                            values[0] = cpfComZeros;
                        }
                    }

                    if (values.Length >= 3)
                    {
                        var pessoa = new Pessoa
                        {
                            cpf = values[0],
                            nome = values[1],
                            nascimento = ConverterDataBanco(values[2]),
                            reference = null,
                            resultado = null
                        };

                        if (!CPF(pessoa.cpf))
                        {
                            logBuilder.AppendLine("CPF inválido: " + pessoa.cpf + ". Gravando no banco de dados como 'CPF-INVÁLIDO'.");

                            string insertQueryInvalido = "INSERT INTO TabelaCpfs (cpf, nome, nascimento, resultado) VALUES (@cpf, @nome, @nascimento, @resultado)";
                            MySqlCommand insertCommandInvalido = new MySqlCommand(insertQueryInvalido, connection);
                            insertCommandInvalido.Parameters.AddWithValue("@cpf", pessoa.cpf);
                            insertCommandInvalido.Parameters.AddWithValue("@nome", pessoa.nome);
                            insertCommandInvalido.Parameters.AddWithValue("@nascimento", pessoa.nascimento);
                            insertCommandInvalido.Parameters.AddWithValue("@resultado", "CPF-INVÁLIDO");
                            insertCommandInvalido.ExecuteNonQuery();

                            Console.WriteLine("* Registro gravado - CPF inválido - CPF = " + pessoa.cpf + " - Total = " + contador + " CPF's");

                            continue;
                        }

                        string cpfExistQuery = "SELECT COUNT(*) FROM TabelaCpfs WHERE cpf = @cpf";
                        MySqlCommand cpfExistCommand = new MySqlCommand(cpfExistQuery, connection);
                        cpfExistCommand.Parameters.AddWithValue("@cpf", pessoa.cpf);

                        int count = Convert.ToInt32(cpfExistCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            logBuilder.AppendLine("CPF: " + pessoa.cpf + "**************** Já existe no banco de dados. Pular para o próximo.");
                            continue;
                        }

                        string insertQuery = "INSERT INTO tabelacpfs (cpf, reference, nome, nascimento, processadora, criacao, profile) VALUES (@cpf, @reference, @nome, @nascimento, @processadora, @criacao, @profile)";
                        MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@cpf", pessoa.cpf);
                        insertCommand.Parameters.AddWithValue("@reference", pessoa.cpf);
                        insertCommand.Parameters.AddWithValue("@nome", pessoa.nome);
                        insertCommand.Parameters.AddWithValue("@nascimento", pessoa.nascimento);
                        insertCommand.Parameters.AddWithValue("@processadora", "ID-WALL");
                        insertCommand.Parameters.AddWithValue("@criacao", DateTime.Now);
                        insertCommand.Parameters.AddWithValue("@profile", "FALSE");
                        insertCommand.ExecuteNonQuery();

                        string updateStatusQuery = "UPDATE tabelacpfs SET Status = @status WHERE cpf = @cpf";
                        MySqlCommand updateStatusCommand = new MySqlCommand(updateStatusQuery, connection);
                        updateStatusCommand.Parameters.AddWithValue("@status", 0);
                        updateStatusCommand.Parameters.AddWithValue("@situacao", "Processando");
                        updateStatusCommand.Parameters.AddWithValue("@cpf", pessoa.cpf);

                        updateStatusCommand.ExecuteNonQuery();

                        Console.WriteLine("* Registro gravado - status = 0 -  CPF = " + pessoa.cpf + " - Total = " + contador + " cpf's");

                        pessoas.Add(pessoa);
                    }

                    //Console.WriteLine("Lendo linha " + contador + " do arquivo...");
                }

                logBuilder.AppendLine("Total de pessoas lidas do arquivo: " + contador);
                Console.WriteLine(logBuilder.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro ao ler o arquivo CSV ou gravar no banco de dados: " + ex.Message);
            Console.WriteLine("Reiniciando o método...");
            return ReadAndSaveCSVFile(filePath); // Reinicia o método
        }
        finally
        {
            connection.Close();
        }

        return pessoas;
    }

    private static string ConverterDataBanco(string inputDate)
    {
        // Converte a data para o formato desejado (yyyy-MM-dd)
        if (DateTime.TryParseExact(inputDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate) ||
            DateTime.TryParseExact(inputDate, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            return parsedDate.ToString("yyyy-MM-dd");
        }
        else
        {
            // Se a conversão falhar, retorne o valor original
            return inputDate;
        }
    }

    public static async Task VerificarExistenciaPerfil()
    {
        while (true)
        {
            try
            {
                List<Pessoa> pessoas = new List<Pessoa>();
                string connectionString = "conexao com o banco";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT * FROM tabelacpfs";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            Pessoa pessoa = new Pessoa
                            {
                                cpf = reader.IsDBNull(reader.GetOrdinal("cpf")) ? null : reader.GetString("cpf"),
                                nome = reader.IsDBNull(reader.GetOrdinal("nome")) ? null : reader.GetString("nome"),
                                nascimento = reader.IsDBNull(reader.GetOrdinal("nascimento")) ? null : reader.GetString("nascimento"),
                                reference = reader.IsDBNull(reader.GetOrdinal("reference")) ? null : reader.GetString("reference"),
                                resultado = reader.IsDBNull(reader.GetOrdinal("resultado")) ? null : reader.GetString("resultado"),
                                profile = reader.IsDBNull(reader.GetOrdinal("profile")) ? null : reader.GetString("profile"),
                                id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32("id")
                            };

                            pessoas.Add(pessoa);
                        }
                    }
                }

                for (int i = 0; i < pessoas.Count; i++)
                {
                    var pessoa = pessoas[i];
                    string reference = pessoa.reference;
                    bool profileExists = !string.IsNullOrEmpty(pessoa.profile) && pessoa.profile.ToUpper() != "FALSE";

                    if (!profileExists)
                    {
                        Console.WriteLine($"Verificando índice {i} da lista...");

                        string url = $"https://api-v3.idwall.co/maestro/profile/{reference}";

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", Authorization);
                            client.DefaultRequestHeaders.Add("Accept", ContentType);

                            HttpResponseMessage response = await client.GetAsync(url);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"O profile correspondente ao cpf: {reference} já existe..!");

                                string updateQuery = "UPDATE tabelacpfs SET profile = @profile, status = @status, atualizacao = @atualizacao WHERE reference = @reference";
                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    await connection.OpenAsync();
                                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                                    updateCommand.Parameters.AddWithValue("@profile", "TRUE");
                                    updateCommand.Parameters.AddWithValue("@status", 1);
                                    updateCommand.Parameters.AddWithValue("@atualizacao", DateTime.Now);
                                    updateCommand.Parameters.AddWithValue("@reference", reference);
                                    await updateCommand.ExecuteNonQueryAsync();
                                }
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                Retorno404VerificaoExistenciaProfile error = await response.Content.ReadFromJsonAsync<Retorno404VerificaoExistenciaProfile>();
                                if (error?.message == "Profile not found")
                                {
                                    Console.WriteLine($"O perfil correspondente ao cpf: {reference} não foi encontrado! - criando perfil.....");

                                    RequisicaoCriacaoProfile requisicaoCriacaoProfile = new RequisicaoCriacaoProfile
                                    {
                                        @ref = reference,
                                        personal = new Personal
                                        {
                                            name = pessoa.nome,
                                            cpfNumber = pessoa.cpf,
                                            birthDate = DateTime.ParseExact(pessoa.nascimento, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")
                                        },
                                        status = 1
                                    };

                                    string createProfileUrl = $"https://api-v3.idwall.co/maestro/profile/";
                                    using (HttpClient createProfileClient = new HttpClient())
                                    {
                                        createProfileClient.DefaultRequestHeaders.Add("Authorization", Authorization);
                                        createProfileClient.DefaultRequestHeaders.Add("Accept", ContentType);

                                        string requestBody = System.Text.Json.JsonSerializer.Serialize(requisicaoCriacaoProfile);
                                        StringContent content = new StringContent(requestBody, Encoding.UTF8, ContentType);

                                        HttpResponseMessage createProfileResponse = await createProfileClient.PostAsync(createProfileUrl, content);

                                        if (createProfileResponse.IsSuccessStatusCode)
                                        {
                                            Console.WriteLine($"Perfil criado para o cpf: {reference}!");

                                            string updateProfileQuery = "UPDATE tabelacpfs SET profile = @profile, status = @status, atualizacao = @atualizacao WHERE reference = @reference";
                                            using (MySqlConnection connection = new MySqlConnection(connectionString))
                                            {
                                                await connection.OpenAsync();
                                                MySqlCommand updateProfileCommand = new MySqlCommand(updateProfileQuery, connection);
                                                updateProfileCommand.Parameters.AddWithValue("@profile", "TRUE");
                                                updateProfileCommand.Parameters.AddWithValue("@status", 1);
                                                updateProfileCommand.Parameters.AddWithValue("@atualizacao", DateTime.Now);
                                                updateProfileCommand.Parameters.AddWithValue("@reference", reference);
                                                await updateProfileCommand.ExecuteNonQueryAsync();
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Erro ao criar o perfil para o cpf: {reference}. Status: {createProfileResponse.StatusCode}");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Outro código de status, trata conforme necessário
                                Console.WriteLine($"Erro na requisição para o perfil do cpf: {reference}. Status: {response.StatusCode}");
                            }
                        }
                    }
                }

                break; // Sai do loop em caso de sucesso
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu uma exceção: {ex.Message}");
                // Aguarda um tempo antes de reiniciar o método
                await Task.Delay(5000);
            }
        }
    }

    public static async Task PreencheProfileFlowId()
    {
        string connectionString = "conexao com o banco";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM tabelacpfs";

            MySqlCommand command = new MySqlCommand(query, connection);

            List<Pessoa> pessoas = new List<Pessoa>();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Pessoa pessoa = new Pessoa
                    {
                        cpf = reader.IsDBNull(reader.GetOrdinal("cpf")) ? null : reader.GetString("cpf"),
                        nome = reader.IsDBNull(reader.GetOrdinal("nome")) ? null : reader.GetString("nome"),
                        nascimento = reader.IsDBNull(reader.GetOrdinal("nascimento")) ? null : reader.GetString("nascimento"),
                        reference = reader.IsDBNull(reader.GetOrdinal("reference")) ? null : reader.GetString("reference"),
                        id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32("id"),
                        profileFlowId = reader.IsDBNull(reader.GetOrdinal("profileFlowId")) ? null : reader.GetString("profileFlowId")
                    };

                    pessoas.Add(pessoa);
                }

                reader.Close();
            }

            foreach (Pessoa pessoa in pessoas)
            {
                if (string.IsNullOrEmpty(pessoa.profileFlowId))
                {
                    try
                    {
                        string profileFlowId = await ObterProfileFlowId(pessoa.reference);
                        AtualizarProfileFlowIdNoBancoDeDados(connection, pessoa.id, profileFlowId);
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Ocorreu uma exceção: {ex.GetType()}");
                        Console.WriteLine($"Mensagem de erro: {ex.Message}");
                        Console.WriteLine("Pulando para o próximo item...");
                    }
                }
            }
        }
    }

    public static async Task<string> ObterProfileFlowId(string reference)
    {
        string endpoint = $"https://api-v3.idwall.co/maestro/profile/{reference}";
        string connectionString = "conexao com o banco";
        int id;

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", Authorization);
            client.DefaultRequestHeaders.Add("Accept", ContentType);

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Consulta SQL para obter o índice correspondente à referência
                    string idQuery = "SELECT id FROM tabelacpfs WHERE reference = @reference";
                    MySqlCommand indiceCommand = new MySqlCommand(idQuery, connection);
                    indiceCommand.Parameters.AddWithValue("@reference", reference);
                    id = Convert.ToInt32(await indiceCommand.ExecuteScalarAsync());
                }

                Console.WriteLine($"Obtendo ProfileFlowId para o índice {id}...");

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    DataProfileFlowId retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<DataProfileFlowId>(jsonResponse);
                    string profileFlowId = retorno.data.flows.lastFlow.profileFlowId;

                    Console.WriteLine($"ProfileFlowId obtido para o índice {id}: {profileFlowId}");
                    return profileFlowId;
                }
                else
                {
                    Console.WriteLine($"Erro na chamada. Código de status: {response.StatusCode}");
                    throw new HttpRequestException($"Erro na chamada. Código de status: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro na chamada HTTP: {ex.Message}");
                throw;
            }
        }
    }

    public static void AtualizarProfileFlowIdNoBancoDeDados(MySqlConnection connection, int pessoaId, string profileFlowId)
    {
        string query = "UPDATE tabelacpfs SET profileFlowId = @profileFlowId, status = @status, atualizacao = @atualizacao WHERE id = @pessoaId";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@profileFlowId", profileFlowId);
            command.Parameters.AddWithValue("@status", "2");
            command.Parameters.AddWithValue("@atualizacao", DateTime.Now);
            command.Parameters.AddWithValue("@pessoaId", pessoaId);
            command.ExecuteNonQuery();
        }
    }

    public static bool CPF(string cpf)
    {
        int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCpf;
        string digito;
        int soma;
        int resto;

        cpf = cpf.Trim();
        cpf = cpf.Replace(".", "").Replace("-", "");

        if (cpf.Length != 11)
        {
            tempCpf = cpf.Length >= 9 ? cpf.Substring(0, 9) : cpf;
            soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            }

            resto = soma % 11;
            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;
            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digito = digito + resto.ToString();
            cpf = cpf + digito;
        }
        else
        {
            digito = cpf.Substring(9, 2);
        }

        if (cpf.Length < 11)
        {
            cpf = cpf.PadLeft(11, '0');
        }

        return cpf.EndsWith(digito);
    }

    public static async Task PreencheResultado()
    {
        string connectionString = "conexao com o banco";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM tabelacpfs";

            MySqlCommand command = new MySqlCommand(query, connection);

            List<Pessoa> pessoas = new List<Pessoa>();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Pessoa pessoa = new Pessoa
                    {
                        cpf = reader.IsDBNull(reader.GetOrdinal("cpf")) ? null : reader.GetString("cpf"),
                        nome = reader.IsDBNull(reader.GetOrdinal("nome")) ? null : reader.GetString("nome"),
                        nascimento = reader.IsDBNull(reader.GetOrdinal("nascimento")) ? null : reader.GetString("nascimento"),
                        reference = reader.IsDBNull(reader.GetOrdinal("reference")) ? null : reader.GetString("reference"),
                        id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32("id"),
                        profileFlowId = reader.IsDBNull(reader.GetOrdinal("profileFlowId")) ? null : reader.GetString("profileFlowId"),
                        resultado = reader.IsDBNull(reader.GetOrdinal("resultado")) ? null : reader.GetString("resultado")
                    };

                    pessoas.Add(pessoa);
                }

                reader.Close();
            }

            foreach (Pessoa pessoa in pessoas)
            {
                if (string.IsNullOrEmpty(pessoa.resultado))
                {
                    try
                    {
                        string resultado = await ObterResultado(pessoa.profileFlowId, pessoa.id);
                        AtualizarResultadoNoBancoDeDados(connection, pessoa.id, resultado);
                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine($"Ocorreu uma exceção: {ex.GetType()}");
                        Console.WriteLine($"Mensagem de erro: {ex.Message}");
                        Console.WriteLine("Pulando para o próximo item...");
                    }
                }
            }
        }
    }

    public static async Task PreencheResultado(int startingId)
    {
        string connectionString = "conexao com o banco";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = $"SELECT * FROM tabelacpfs WHERE id >= {startingId}";

            MySqlCommand command = new MySqlCommand(query, connection);

            List<Pessoa> pessoas = new List<Pessoa>();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Pessoa pessoa = new Pessoa
                    {
                        cpf = reader.IsDBNull(reader.GetOrdinal("cpf")) ? null : reader.GetString("cpf"),
                        nome = reader.IsDBNull(reader.GetOrdinal("nome")) ? null : reader.GetString("nome"),
                        nascimento = reader.IsDBNull(reader.GetOrdinal("nascimento")) ? null : reader.GetString("nascimento"),
                        reference = reader.IsDBNull(reader.GetOrdinal("reference")) ? null : reader.GetString("reference"),
                        id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32("id"),
                        profileFlowId = reader.IsDBNull(reader.GetOrdinal("profileFlowId")) ? null : reader.GetString("profileFlowId")
                    };

                    pessoas.Add(pessoa);
                }

                reader.Close();
            }

            foreach (Pessoa pessoa in pessoas)
            {
                if (string.IsNullOrEmpty(pessoa.resultado))
                {
                    string resultado = await ObterResultado(pessoa.profileFlowId, pessoa.id);
                    AtualizarResultadoNoBancoDeDados(connection, pessoa.id, resultado);
                }
            }
        }
    }

    public static async Task<string> ObterResultado(string profileFlowId, int id)
    {
        string endpoint = $"https://api-v3.idwall.co/maestro/profile/profileFlow/{profileFlowId}";
        string connectionString = "conexao com o banco";

        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(2);
            client.DefaultRequestHeaders.Add("Authorization", Authorization);
            client.DefaultRequestHeaders.Add("Accept", ContentType);

            try
            {
                Console.WriteLine($"Obtendo resultado para o índice {id}...");

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Consulta SQL para obter o índice correspondente à referência
                    string idQuery = "SELECT id FROM tabelacpfs WHERE profileFlowId = @profileFlowId";
                    MySqlCommand indiceCommand = new MySqlCommand(idQuery, connection);
                    indiceCommand.Parameters.AddWithValue("@profileFlowId", profileFlowId);
                    id = Convert.ToInt32(await indiceCommand.ExecuteScalarAsync());
                }

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    RetornoResultadoREGULAR retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<RetornoResultadoREGULAR>(jsonResponse);
                    string resultado = retorno.data.stages[0].response.data.cpf.cpf_situacao_cadastral;

                    Console.WriteLine($"Resultado obtido para o índice {id}: {resultado}");
                    return resultado;
                }
                else
                {
                    Console.WriteLine($"Erro na chamada. Código de status: {response.StatusCode}");
                    throw new HttpRequestException($"Erro na chamada. Código de status: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro na chamada HTTP: {ex.Message}");
                throw;
            }
        }
    }

    public static void AtualizarResultadoNoBancoDeDados(MySqlConnection connection, int pessoaId, string resultado)
    {
        string query = "UPDATE tabelacpfs SET resultado = @resultado, status = @status, atualizacao = @atualizacao WHERE id = @pessoaId";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@resultado", resultado);
            command.Parameters.AddWithValue("@status", "3");
            command.Parameters.AddWithValue("@atualizacao", DateTime.Now);
            command.Parameters.AddWithValue("@pessoaId", pessoaId);
            command.ExecuteNonQuery();
        }
    }

    public static List<Pessoa> ObterPessoasDoBancoParaPlanilha()
    {
        string connectionString = "conexao com o banco";
        List<Pessoa> pessoas = new List<Pessoa>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT nome, nascimento, cpf, resultado FROM tabelacpfs";

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string nome = reader.IsDBNull(reader.GetOrdinal("nome")) ? null : reader.GetString("nome");
                    string cpf = reader.IsDBNull(reader.GetOrdinal("cpf")) ? null : reader.GetString("cpf");
                    string resultado = reader.IsDBNull(reader.GetOrdinal("resultado")) ? null : reader.GetString("resultado");
                    DateTime? dataNascimento = null;

                    if (!reader.IsDBNull(reader.GetOrdinal("nascimento")))
                    {
                        dataNascimento = reader.GetDateTime("nascimento");
                    }

                    string dataNascimentoFormatada = dataNascimento?.ToString("dd/MM/yyyy");

                    if (nome == null || cpf == null || resultado == null)
                    {
                        continue; // Pula para a próxima iteração se o nome, CPF ou resultado forem nulos
                    }

                    Pessoa pessoa = new Pessoa
                    {
                        cpf = cpf,
                        nome = nome,
                        nascimento = dataNascimentoFormatada,
                        resultado = resultado
                    };

                    pessoas.Add(pessoa);
                }
            }
        }

        return pessoas;
    }

    public static void MontaPlanilhaExcel()
    {
        List<Pessoa> pessoas = ObterPessoasDoBancoParaPlanilha();

        try
        {
            // Criação do Workbook
            var wb = new XLWorkbook();

            // Criação das planilhas
            var ws = wb.Worksheets.Add("TODOS");
            var wsRegular = wb.Worksheets.Add("REGULARES");
            var wsOutras = wb.Worksheets.Add("OUTROS");

            // Cabeçalho da planilha
            ws.Cell("A1").Value = "CPF";
            ws.Cell("B1").Value = "Nome";
            ws.Cell("C1").Value = "Data de Nascimento";
            ws.Cell("D1").Value = "Situação Cadastral";

            wsRegular.Cell("A1").Value = "CPF";
            wsRegular.Cell("B1").Value = "Nome";
            wsRegular.Cell("C1").Value = "Data de Nascimento";
            wsRegular.Cell("D1").Value = "Situação Cadastral";

            // Corpo do relatório 
            var linha = 2;
            var linhaRegular = 2;
            var linhaOutra = 2;

            foreach (Pessoa pessoa in pessoas)
            {
                try
                {
                    ws.Cell("A" + linha.ToString()).Value = pessoa.cpf;
                    ws.Cell("B" + linha.ToString()).Value = pessoa.nome;
                    ws.Cell("C" + linha.ToString()).Value = pessoa.nascimento;
                    ws.Cell("D" + linha.ToString()).Value = pessoa.resultado;
                }
                catch (System.Data.SqlTypes.SqlNullValueException)
                {
                    // Caso ocorra a exceção SqlNullValueException, pula para a próxima linha
                    linha++;
                    continue;
                }

                linha++;

                if (pessoa.resultado != null && pessoa.resultado.ToUpper() == "REGULAR")
                {
                    wsRegular.Cell("A" + linhaRegular.ToString()).Value = pessoa.cpf;
                    wsRegular.Cell("B" + linhaRegular.ToString()).Value = pessoa.nome;
                    wsRegular.Cell("C" + linhaRegular.ToString()).Value = pessoa.nascimento;
                    wsRegular.Cell("D" + linhaRegular.ToString()).Value = pessoa.resultado;

                    linhaRegular++;
                }
                else
                {
                    wsOutras.Cell("A" + linhaOutra.ToString()).Value = pessoa.cpf;
                    wsOutras.Cell("B" + linhaOutra.ToString()).Value = pessoa.nome;
                    wsOutras.Cell("C" + linhaOutra.ToString()).Value = pessoa.nascimento;
                    wsOutras.Cell("D" + linhaOutra.ToString()).Value = pessoa.resultado;

                    linhaOutra++;
                }
            }

            // Salva planilha
            string caminhoArquivo = @"C:\Users\Daniel Saes\Pagare\id_wall_V3\src\OUT\teste.xlsx";
            wb.SaveAs(caminhoArquivo);

            // Libera cache
            ws.Clear();
            wb.Dispose();

            Console.WriteLine("****** Planilha criada com sucesso! Caminho do arquivo: " + caminhoArquivo);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu uma exceção: " + ex);
        }
    }

}

