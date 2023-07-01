using System.ComponentModel.DataAnnotations;

namespace SurveyHeaven.Domain.Entities
{
    public class Reply
    {
        [Required(ErrorMessage = "Soru boş bırakılamaz!")]
        public Question Question { get; set; }
        //TODO: Custom validator ile sorunun zorunlu olup olmamasına göre validasyon ekle.
        public string Content { get; set; }
    }
}
