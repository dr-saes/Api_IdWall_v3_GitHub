using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace id_wall.Models
{
    public class Action
    {
        public string type { get; set; }
    }

    public class Automation
    {
        public string id { get; set; }
        public string criteria { get; set; }
        public Rule rule { get; set; }
        public string condition { get; set; }
        public List<string> value { get; set; }
        public List<Action> actions { get; set; }
    }

    // public class Company
    // {
    //     public string id { get; set; }
    //     public string name { get; set; }
    //     public string slug { get; set; }
    // }

    public class Consulta
    {
        public string nome { get; set; }
        public string idConsulta { get; set; }
        public string status_fonte { get; set; }
        public List<Tentativa> tentativas { get; set; }
    }

    public class Cpf
    {
        public string numero { get; set; }
        public string nome { get; set; }
        public string data_de_nascimento { get; set; }
        public string cpf_situacao_cadastral { get; set; }
        public string cpf_data_de_inscricao { get; set; }
        public string cpf_digito_verificador { get; set; }
        public string cpf_anterior_1990 { get; set; }
        public string ano_obito { get; set; }
        public string nome_social { get; set; }
        public int? similaridade { get; set; }
        public List<string> grafias { get; set; }
    }

    // public class CreatedBy
    // {
    //     public Company company { get; set; }
    //     public User user { get; set; }
    // }

    public class Criterion
    {
        public string label { get; set; }
        public string value { get; set; }
        public List<string> result { get; set; }
    }

    public class Data2
    {
        public string id { get; set; }
        public DateTime createdAt { get; set; }
        public List<Stage> stages { get; set; }
        public string status { get; set; }
        public Input input { get; set; }
        public string triggerType { get; set; }
        public string nextStage { get; set; }
        public string profileRef { get; set; }
        public string flowName { get; set; }
        public string flowId { get; set; }
        public DateTime updatedAt { get; set; }
        public CreatedBy createdBy { get; set; }
        public DateTime atualizado_em { get; set; }
        public DateTime criado_em { get; set; }
        public string mensagem { get; set; }
        public string nome { get; set; }
        public string numero { get; set; }
        public string resultado { get; set; }
        public DateTime validado_em { get; set; }
        public bool validado_manualmente { get; set; }
        public List<object> fontes_canceladas_timeout { get; set; }
        public Cpf cpf { get; set; }
    }

    public class Input
    {
        public string flowId { get; set; }
        public string profileRef { get; set; }
        public string triggerType { get; set; }
    }

    public class InputsMapping
    {
        public List<string> here { get; set; }
        public There there { get; set; }
    }

    public class Parameters
    {
        public DateTime atualizado_em { get; set; }
        public string mensagem { get; set; }
        public string nome { get; set; }
        public string numero { get; set; }
        public Parametros parametros { get; set; }
        public string resultado { get; set; }
        public string status { get; set; }
    }

    public class Parametros
    {
        public string cpf { get; set; }
        public string data_de_nascimento { get; set; }
        public string nome { get; set; }
    }

    public class Params
    {
        public string setting { get; set; }
        public List<Rule> rules { get; set; }
        public List<InputsMapping> inputsMapping { get; set; }
    }

    public class Queries
    {
        public string nome_matriz { get; set; }
        public string status_protocolo { get; set; }
        public object numero_pai { get; set; }
        public List<Consulta> consultas { get; set; }
    }

    public class Response
    {
        public string numero { get; set; }
        public string status { get; set; }
        public string nome { get; set; }
        public string mensagem { get; set; }
        public string resultado { get; set; }
        public DateTime validado_em { get; set; }
        public object validado_por { get; set; }
        public bool validado_manualmente { get; set; }
        public DateTime atualizado_em { get; set; }
        public DateTime criado_em { get; set; }
        public string criado_por { get; set; }
        public object numero_pai { get; set; }
        public Queries queries { get; set; }
        public Validations validations { get; set; }
        public Data data { get; set; }
        public Parameters parameters { get; set; }
    }

    public class DataResultado
    {
        public Data2 data { get; set; }
    }

    public class Rule
    {
        public string label { get; set; }
        public string description { get; set; }
        public List<Criterion> criteria { get; set; }
        public string slug { get; set; }
        public List<Automation> automations { get; set; }
    }

    public class Rule2
    {
        public string label { get; set; }
        public string description { get; set; }
        public List<Criterion> criteria { get; set; }
        public string slug { get; set; }
    }

    public class Stage
    {
        public string stageId { get; set; }
        public string status { get; set; }
        public string issuer { get; set; }
        public string type { get; set; }
        public Params @params { get; set; }
        public List<string> conditions { get; set; }
        public DateTime updatedAt { get; set; }
        public string @ref { get; set; }
        public List<UsedField> usedFields { get; set; }
        public Response response { get; set; }
    }

    public class Tentativa
    {
        public int duracao_tentativa { get; set; }
        public DateTime hora_fim_tentativa { get; set; }
        public DateTime hora_inicio_tentativa { get; set; }
        public object msg_erro_tentativa { get; set; }
        public string nome_fonte { get; set; }
        public string status_fonte { get; set; }
        public string status_tentativa { get; set; }
        public object tipo_erro_tentativa { get; set; }
    }

    public class There
    {
        public string input { get; set; }
        public string inputLegacy { get; set; }
        public List<string> options { get; set; }
    }

    public class UsedField
    {
        public string there { get; set; }
        public string field { get; set; }
        public object value { get; set; }
    }

    // public class User
    // {
    //     public string id { get; set; }
    //     public string email { get; set; }
    //     public string name { get; set; }
    // }

    public class Validaco
    {
        public string regra { get; set; }
        public string nome { get; set; }
        public string descricao { get; set; }
        public string resultado { get; set; }
        public string mensagem { get; set; }
    }

    public class Validations
    {
        public DateTime atualizado_em { get; set; }
        public string mensagem { get; set; }
        public string nome { get; set; }
        public string numero { get; set; }
        public string resultado { get; set; }
        public string status { get; set; }
        public List<Validaco> validacoes { get; set; }
        public DateTime validado_em { get; set; }
        public bool validado_manualmente { get; set; }
        public object validado_por { get; set; }
        public object numero_pai { get; set; }
        public List<object> fontes_canceladas_timeout { get; set; }
    }

}

