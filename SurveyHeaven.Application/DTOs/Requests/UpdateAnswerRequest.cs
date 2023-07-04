using SurveyHeaven.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SurveyHeaven.Application.DTOs.Requests
{
    public class UpdateAnswerRequest : IDto
    {
        [Required(ErrorMessage = "Id boş bırakılamaz!")]
        public string Id { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Survey Id boş bırakılamaz!")]
        public string SurveyId { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "En az bir soru cevaplanmalı!")]
        public List<Reply> Replies { get; set; }
    }
}
