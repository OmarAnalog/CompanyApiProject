using Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected Context _dbContext;
        public RepositoryBase(Context dbContext)
        {
            _dbContext = dbContext;
        }
        public void Create(T entity) => _dbContext.Set<T>().Add(entity);
        public void Update(T entity) => _dbContext.Set<T>().Update(entity);
        public void Delete(T entity) => _dbContext.Set<T>().Remove(entity);

        public IQueryable<T> FindAll(bool trackChanges = false)
        => trackChanges
            ? _dbContext.Set<T>()
            : _dbContext.Set<T>().AsNoTracking();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
        => trackChanges
            ? _dbContext.Set<T>().Where(expression)
            : _dbContext.Set<T>().Where(expression).AsNoTracking();
    }
}
