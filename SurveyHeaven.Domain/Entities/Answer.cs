using SurveyHeaven.Domain.Common;

namespace SurveyHeaven.Domain.Entities
{
    public class Answer : MongoDbEntity
    {
        public string SurveyId { get; set; }
        public string userIp { get; set; }
        public string? userId { get; set; }
        public List<Reply> Replies { get; set; }
    }
}
