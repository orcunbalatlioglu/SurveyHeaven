using SurveyHeaven.Domain.Common;

namespace SurveyHeaven.Domain.Entities
{
    public class Answer : MongoDbEntity
    {
        public string SurveyId { get; set; }
        public string UserIp { get; set; }
        public string? UserId { get; set; }
        public string? LastEditByUserId { get; set; }
        public List<Reply> Replies { get; set; }
    }
}
