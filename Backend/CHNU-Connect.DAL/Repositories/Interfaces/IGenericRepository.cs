using System.Linq.Expressions;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "");

        Task<T?> GetByIdAsync(object id);
        Task InsertAsync(T entity);
        void Delete(T entity);
        Task DeleteByIdAsync(object id);
        void Update(T entity);
        Task SaveAsync();
    }
}
