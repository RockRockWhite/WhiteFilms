using System.Text.Json.Serialization;

namespace  WhiteFilms.API.Models
{
    public class Payload
    {
        [JsonPropertyName("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")]
        public string Name { set; get; }

        [JsonPropertyName("nameid")] public string Id { set; get; }
        [JsonPropertyName("exp")] public int Exp { set; get; }
        [JsonPropertyName("iss")] public string Iss { set; get; }
        [JsonPropertyName("aud")] public string Aud { set; get; }
    }
}