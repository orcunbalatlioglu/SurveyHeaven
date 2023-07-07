using WebUI.Extensions;
using WebUI.Models.DTOs.Requests;
using WebUI.Models.DTOs.Responses;
using WebUI.Models;
using System.Security.Claims;

namespace WebUI.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _contextAccessor;
        private const string basePath = "/api/User";
        private const string loginPath = $"/Login";
        private const string createPath = "/Create";
        private const string editPath = "/Edit";
        private const string editProfilePath = "/ProfileEdit";
        private const string deletePath = "/Delete?id=";
        private const string getPath = "/Get?id=";
        private const string getProfilePath = "/GetProfile";
        private const string getForEditPath = "/GetForEdit?id=";
        private const string getAllPath = "/GetAll";
        private const string getProfileForEditPath = "/GetProfileForEdit";

        public UserService(HttpClient client, 
                           IHttpContextAccessor contextAccessor)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(client));

        }

        public async Task<LoginToken> LoginAsync(LoginRequest request)
        {
            JsonContent content = JsonContent.Create(request);

            var response = await _client.PostAsync(basePath + loginPath, content);

            return await response.ReadContentAsync<LoginToken>();
        }

        public async Task<HttpResponseMessage> CreateAsync(CreateUserRequest request)
        {
            JsonContent content = JsonContent.Create(request);

            var response = await _client.PostAsync(basePath + createPath, content);

            return response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> EditAsync(UpdateUserRequest request)
        {
            JsonContent content = JsonContent.Create(request);
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.PutAsync(basePath + editPath, content);

            return response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> EditProfileAsync(UpdateUserRequest request)
        {
            JsonContent content = JsonContent.Create(request);
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.PutAsync(basePath + editProfilePath, content);

            return response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.DeleteAsync(basePath + deletePath + id);

            return response.EnsureSuccessStatusCode();
        }

        public async Task<UserDisplayResponse> GetAsync(string id)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getPath + id);

            return await response.ReadContentAsync<UserDisplayResponse>();
        }

        public async Task<UserDisplayResponse> GetProfileAsync()
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getProfilePath);

            return await response.ReadContentAsync<UserDisplayResponse>();
        }

        public async Task<UpdateUserRequest> GetForEditAsync(string id)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getForEditPath + id);

            return await response.ReadContentAsync<UpdateUserRequest>();
        }

        public async Task<IEnumerable<UserDisplayResponse>> GetAllAsync()
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getAllPath);

            return await response.ReadContentAsync<IEnumerable<UserDisplayResponse>>();
        }

        public async Task<UpdateUserRequest> GetProfileForEditAsync()
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getProfileForEditPath);

            return await response.ReadContentAsync<UpdateUserRequest>();
        }
    }
}
