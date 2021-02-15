using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure
{
    public abstract class RepositoryBase<TModel, TDbContext> : IRepositoryBase<TModel> where TModel : class, new() where TDbContext : DbContext, new()
    {
        private bool _disposed;
        protected TDbContext RepositoryContext { get; set; }
        protected RepositoryBase(TDbContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    RepositoryContext.Dispose();
                }
            }
            _disposed = true;
        }
        public IQueryable<TModel> FindAll()
        {
            return RepositoryContext.Set<TModel>().AsNoTracking();
        }
        public IQueryable<TModel> FindByCondition(Expression<Func<TModel, bool>> predicate = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null, Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>> include = null,
            bool disableTracking = true)
        {
            var query = RepositoryContext.Set<TModel>().Where(predicate);
            if (include != null)
            {
                query = include(query);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return disableTracking ? query.AsNoTracking().AsQueryable() : query.AsQueryable();
        }
        public async Task<TModel> FindItemByCondition(Expression<Func<TModel, bool>> predicate = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null, Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>> include = null,
            bool disableTracking = true)
        {
            var query = RepositoryContext.Set<TModel>().Where(predicate);
            if (include != null)
            {
                query = include(query);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (disableTracking)
            {
                return await query.AsNoTracking().FirstOrDefaultAsync();
            }
            return await query.FirstOrDefaultAsync();
        }
        public async Task<bool> Insert(TModel entity)
        {
            await RepositoryContext.Set<TModel>().AddAsync(entity);
            return true;
        }
        public async Task<bool> InsertRange(List<TModel> entity)
        {
            await RepositoryContext.Set<TModel>().AddRangeAsync(entity);
            return true;
        }
        public async Task<bool> Update(TModel entity)
        {
            RepositoryContext.Set<TModel>().Update(entity);
            return true;
        }
        public async Task<int> Commit()
        {
            return await RepositoryContext.SaveChangesAsync();
        }
        public async Task<bool> Delete(TModel entity)
        {
            RepositoryContext.Set<TModel>().Remove(entity);
            return true;
        }
        public async Task<bool> Rollback()
        {
            RepositoryContext.ChangeTracker.Entries().ToList().ForEach(async x => await x.ReloadAsync());
            return true;
        }
    }
}