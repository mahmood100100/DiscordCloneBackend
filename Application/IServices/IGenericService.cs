using System.Linq.Expressions;

namespace DiscordCloneBackend.Application.IServices
{
    public interface IGenericService<T , TDtoRequest , TDtoResponse>
        where T : class
        where TDtoRequest : class
        where TDtoResponse : class
    {
        Task<IEnumerable<TDtoResponse>> GetAllAsync(int pageSize = 10, int pageNumber = 1, string includeProperties = null , Expression<Func<T, bool>> filter = null);
        Task<TDtoResponse> GetByIdAsync(string id , string includeProperties = null);
        Task AddAsync(TDtoRequest dto);
        Task<TDtoResponse?> UpdateAsync(string id, TDtoRequest dto);
        Task DeleteAsync(string id);
    }
}
