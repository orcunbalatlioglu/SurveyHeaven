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

        public IEnumerable<AnswerDisplayResponse> GetBySurveyId(string surveyId)
        {
            var surveys = _repository.GetAllWithPredicate((a => a.SurveyId == surveyId));
            var surveyDisplays = _mapper.Map<IEnumerable<AnswerDisplayResponse>>(surveys);
            return surveyDisplays;
        }

        public async Task<IEnumerable<AnswerDisplayResponse>> GetBySurveyIdAsync(string surveyId)
        {
            var surveys = await _repository.GetAllWithPredicateAsync((a => a.SurveyId == surveyId));
            var surveyDisplays = _mapper.Map<IEnumerable<AnswerDisplayResponse>>(surveys);
            return surveyDisplays;
        }

        public void Update(UpdateAnswerRequest request)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            _repository.Update(request.Id, updatedAnswer);
        }

        public async Task UpdateAsync(UpdateAnswerRequest request)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            await _repository.UpdateAsync(request.Id, updatedAnswer);
        }
    }
}
