﻿using SurveyHeaven.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SurveyHeaven.Application.DTOs.Requests
{
    public class CreateAnswerRequest : IDto
    {
        [Required(ErrorMessage = "Anket id boş bırakılamaz!")]
        public string SurveyId { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessage ="Kullanıcı ip adresi boş bırakılamaz!")]
        public string UserIp { get; set; }
        [Required]
        [MinLength(1,ErrorMessage ="En az 1 adet cevap girilmeli!")]
        public List<Reply> Replies { get; set; }
    }
}
