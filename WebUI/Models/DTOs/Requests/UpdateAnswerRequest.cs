using System.ComponentModel.DataAnnotations;
using WebUI.Models.Entities;

namespace WebUI.Models.DTOs.Requests
{
    public class UpdateAnswerRequest
    {
        [Required(ErrorMessage = "Id boş bırakılamaz!")]
        public string Id { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "En az bir soru cevaplanmalı!")]
        public List<Reply> Replies { get; set; }
    }
}
