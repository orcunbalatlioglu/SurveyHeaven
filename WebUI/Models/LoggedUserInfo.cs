using WebUI.Models.DTOs.Responses;

namespace WebUI.Models
{
    public class LoggedUserInfo
    {
        public string Token { get; set; }
        public UserDisplayResponse UserDisplay { get; set; }
    }
}
