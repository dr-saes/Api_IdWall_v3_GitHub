using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace id_wall.Models
{

    public class Affiliations
    {
        [JsonPropertyName("father")]
        public Father father { get; set; }

        [JsonPropertyName("mother")]
        public Mother mother { get; set; }
    }

    public class Biometric
    {
        [JsonPropertyName("face")]
        public Face face { get; set; }
    }

    public class CanInitiate
    {
        [JsonPropertyName("names")]
        public List<string> names { get; set; }

        [JsonPropertyName("total")]
        public int? total { get; set; }
    }

    public class Cnh
    {
        [JsonPropertyName("orgaoExpedidor")]
        public object orgaoExpedidor { get; set; }

        [JsonPropertyName("observacao")]
        public object observacao { get; set; }

        [JsonPropertyName("numeroEspelho")]
        public object numeroEspelho { get; set; }

        [JsonPropertyName("numeroSeguranca")]
        public object numeroSeguranca { get; set; }

        [JsonPropertyName("numeroRenach")]
        public object numeroRenach { get; set; }

        [JsonPropertyName("dataEmissao")]
        public object dataEmissao { get; set; }

        [JsonPropertyName("estadoEmissao")]
        public object estadoEmissao { get; set; }

        [JsonPropertyName("cidadeEmissao")]
        public object cidadeEmissao { get; set; }

        [JsonPropertyName("primeiraHabilitacao")]
        public object primeiraHabilitacao { get; set; }

        [JsonPropertyName("validade")]
        public object validade { get; set; }

        [JsonPropertyName("numeroRegistro")]
        public object numeroRegistro { get; set; }

        [JsonPropertyName("categoria")]
        public object categoria { get; set; }

        [JsonPropertyName("acc")]
        public object acc { get; set; }

        [JsonPropertyName("permissao")]
        public object permissao { get; set; }

        [JsonPropertyName("filiacao")]
        public Filiacao filiacao { get; set; }

        [JsonPropertyName("dataNascimento")]
        public object dataNascimento { get; set; }

        [JsonPropertyName("numeroCpf")]
        public object numeroCpf { get; set; }

        [JsonPropertyName("ufExpedidorRg")]
        public object ufExpedidorRg { get; set; }

        [JsonPropertyName("orgaoExpedidorRg")]
        public object orgaoExpedidorRg { get; set; }

        [JsonPropertyName("numeroRg")]
        public object numeroRg { get; set; }

        [JsonPropertyName("nome")]
        public object nome { get; set; }

        [JsonPropertyName("imagesUrls")]
        public List<object> imagesUrls { get; set; }
    }

    public class Company
    {
        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("slug")]
        public string slug { get; set; }
    }

    public class Contacts
    {
        [JsonPropertyName("phone")]
        public List<object> phone { get; set; }

        [JsonPropertyName("email")]
        public List<object> email { get; set; }
    }

    public class CreatedBy
    {
        [JsonPropertyName("company")]
        public Company company { get; set; }

        [JsonPropertyName("user")]
        public User user { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("others")]
        public Others others { get; set; }

        [JsonPropertyName("integrations")]
        public Integrations integrations { get; set; }

        [JsonPropertyName("affiliations")]
        public Affiliations affiliations { get; set; }

        [JsonPropertyName("addresses")]
        public List<object> addresses { get; set; }

        [JsonPropertyName("contacts")]
        public Contacts contacts { get; set; }

        [JsonPropertyName("identity")]
        public Identity identity { get; set; }

        [JsonPropertyName("personal")]
        public Personal personal { get; set; }

        [JsonPropertyName("segments")]
        public List<object> segments { get; set; }

        [JsonPropertyName("crons")]
        public List<object> crons { get; set; }

        [JsonPropertyName("pendencies")]
        public List<object> pendencies { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("ref")]
        public string @ref { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? createdAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? updatedAt { get; set; }

        [JsonPropertyName("createdBy")]
        public CreatedBy createdBy { get; set; }

        [JsonPropertyName("images")]
        public List<object> images { get; set; }

        [JsonPropertyName("flows")]
        public Flows flows { get; set; }
    }

    public class Documents
    {
        [JsonPropertyName("rg")]
        public Rg rg { get; set; }

        [JsonPropertyName("cnh")]
        public Cnh cnh { get; set; }
    }

    public class Face
    {
        [JsonPropertyName("imagesUrls")]
        public List<object> imagesUrls { get; set; }
    }

    public class Father
    {
        [JsonPropertyName("name")]
        public object name { get; set; }
    }

    public class Filiacao
    {
        [JsonPropertyName("nomeMae")]
        public object nomeMae { get; set; }

        [JsonPropertyName("nomePai")]
        public object nomePai { get; set; }
    }

    public class Flows
    {
        [JsonPropertyName("totalOfActive")]
        public int? totalOfActive { get; set; }

        [JsonPropertyName("canInitiate")]
        public CanInitiate canInitiate { get; set; }

        [JsonPropertyName("lastFlow")]
        public LastFlow lastFlow { get; set; }

        [JsonPropertyName("lastAt")]
        public DateTime? lastAt { get; set; }
    }

    public class Identity
    {
        [JsonPropertyName("biometric")]
        public Biometric biometric { get; set; }

        [JsonPropertyName("documents")]
        public Documents documents { get; set; }
    }

    public class Integrations
    {
        [JsonPropertyName("sdk")]
        public List<object> sdk { get; set; }
    }

    public class LastFlow
    {
        [JsonPropertyName("profileFlowId")]
        public string profileFlowId { get; set; }

        [JsonPropertyName("flowName")]
        public string flowName { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? updatedAt { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }
    }

    public class Mother
    {
        [JsonPropertyName("name")]
        public object name { get; set; }
    }

    public class Others
    {
        [JsonPropertyName("maritalStatus")]
        public object maritalStatus { get; set; }

        [JsonPropertyName("sex")]
        public object sex { get; set; }
    }

    // public class Personal
    // {
    //     [JsonPropertyName("cpfNumber")]
    //     public string cpfNumber { get; set; }

    //     [JsonPropertyName("birthDate")]
    //     public DateTime? birthDate { get; set; }

    //     [JsonPropertyName("name")]
    //     public string name { get; set; }
    // }

    public class Rg
    {
        [JsonPropertyName("ufEmissao")]
        public object ufEmissao { get; set; }

        [JsonPropertyName("orgaoExpedidor")]
        public object orgaoExpedidor { get; set; }

        [JsonPropertyName("numeroCpf")]
        public object numeroCpf { get; set; }

        [JsonPropertyName("docOrigem")]
        public object docOrigem { get; set; }

        [JsonPropertyName("dataNascimento")]
        public object dataNascimento { get; set; }

        [JsonPropertyName("naturalidade")]
        public object naturalidade { get; set; }

        [JsonPropertyName("filiacao")]
        public Filiacao filiacao { get; set; }

        [JsonPropertyName("dataExpedicao")]
        public object dataExpedicao { get; set; }

        [JsonPropertyName("numeroRg")]
        public object numeroRg { get; set; }

        [JsonPropertyName("nome")]
        public object nome { get; set; }

        [JsonPropertyName("imagesUrls")]
        public List<object> imagesUrls { get; set; }
    }

    public class DataProfileFlowId
    {
        [JsonPropertyName("data")]
        public Data data { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }
    }


}