using SurveyHeaven.Domain.Common;

namespace SurveyHeaven.Domain.Entities
{
    public class Answer : MongoDbEntity
    {
        public string SurveyId { get; set; }
        public List<Reply> Replies { get; set; }
    }
}
