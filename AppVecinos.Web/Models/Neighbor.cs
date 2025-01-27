using System.Text.Json.Serialization;

namespace AppVecinos.Web.Models
{
    public class Neighbor
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("number")]
        public int Number { get; set; }
        [JsonPropertyName("user")]
        public string User { get; set; }
        [JsonPropertyName("password")]
        public string PasswordHash { get; set; }
        [JsonPropertyName("level")]
        public string Level { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}