using Microsoft.Extensions.Options;
using SurveyHeaven.Domain.Entities;

namespace SurveyHeaven.DomainService.Repositories
{
    public class AnswerRepository : MongoDbRepository<Answer>, IAnswerRepository
    {
        public AnswerRepository(IOptions<MongoDbSettings> options) : base(options) { }
    }
}
