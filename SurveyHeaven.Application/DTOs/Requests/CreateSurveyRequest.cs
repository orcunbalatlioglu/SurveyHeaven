using SurveyHeaven.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SurveyHeaven.Application.DTOs.Requests
{
    public class CreateSurveyRequest : IDto
    {
        [Required(ErrorMessage ="Oluşturan kullanıcı id boş bırakılamaz!")]
        public string CreatedUserId { get; set; }
        [Required]
        [MinLength(1,ErrorMessage = "En az bir adet soru oluşturulmalı!")]
        public List<Question> Questions { get; set; }
    }
}
