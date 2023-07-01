namespace SurveyHeaven.Application.DTOs.Responses
{
    public class UserDisplayResponse : IDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
