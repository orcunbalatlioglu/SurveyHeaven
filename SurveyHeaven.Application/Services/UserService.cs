using AutoMapper;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;
using SurveyHeaven.Domain.Entities;
using SurveyHeaven.DomainService.Repositories;

namespace SurveyHeaven.Application.Services
{
    public class UserService : BaseService<User, CreateUserRequest, UpdateUserRequest, UserDisplayResponse>, IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository,
                           IMapper mapper) : base(repository,mapper) 
        { 
            _repository = repository;
            _mapper = mapper;        
        }

        public void Update(UpdateUserRequest request)
        {
            var updatedUser = _mapper.Map<User>(request);
            _repository.Update(request.Id, updatedUser);
        }

        public async Task UpdateAsync(UpdateUserRequest request)
        {
            var updatedUser = _mapper.Map<User>(request);
            await _repository.UpdateAsync(request.Id, updatedUser);
        }
    }
}
