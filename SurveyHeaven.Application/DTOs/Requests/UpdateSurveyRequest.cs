using SurveyHeaven.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SurveyHeaven.Application.DTOs.Requests
{
    public class UpdateSurveyRequest : IDto
    {
        [Required(ErrorMessage = "Id boş bırakılamaz!")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Oluşturan kullanıcı id boş bırakılamaz!")]
        public string CreatedUserId { get; set; }
        [Required]
        [MinLength(1,ErrorMessage = "En az bir soru oluşturulmalı!")]
        public List<Question> Questions { get; set; }
    }
}
