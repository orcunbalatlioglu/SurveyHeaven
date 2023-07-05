using AutoMapper;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;
using SurveyHeaven.Domain.Entities;
using SurveyHeaven.DomainService.Repositories;

namespace SurveyHeaven.Application.Services
{
    public class SurveyService : BaseService<Survey, CreateSurveyRequest, UpdateSurveyRequest, SurveyDisplayResponse>, ISurveyService
    {
        private readonly ISurveyRepository _repository;
        private readonly IMapper _mapper;

        public SurveyService(ISurveyRepository repository,
                             IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public void Update(UpdateSurveyRequest request)
        {
            var updatedSurvey = _mapper.Map<Survey>(request);
            updatedSurvey.Id = request.Id;

            //Anketin kim tarafından oluşturulduğu değişmemesi gerektiği için sunucudaki varlıktan bu bilgiyi alıyorum.
            var unchangedSurvey = GetById(updatedSurvey.Id);
            updatedSurvey.CreatedUserId = unchangedSurvey.CreatedUserId;
            updatedSurvey.LastEditByUserId = unchangedSurvey.LastEditByUserId;

            _repository.Update(updatedSurvey.Id, updatedSurvey);
        }

        public async Task UpdateAsync(UpdateSurveyRequest request)
        {
            var updatedSurvey = _mapper.Map<Survey>(request);
            updatedSurvey.Id = request.Id;

            //Anketin kim tarafından oluşturulduğu değişmemesi gerektiği için sunucudaki varlıktan bu bilgiyi alıyorum.
            var unchangedSurvey = await GetByIdAsync(updatedSurvey.Id);
            updatedSurvey.CreatedUserId = unchangedSurvey.CreatedUserId;
            updatedSurvey.LastEditByUserId = unchangedSurvey.LastEditByUserId;

            await _repository.UpdateAsync(request.Id, updatedSurvey);
        }

        public void Update(UpdateSurveyRequest request, string signedInUserId)
        {
            var updatedSurvey = _mapper.Map<Survey>(request);
            updatedSurvey.Id = request.Id;

            //Anketin kim tarafından oluşturulduğu değişmemesi gerektiği için sunucudaki varlıktan bu bilgiyi alıyorum.
            var unchangedSurvey = GetById(updatedSurvey.Id);
            updatedSurvey.CreatedUserId = unchangedSurvey.CreatedUserId;
            //Admin veya editör tarafından düzenlenmesi halinde en son düzenleyenin kim olduğu bilgisini tutmak amacıyla bu özelliği koydum.
            updatedSurvey.LastEditByUserId = signedInUserId;

            _repository.Update(updatedSurvey.Id, updatedSurvey);
        }

        public async Task UpdateAsync(UpdateSurveyRequest request, string signedInUserId)
        {
            var updatedSurvey = _mapper.Map<Survey>(request);
            updatedSurvey.Id = request.Id;

            //Anketin kim tarafından oluşturulduğu değişmemesi gerektiği için sunucudaki varlıktan bu bilgiyi alıyorum.
            var unchangedSurvey = await GetByIdAsync(updatedSurvey.Id);
            updatedSurvey.CreatedUserId = unchangedSurvey.CreatedUserId;
            //Admin veya editör tarafından düzenlenmesi halinde en son düzenleyenin kim olduğu bilgisini tutmak amacıyla bu özelliği koydum.
            updatedSurvey.LastEditByUserId = signedInUserId;

            await _repository.UpdateAsync(request.Id, updatedSurvey);
        }

        public IEnumerable<SurveyDisplayResponse> GetByCreatedUserId(string userId)
        {
            var surveys = _repository.GetAllWithPredicate((s => s.CreatedUserId == userId));
            var surveyDisplays = _mapper.Map<IEnumerable<SurveyDisplayResponse>>(surveys);
            return surveyDisplays;
        }

        public async Task<IEnumerable<SurveyDisplayResponse>> GetByCreatedUserIdAsync(string userId)
        {
            var surveys = await _repository.GetAllWithPredicateAsync((s => s.CreatedUserId == userId));
            var surveyDisplays = _mapper.Map<IEnumerable<SurveyDisplayResponse>>(surveys);
            return surveyDisplays;
        }

        public string CreateAndReturnId(CreateSurveyRequest request)
        {
            var survey = _mapper.Map<Survey>(request);
            _repository.Add(survey);
            return survey.Id;
        }

        public async Task<string> CreateAndReturnIdAsync(CreateSurveyRequest request)
        {
            var survey = _mapper.Map<Survey>(request);
            await _repository.AddAsync(survey);
            return survey.Id;
        }
    }
}
