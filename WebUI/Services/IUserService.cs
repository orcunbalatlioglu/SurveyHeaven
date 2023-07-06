using WebUI.Models.DTOs.Requests;
using WebUI.Models.DTOs.Responses;
using WebUI.Models;
namespace WebUI.Services
{
    public interface IUserService
    {
        Task<LoggedUserInfo> LoginAsync(LoginRequest request);
        Task<HttpResponseMessage> CreateAsync(CreateUserRequest request);
        Task<HttpResponseMessage> EditAsync(UpdateUserRequest request);
        Task<HttpResponseMessage> EditProfileAsync(UpdateUserRequest request);
        Task<HttpResponseMessage> DeleteAsync(string id);
        Task<UserDisplayResponse> GetAsync(string id);
        Task<UpdateUserRequest> GetForEditAsync(string id);
        Task<IEnumerable<UserDisplayResponse>> GetAllAsync();
        Task<UpdateUserRequest> GetProfileForEditAsync();
    }
}
