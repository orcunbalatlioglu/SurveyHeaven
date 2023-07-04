using AutoMapper;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;
using SurveyHeaven.Domain.Entities;
using SurveyHeaven.DomainService.Repositories;

namespace SurveyHeaven.Application.Services
{
    public class AnswerService : BaseService<Answer, CreateAnswerRequest, UpdateAnswerRequest, AnswerDisplayResponse>, IAnswerService
    {
        private readonly IAnswerRepository _repository;
        private readonly IMapper _mapper;

        public AnswerService(IAnswerRepository repository,
                             IMapper mapper) : base(repository,mapper) 
        { 
            _repository = repository;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateAnswerRequest request,string ipAddress)
        {
            var answer = _mapper.Map<Answer>(request);
            answer.UserIp = ipAddress;
            await _repository.AddAsync(answer);
        }

        public IEnumerable<AnswerDisplayResponse> GetBySurveyId(string surveyId)
        {
            var answers = _repository.GetAllWithPredicate((a => a.SurveyId == surveyId));
            var answerDisplays = _mapper.Map<IEnumerable<AnswerDisplayResponse>>(answers);
            return answerDisplays;
        }

        public async Task<IEnumerable<AnswerDisplayResponse>> GetBySurveyIdAsync(string surveyId)
        {
            var answers = await _repository.GetAllWithPredicateAsync((a => a.SurveyId == surveyId));
            var answerDisplays = _mapper.Map<IEnumerable<AnswerDisplayResponse>>(answers);
            return answerDisplays;
        }

        public IEnumerable<Answer> GetForSameUserCheckBySurveyId(string surveyId)
        {
            var answers = _repository.GetAllWithPredicate((a => a.SurveyId == surveyId));
            return answers;
        }

        public async Task<IEnumerable<Answer>> GetForSameUserCheckBySurveyIdAsync(string surveyId)
        {
            var answers = await _repository.GetAllWithPredicateAsync((a => a.SurveyId == surveyId));
            return answers;
        }

        public async Task<string> GetIdBySurveyIdAndUserIpAsync(string surveyId, string ipAddress)
        {
            var answers = await _repository.GetAllWithPredicateAsync((a => a.SurveyId == surveyId && a.UserIp == ipAddress));
            var answer = answers[0];
            return answer.Id;
        }

        public async Task<Dictionary<string, string>> GetIpAndUserIdByIdAsync(string id)
        {
            var answer = await _repository.GetAsync(id);
            Dictionary<string, string> ipAndUserId = new Dictionary<string, string>
            {
                { "userId", answer.UserId },
                { "ip", answer.UserIp }
            };
            return ipAndUserId;
        }

        public void Update(UpdateAnswerRequest request, string userId, string userIp)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            updatedAnswer.Id = request.Id;
            updatedAnswer.UserIp = userIp;
            updatedAnswer.UserId = userId;
            _repository.Update(request.Id, updatedAnswer);
        }

        public async Task UpdateAsync(UpdateAnswerRequest request, string userId, string userIp)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            updatedAnswer.Id = request.Id;
            updatedAnswer.UserIp = userIp;
            updatedAnswer.UserId = userId;
            await _repository.UpdateAsync(request.Id, updatedAnswer);
        }
    }
}
