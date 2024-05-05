using System.Linq.Expressions;

namespace ProPortel.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {

        IEnumerable<T> GetALL(Expression<Func<T, bool>>? filter = null, string? incluedProperties = null);

        T Get(Expression<Func<T, bool>> filter, string? incluedProperties = null, bool tracked = true);

        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
