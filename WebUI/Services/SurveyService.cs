using WebUI.Extensions;
using WebUI.Models.DTOs.Requests;
using WebUI.Models.DTOs.Responses;

namespace WebUI.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _contextAccessor;
        private const string basePath = "/api/Survey";        
        private const string createPath = "/Create";
        private const string editPath = "/Edit";
        private const string deletePath = "/Delete?id=";
        private const string getPath = "/Get?id=";
        private const string getForEditPath = "/GetForEdit?id=";
        private const string getByUserIdPath = "/GetByUserId?id=";
        private const string getAllPath = "/GetAll";

        public SurveyService(HttpClient client, 
                             IHttpContextAccessor contextAccessor)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<HttpResponseMessage> CreateAsync(CreateSurveyRequest request)
        {
            JsonContent content = JsonContent.Create(request);
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.PostAsync(basePath + createPath, content);

            return response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> EditAsync(UpdateSurveyRequest request)
        {
            JsonContent content = JsonContent.Create(request);
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.PutAsync(basePath + editPath, content);

            return response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.DeleteAsync(basePath + deletePath + id);

            return response.EnsureSuccessStatusCode();
        }

        public async Task<SurveyDisplayResponse> GetAsync(string id)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getPath + id);

            return await response.ReadContentAsync<SurveyDisplayResponse>();
        }

        public async Task<UpdateSurveyRequest> GetForEditAsync(string id)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getForEditPath + id);

            return await response.ReadContentAsync<UpdateSurveyRequest>();
        }

        public async Task<IEnumerable<SurveyDisplayResponse>> GetByUserIdAsync(string userId)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getByUserIdPath + userId);

            return await response.ReadContentAsync<IEnumerable<SurveyDisplayResponse>>();
        }

        public async Task<IEnumerable<SurveyDisplayResponse>> GetAllAsync()
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getAllPath);

            return await response.ReadContentAsync<IEnumerable<SurveyDisplayResponse>>();
        }


    }
}
