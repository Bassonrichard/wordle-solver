using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Web;
using WordleSolver.Domain.Models;

namespace WordleSolver.Domain
{
    public interface ITwitterService
    {
        public Task<TwitterSearch> GetTwitterSearch(string searchTerm);
    }

    public class TwitterService : ITwitterService
    {
        private readonly IHttpClientFactory _clientFactory;

        public TwitterService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<TwitterSearch> GetTwitterSearch(string searchTerm)
        {
            var client = _clientFactory.CreateClient("Twitter");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"/2/tweets/search/recent?max_results=100&query={searchTerm}", UriKind.Relative),
                Method = HttpMethod.Get
            };

            var response = await client.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TwitterSearch>(jsonResponse);

            return data;

        }
    }
}
