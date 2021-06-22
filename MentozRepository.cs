using Autofac;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mentoz.AspNet.Api
{
    public class MentozRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly MentozDbContext _context;
        public MentozRepository(ITransaction transaction) => _context = transaction as MentozDbContext;// inject IWorkUnit -> EFDbContext
        public IQueryable<TEntity> Entities => _context.Set<TEntity>().Where(QueryFilter<TEntity>());
        public async Task<TEntity> FindAsync(params object[] keyValues)
        {
            var find = await _context.Set<TEntity>().FindAsync(keyValues);
            if (find is null) return null;
            if (find is Entity entity)
                return entity.DeleteTime == DateTime.MinValue ? find : null;
            return find;
        }
        public Task InsertAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            return Task.CompletedTask;
        }
        public Task InsertAsync(List<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(TEntity entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _context.Set<TEntity>().Attach(entity);
                entry.State = EntityState.Modified;
            }
            return Task.CompletedTask;
        }
        public Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(List<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
            return Task.CompletedTask;
        }
        private static Expression<Func<T, bool>> QueryFilter<T>() where T : class
        {
            if (!typeof(Entity).IsAssignableFrom(typeof(T)))
                return x => true;
            var parameter = Expression.Parameter(typeof(T));
            var equal = Expression.Equal(Expression.Property(parameter, "DeleteTime"), Expression.Constant(DateTime.MinValue));
            return Expression.Lambda<Func<T, bool>>(equal, parameter);
        }
    }
}