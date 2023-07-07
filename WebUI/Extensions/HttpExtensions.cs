using NuGet.Common;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace WebUI.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ReadContentAsync<T>(this HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = JsonSerializer.Deserialize<T>(
                dataAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return result;
        }

        public static void InjectJwtToRequest(this HttpClient httpClient, IHttpContextAccessor httpContext)
        {
            string token = string.Empty;
            var tokenClaim = httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Upn);
            if(tokenClaim != null) 
            { 
                token = tokenClaim.Value;
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    }
}
