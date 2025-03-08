using System.Linq.Expressions;

namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            int pageSize = 10,
            int pageNumber = 1,
            string includeProperties = null,
            Expression<Func<T, bool>> filter = null
        );

        Task<T> GetByIdAsync(string id, string includeProperties = null);
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(string id);
    }
}
