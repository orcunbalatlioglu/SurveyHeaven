using SurveyHeaven.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace SurveyHeaven.Domain.Entities
{
    public class Reply : IEntity
    {
        [Required(ErrorMessage = "Soru id boş bırakılamaz!")]
        public string QuestionId { get; set; }
        //TODO: Custom validator ile sorunun zorunlu olup olmamasına göre validasyon ekle.
        public string Content { get; set; }
    }
}
