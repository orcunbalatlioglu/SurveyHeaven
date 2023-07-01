
using SurveyHeaven.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SurveyHeaven.Application.DTOs.Responses
{
    public class AnswerDisplayResponse : IDto
    {
        public List<Reply> Replies { get; set; }
    }
}
