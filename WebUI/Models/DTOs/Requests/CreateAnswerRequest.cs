using System.ComponentModel.DataAnnotations;
using WebUI.Models.Entities;

namespace WebUI.Models.DTOs.Requests
{
    public class CreateAnswerRequest
    {
        [Required(ErrorMessage = "Anket id boş bırakılamaz!")]
        public string SurveyId { get; set; }
        [Required]
        [MinLength(1,ErrorMessage ="En az 1 adet cevap girilmeli!")]
        public List<Reply> Replies { get; set; }
    }
}
