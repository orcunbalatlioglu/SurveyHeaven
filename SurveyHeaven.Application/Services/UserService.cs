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

        public override void Create(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var users = _repository.GetAllWithPredicate(u => u.Email == user.Email);
            if (users.Count > 0)
            {
                throw new Exception("Oluşturulmaya çalışılan kullanıcıya ait e-posta adresi zaten kullanımda!");
            }
            _repository.Add(user);
        }

        public override async Task CreateAsync(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var users = _repository.GetAllWithPredicate(u => u.Email == user.Email);
            if (users.Count > 0)
            {
                throw new Exception("Oluşturulmaya çalışılan kullanıcıya ait e-posta adresi zaten kullanımda!");
            }
            await _repository.AddAsync(user);
        }

        public string CreateAndReturnId(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var users = _repository.GetAllWithPredicate(u => u.Email == user.Email);
            if(users.Count > 0)
            {
                throw new Exception("Oluşturulmaya çalışılan kullanıcıya ait e-posta adresi zaten kullanımda!");
            }
            _repository.Add(user);
            return user.Id;
        }

        public async Task<string> CreateAndReturnIdAsync(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var users = await _repository.GetAllWithPredicateAsync(u => u.Email == user.Email);
            if (users.Count > 0)
            {
                throw new Exception("Oluşturulmaya çalışılan kullanıcıya ait e-posta adresi zaten kullanımda!");
            }
            await _repository.AddAsync(user);
            return user.Id;
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

        public async Task<User?> ValidateAsync(string email, string password)
        {
            var users = await _repository.GetAllAsync();
            return users.SingleOrDefault(u => u.Email == email && u.Password == password);
        }
    }
}
