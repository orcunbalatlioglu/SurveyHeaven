using System.ComponentModel.DataAnnotations;

namespace SurveyHeaven.Application.DTOs.Requests
{
    public class UpdateUserRequest : IDto
    {
        [Required(ErrorMessage = "Kullanıcı id boş bırakılamaz!")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Kullanıcı ismi boş bırakılamaz!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Kullanıcı soyismi boş bırakılamaz!")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Kullanıcı e-posta adresi boş bırakılamaz!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Kullanıcı şifresi boş bırakılamaz!")]
        public string Password { get; set; }
    }
}
