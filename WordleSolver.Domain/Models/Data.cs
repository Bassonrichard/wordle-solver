using System.Text.Json.Serialization;

namespace WordleSolver.Domain.Models
{
    public record Data
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
