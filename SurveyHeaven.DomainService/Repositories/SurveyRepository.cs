using Microsoft.Extensions.Options;
using SurveyHeaven.Domain.Entities;

namespace SurveyHeaven.DomainService.Repositories
{
    public class SurveyRepository : MongoDbRepository<Survey>, ISurveyRepository
    {
        public SurveyRepository(IOptions<MongoDbSettings> options) : base(options) { }
    }
}
