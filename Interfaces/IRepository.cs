using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mentoz.AspNet.Api
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Entities { get; } // DbSet<TEntity>
        Task<TEntity> FindAsync(params object[] keyValues);
        Task InsertAsync(TEntity entity);
        Task InsertAsync(List<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(List<TEntity> entities);
    }
}