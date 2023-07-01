using SurveyHeaven.Application.DTOs;
using SurveyHeaven.Domain.Common;

namespace SurveyHeaven.Application.Services
{
    public interface IService<TCreate, TUpdate, TDisplay>
    where TCreate : class, IDto
    where TUpdate : class, IDto
    where TDisplay : class, IDto
    {
        void Create(TCreate request);
        Task CreateAsync(TCreate request);
        void Delete(string id);
        Task DeleteAsync(string id);
        bool IsExist(string id);
        Task<bool> IsExistsAsync(string id);
        TDisplay GetById(string id);
        Task<TDisplay> GetByIdAsync(string id);
        TUpdate GetForUpdate(string id);
        Task<TUpdate> GetForUpdateAsync(string id);
        IEnumerable<TDisplay> GetAll();
        Task<IEnumerable<TDisplay>> GetAllAsync();
    }
}
