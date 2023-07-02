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

        public IEnumerable<UpdateAnswerRequest> GetForSameUserCheckBySurveyId(string surveyId)
        {
            var answers = _repository.GetAllWithPredicate((a => a.SurveyId == surveyId));
            var answerUpdateDisplays = _mapper.Map<IEnumerable<UpdateAnswerRequest>>(answers);
            return answerUpdateDisplays;
        }

        public async Task<IEnumerable<Answer>> GetForSameUserCheckBySurveyIdAsync(string surveyId)
        {
            var answers = await _repository.GetAllWithPredicateAsync((a => a.SurveyId == surveyId));
            return answers;
        }

        public void Update(UpdateAnswerRequest request)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            updatedAnswer.Id = request.Id;
            _repository.Update(request.Id, updatedAnswer);
        }

        public async Task UpdateAsync(UpdateAnswerRequest request)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            updatedAnswer.Id = request.Id;
            await _repository.UpdateAsync(request.Id, updatedAnswer);
        }
    }
}
