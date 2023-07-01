using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;

namespace SurveyHeaven.Application.Services
{
    public interface IUserService : IService<CreateUserRequest, UpdateUserRequest, UserDisplayResponse>
    {
        void Update(UpdateUserRequest request);
        Task UpdateAsync(UpdateUserRequest request);
    }
}
