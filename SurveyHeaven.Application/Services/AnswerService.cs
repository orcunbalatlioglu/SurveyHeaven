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

        public void Create(CreateAnswerRequest request, string ipAddress, string userId)
        {
            var answer = _mapper.Map<Answer>(request);
            answer.UserIp = ipAddress;
            answer.UserId = userId;
            _repository.Add(answer);
        }

        public async Task CreateAsync(CreateAnswerRequest request,string ipAddress, string userId)
        {
            var answer = _mapper.Map<Answer>(request);
            answer.UserIp = ipAddress;
            answer.UserId = userId;
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

        public void Update(UpdateAnswerRequest request)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            //Her mapping işleminde yeni bir obje yaratıldığından yeni bir id atanıyor bu yüzden id değişimine uğruyor.
            //Bunu önlemek için request içinde gelen id'i tekrar kendi id'si olarak eşitledim.
            updatedAnswer.Id = request.Id;
            //Güncelleme esnasında değişmemesi gereken değerleri burada eşitledim.
            var unchangedAnswer = GetById(request.Id);
            updatedAnswer.UserIp = unchangedAnswer.UserIp;
            updatedAnswer.UserId = unchangedAnswer.UserId;
            updatedAnswer.SurveyId = unchangedAnswer.SurveyId;
            updatedAnswer.LastEditByUserId = unchangedAnswer.LastEditByUserId;
            _repository.Update(request.Id, updatedAnswer);
        }

        public async Task UpdateAsync(UpdateAnswerRequest request)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            //Her mapping işleminde yeni bir obje yaratıldığından yeni bir id atanıyor bu yüzden id değişimine uğruyor.
            //Bunu önlemek için request içinde gelen id'i tekrar kendi id'si olarak eşitledim.
            updatedAnswer.Id = request.Id;
            //Güncelleme esnasında değişmemesi gereken değerleri burada eşitledim.
            var unchangedAnswer = await GetByIdAsync(request.Id);
            updatedAnswer.UserIp = unchangedAnswer.UserIp;
            updatedAnswer.UserId = unchangedAnswer.UserId;
            updatedAnswer.SurveyId = unchangedAnswer.SurveyId;
            updatedAnswer.LastEditByUserId = unchangedAnswer.LastEditByUserId;

            await _repository.UpdateAsync(request.Id, updatedAnswer);
        }

        public void Update(UpdateAnswerRequest request, string editByUserId)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            //Her mapping işleminde yeni bir obje yaratıldığından yeni bir id atanıyor bu yüzden id değişimine uğruyor.
            //Bunu önlemek için request içinde gelen id'i tekrar kendi id'si olarak eşitledim.
            updatedAnswer.Id = request.Id;
            //Güncelleme esnasında değişmemesi gereken değerleri burada eşitledim.
            var unchangedAnswer = GetById(request.Id);
            updatedAnswer.UserIp = unchangedAnswer.UserIp;
            updatedAnswer.UserId = unchangedAnswer.UserId;
            updatedAnswer.SurveyId = unchangedAnswer.SurveyId;
            //Düzenlemeyi yapmış olan admin ya da editör id'si burada atadım.
            updatedAnswer.LastEditByUserId = editByUserId;

            _repository.Update(request.Id, updatedAnswer);
        }

        public async Task UpdateAsync(UpdateAnswerRequest request, string editByUserId)
        {
            var updatedAnswer = _mapper.Map<Answer>(request);
            //Her mapping işleminde yeni bir obje yaratıldığından yeni bir id atanıyor bu yüzden id değişimine uğruyor.
            //Bunu önlemek için request içinde gelen id'i tekrar kendi id'si olarak eşitledim.
            updatedAnswer.Id = request.Id;
            //Güncelleme esnasında değişmemesi gereken değerleri burada eşitledim.
            var unchangedAnswer = await GetByIdAsync(request.Id);
            updatedAnswer.UserIp = unchangedAnswer.UserIp;
            updatedAnswer.UserId = unchangedAnswer.UserId;
            updatedAnswer.SurveyId = unchangedAnswer.SurveyId;
            //Düzenlemeyi yapmış olan admin ya da editör id'si burada atadım.
            updatedAnswer.LastEditByUserId = editByUserId;

            await _repository.UpdateAsync(request.Id, updatedAnswer);
        }
    }
}
