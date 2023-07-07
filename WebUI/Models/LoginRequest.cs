using System.ComponentModel.DataAnnotations;

namespace WebUI.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "E-posta adresi boş bırakılamaz")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Şifre boş bırakılamaz")]
    public string Password { get; set; }
}
