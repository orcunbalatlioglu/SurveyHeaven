using WebUI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.DTOs.Requests
{
    public class UpdateSurveyRequest
    {
        [Required(ErrorMessage = "Id boş bırakılamaz!")]
        public string Id { get; set; }
        [Required]
        [MinLength(1,ErrorMessage = "En az bir soru oluşturulmalı!")]
        public List<Question> Questions { get; set; }
    }
}
