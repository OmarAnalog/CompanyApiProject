using System.Linq.Expressions;

namespace Contracts
{
    public interface IRepositoryBase<T>  where T : class
    {
        // Define common repository methods here
        // Example: Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        // Example: Task<T> GetByIdAsync<T>(int id) where T : class;
        // Example: Task CreateAsync<T>(T entity) where T : class;
        // Example: Task UpdateAsync<T>(T entity) where T : class;
        // Example: Task DeleteAsync<T>(int id) where T : class;
        IQueryable<T> FindAll(bool trackChanges = false);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
