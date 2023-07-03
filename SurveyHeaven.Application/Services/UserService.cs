using Amazon.Runtime.Internal.Transform;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;
using SurveyHeaven.Domain.Entities;
using SurveyHeaven.DomainService.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
                throw new InvalidOperationException("Oluşturulmaya çalışılan kullanıcıya ait e-posta adresi zaten kullanımda!");
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
            updatedUser.Id = request.Id;
            _repository.Update(request.Id, updatedUser);
        }

        public async Task UpdateAsync(UpdateUserRequest request)
        {
            var updatedUser = _mapper.Map<User>(request);
            updatedUser.Id = request.Id;
            await _repository.UpdateAsync(request.Id, updatedUser);
        }

        public async Task<Dictionary<string,string>> ValidateAsync(string email, string password, string jwtKey)
        {
            var users = await _repository.GetAllAsync();
            var user = users.SingleOrDefault(u => u.Email == email && u.Password == password);
            if (user is not null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes(jwtKey);

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Surname, user.Surname),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                var finalToken = tokenHandler.WriteToken(token);
                Dictionary<string, string> validatedUserInfos = new Dictionary<string, string>();
                validatedUserInfos.Add("Token", finalToken);
                validatedUserInfos.Add("UserId", user.Id);
                return validatedUserInfos;
            }
            return new Dictionary<string,string>();
        }
    }
}
