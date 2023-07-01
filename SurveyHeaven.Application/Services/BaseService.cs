using AutoMapper;
using SurveyHeaven.Application.DTOs;
using SurveyHeaven.Domain.Common;
using SurveyHeaven.DomainService.Repositories;

namespace SurveyHeaven.Application.Services
{
    public abstract class BaseService<T, TCreate, TUpdate, TDisplay> : IService<TCreate, TUpdate, TDisplay> 
    where T : class, IEntity
    where TCreate : class, IDto
    where TUpdate : class, IDto
    where TDisplay : class, IDto
    {
        private readonly IRepository<T> _repository;
        private readonly IMapper _mapper;

        protected BaseService(IRepository<T> repository,
                              IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public void Create(TCreate request)
        {
            var entity = _mapper.Map<T>(request);
            _repository.Add(entity);
        }

        public async Task CreateAsync(TCreate request)
        {
            var entity = _mapper.Map<T>(request);
            await _repository.AddAsync(entity);
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public IEnumerable<TDisplay> GetAll()
        {
            var entities = _repository.GetAll();
            var displayResponses = _mapper.Map<IEnumerable<TDisplay>>(entities);
            return displayResponses;
        }

        public async Task<IEnumerable<TDisplay>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            var displayResponses = _mapper.Map<IEnumerable<TDisplay>>(entities);
            return displayResponses;
        }

        public TDisplay GetById(string id)
        {
            var entity = _repository.Get(id);
            var displayResponse = _mapper.Map<TDisplay>(entity);
            return displayResponse;
        }

        public async Task<TDisplay> GetByIdAsync(string id)
        {
            var entity = await _repository.GetAsync(id);
            var displayResponse = _mapper.Map<TDisplay>(entity);
            return displayResponse;
        }

        public TUpdate GetForUpdate(string id)
        {
            var entity = _repository.Get(id);
            var updateRequest = _mapper.Map<TUpdate>(entity);
            return updateRequest;
        }

        public async Task<TUpdate> GetForUpdateAsync(string id)
        {
            var entity = await _repository.GetAsync(id);
            var updateRequest = _mapper.Map<TUpdate>(entity);
            return updateRequest;
        }

        public bool IsExist(string id)
        {
            return _repository.IsExists(id);
        }

        public async Task<bool> IsExistsAsync(string id)
        {
            return await _repository.IsExistsAsync(id);
        }

       
    }
}
