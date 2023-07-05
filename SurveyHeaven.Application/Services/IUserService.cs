using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;
using SurveyHeaven.Domain.Entities;

namespace SurveyHeaven.Application.Services
{
    public interface IUserService : IService<CreateUserRequest, UpdateUserRequest, UserDisplayResponse>
    {
        void Update(UpdateUserRequest request);
        Task UpdateAsync(UpdateUserRequest request);
        string CreateAndReturnId(CreateUserRequest request);
        Task<string> CreateAndReturnIdAsync(CreateUserRequest request);
        void CreateEditor(CreateUserRequest request);
        Task CreateEditorAsync(CreateUserRequest request);
        string CreateEditorAndReturnId(CreateUserRequest request);
        Task<string> CreateEditorAndReturnIdAsync(CreateUserRequest request);
        Task<Dictionary<string,string>> ValidateAsync(string email, string password, string jwtKey);
    }
}
