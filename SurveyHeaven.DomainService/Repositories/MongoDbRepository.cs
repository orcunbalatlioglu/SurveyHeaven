using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SurveyHeaven.Domain.Common;
using SurveyHeaven.Domain.Entities;
using System.Linq.Expressions;

namespace SurveyHeaven.DomainService.Repositories
{
    public abstract class MongoDbRepository<T> : IRepository<T> where T : MongoDbEntity, new()
    {
        protected readonly IMongoCollection<T> Collection;
        private readonly MongoDbSettings settings;

        protected MongoDbRepository(IOptions<MongoDbSettings> options)
        {
            settings = options.Value;
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.Database);
            Collection = db.GetCollection<T>(typeof(T).Name);
        }

        public virtual void Add(T entity)
        {
            Collection.InsertOne(entity);
        }

        public virtual Task AddAsync(T entity)
        {
            return Collection.InsertOneAsync(entity);
        }

        public virtual void Delete(string id)
        {
            Collection.FindOneAndDelete(x => x.Id == id);
        }

        public virtual Task DeleteAsync(string id)
        {
            return Collection.FindOneAndDeleteAsync(x => x.Id == id);
        }

        public virtual void Update(string id, T entity)
        {
            Collection.FindOneAndReplace(x => x.Id == id, entity);
        }

        public virtual Task UpdateAsync(string id, T entity)
        {
            return Collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
        }

        public virtual T? Get(string id)
        {
            return Collection.Find(x => x.Id == id)
                             .FirstOrDefault();
        }

        public virtual Task<T?> GetAsync(string id)
        {
            return Collection.Find(x => x.Id == id)
                             .FirstOrDefaultAsync();
        }

        public virtual IEnumerable<T?> GetAll()
        {
            return Collection.Find(new BsonDocument())
                             .ToList();
        }

        public virtual async Task<IEnumerable<T?>> GetAllAsync()
        {
            var result = await Collection.FindAsync(new BsonDocument());
            return result.ToEnumerable();
        }

        public virtual List<T> GetAllWithPredicate(Expression<Func<T, bool>> predicate)
        {
            return Collection.Find(predicate)
                             .ToList();
        }

        public virtual Task<List<T>> GetAllWithPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            return Collection.Find(predicate)
                             .ToListAsync();
        }

        public virtual bool IsExists(string id)
        {
            var isExist = Collection.Find(x => x.Id == id).Any();
            return isExist;
        }

        public virtual async Task<bool> IsExistsAsync(string id)
        {
            var isExist = await Collection.Find(x => x.Id == id).AnyAsync();
            return isExist;
        }
    }
}
