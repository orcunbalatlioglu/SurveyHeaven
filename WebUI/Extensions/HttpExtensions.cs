using NuGet.Common;
using System.Text.Json;

namespace WebUI.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ReadContentAsync<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == false)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = JsonSerializer.Deserialize<T>(
                dataAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return result;
        }

        public static void InjectJwtToRequest(this HttpClient httpClient, IHttpContextAccessor contextAccessor)
        {
            //TODO: Burada token elde et
            var token = contextAccessor.HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    }
}
