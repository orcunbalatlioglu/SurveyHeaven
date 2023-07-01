using SurveyHeaven.Domain.Common;

namespace SurveyHeaven.Domain.Entities
{
    public class User : MongoDbEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }
}
