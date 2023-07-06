using NuGet.Common;
using System.Text;
using WebUI.Extensions;
using WebUI.Models.DTOs.Requests;
using WebUI.Models.DTOs.Responses;

namespace WebUI.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _contextAccessor;
        private const string basePath = "/api/Answer";
        private const string createPath = "/Create";
        private const string editPath = "/Edit";
        private const string deletePath = "/Delete?id=";
        private const string getPath = "/Get?id=";
        private const string getForEditPath = "/GetForEdit?id=";        
        private const string getAllPath = "/GetAll";
        private const string getSurveyAnswersPath = "/GetSurveyAnswers?surveyId=";

        public AnswerService(HttpClient client,
                             IHttpContextAccessor contextAccessor)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<HttpResponseMessage> CreateAsync(CreateAnswerRequest request)
        {
            JsonContent content = JsonContent.Create(request);
            
            var response = await _client.PostAsync(basePath + createPath, content);

            return response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> EditAsync(UpdateAnswerRequest request)
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

        public async Task<AnswerDisplayResponse> GetAsync(string id)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getPath + id);

            return await response.ReadContentAsync<AnswerDisplayResponse>();
        }

        public async Task<UpdateAnswerRequest> GetForEditAsync(string id)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getForEditPath + id);

            return await response.ReadContentAsync<UpdateAnswerRequest>();
        }
        
        public async Task<IEnumerable<AnswerDisplayResponse>> GetAllAsync()
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getAllPath);

            return await response.ReadContentAsync<IEnumerable<AnswerDisplayResponse>>();
        }

        public async Task<IEnumerable<AnswerDisplayResponse>> GetSurveyAnswers(string surveyId)
        {
            _client.InjectJwtToRequest(_contextAccessor);
            var response = await _client.GetAsync(basePath + getSurveyAnswersPath + surveyId);

            return await response.ReadContentAsync<IEnumerable<AnswerDisplayResponse>>();
        }
    }
}
