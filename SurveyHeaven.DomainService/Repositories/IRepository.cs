using SurveyHeaven.Domain.Common;
using System.Linq.Expressions;

namespace SurveyHeaven.DomainService.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        T? Get(string id);
        Task<T?> GetAsync(string id);
        IEnumerable<T?> GetAll();
        Task<IEnumerable<T?>> GetAllAsync();
        List<T> GetAllWithPredicate(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAllWithPredicateAsync(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        Task AddAsync(T entity);
        void Delete(string id);
        Task DeleteAsync(string id);
        void Update(string id, T entity);
        Task UpdateAsync(string id, T entity);
        bool IsExists(string id);
        Task<bool> IsExistsAsync(string id);
    }
}
