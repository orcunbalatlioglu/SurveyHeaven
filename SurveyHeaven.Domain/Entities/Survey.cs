using SurveyHeaven.Domain.Common;

namespace SurveyHeaven.Domain.Entities
{
    public class Survey : MongoDbEntity
    {
        public string CreatedUserId { get; set; }
        public List<Question> Questions { get; set; }
    }
}
