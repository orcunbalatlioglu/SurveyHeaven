using Microsoft.Extensions.Options;
using SurveyHeaven.Domain.Entities;

namespace SurveyHeaven.DomainService.Repositories
{
    public class UserRepository : MongoDbRepository<User>, IUserRepository
    {
        public UserRepository(IOptions<MongoDbSettings> options) : base(options) { }
    }
}
