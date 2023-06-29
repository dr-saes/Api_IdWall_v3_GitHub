using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace id_wall.Models
{
    public class RequisicaoCriacaoProfile
    {

        [JsonPropertyName("ref")]
        public string @ref { get; set; }

        [JsonPropertyName("personal")]
        public Personal personal { get; set; }

        [JsonPropertyName("status")]
        public int? status { get; set; }

    }
    public class Personal
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("cpfNumber")]
        public string cpfNumber { get; set; }

        [JsonPropertyName("birthDate")]
        public string birthDate { get; set; }
    }


}
