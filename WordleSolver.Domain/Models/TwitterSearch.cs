using System.Text.Json.Serialization;

namespace WordleSolver.Domain.Models
{
    public record TwitterSearch
    {
        [JsonPropertyName("data")]
        public List<Data> Data { get; set; }

        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }
    }
}
