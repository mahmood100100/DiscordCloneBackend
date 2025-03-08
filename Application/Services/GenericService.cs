using AutoMapper;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using System.Linq.Expressions;

namespace DiscordCloneBackend.Application.Services
{
    public class GenericService<T, TDtoRequest, TDtoResponse> :
        IGenericService<T, TDtoRequest, TDtoResponse>
        where T : class
        where TDtoRequest : class
        where TDtoResponse : class
    {
        protected readonly IMapper mapper;
        protected readonly IUnitOfWork unitOfWork;

        public GenericService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public virtual async Task AddAsync(TDtoRequest dto)
        {
            var entity = mapper.Map<T>(dto);
            var repository = unitOfWork.GetGenericRepository<T>();
            await repository.AddAsync(entity);
            await unitOfWork.CompleteAsync();
        }

        public virtual async Task DeleteAsync(string id)
        {
            var repository = unitOfWork.GetGenericRepository<T>();
            await repository.DeleteAsync(id);
            await unitOfWork.CompleteAsync();
        }

        public virtual async Task<IEnumerable<TDtoResponse>> GetAllAsync(int pageSize = 10, int pageNumber = 1, string includeProperties = null, Expression<Func<T, bool>> filter = null)
        {
            var repository = unitOfWork.GetGenericRepository<T>();
            var entities = await repository.GetAllAsync(pageSize, pageNumber, includeProperties, filter);
            return mapper.Map<IEnumerable<TDtoResponse>>(entities);
        }

        public virtual async Task<TDtoResponse> GetByIdAsync(string id, string includeProperties = null)
        {
            var repository = unitOfWork.GetGenericRepository<T>();
            var entity = await repository.GetByIdAsync(id, includeProperties);
            return mapper.Map<TDtoResponse>(entity);
        }

        public virtual async Task<TDtoResponse?> UpdateAsync(string id, TDtoRequest dto)
        {
            var repository = unitOfWork.GetGenericRepository<T>();
            var existingEntity = await repository.GetByIdAsync(id);

            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Server with ID {id} not found.");
            }

            mapper.Map(dto, existingEntity);

            repository.Update(existingEntity);

            var success = await unitOfWork.CompleteAsync() > 0;
            if (!success)
            {
                throw new Exception("Update failed due to an unexpected issue.");
            }

            var updatedEntity = await repository.GetByIdAsync(id);
            return mapper.Map<TDtoResponse>(updatedEntity);
        }

    }
}
