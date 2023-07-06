using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.Entities
{
    public class Reply
    {
        [Required(ErrorMessage = "Soru id boş bırakılamaz!")]
        public int QuestionId { get; set; }
        public List<string> Content { get; set; }
    }
}
