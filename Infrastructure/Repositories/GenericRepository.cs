using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext appDbContext;

        public GenericRepository(AppDbContext appDbContext )
        {
            this.appDbContext = appDbContext;
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await appDbContext.Set<T>().AddAsync(entity);
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await appDbContext.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found");
            }
            appDbContext.Set<T>().Remove(entity);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
    int pageSize = 10,
    int pageNumber = 1,
    string includeProperties = null,
    Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = appDbContext.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()))
                {
                    query = query.Include(includeProperty);
                }
            }

            query = query.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"));

            pageSize = pageSize > 0 ? Math.Min(pageSize, 100) : 10;
            pageNumber = pageNumber > 0 ? pageNumber : 1;

            query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id, string includeProperties = null)
        {
            IQueryable<T> query = appDbContext.Set<T>();

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);
        }

        public void Update(T entity)
        {
            var dbSet = appDbContext.Set<T>();
            dbSet.Attach(entity);
            appDbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
   