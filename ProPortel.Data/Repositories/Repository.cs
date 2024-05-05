using Microsoft.EntityFrameworkCore;
using ProPortel.Data;
using ProPortel.Repositories.IRepositories;
using System.Linq.Expressions;
using System.Linq;

namespace ProPortel.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        private static readonly char[] separator = [','];

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
            //_db.Products.Include(u => u.Category).Include(u => u.categoryId);
        }


        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? incluedProperties = null, bool tracked = true)
        {
            IQueryable<T> query;

            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(incluedProperties))
            {
                foreach (var incluedProp in incluedProperties.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluedProp);
                }
            }
            return query.FirstOrDefault()!;
        }

        //Category,ProductType
        public IEnumerable<T> GetALL(Expression<Func<T, bool>>? filter, string? incluedProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(incluedProperties))
            {
                foreach (var incluedProp in incluedProperties.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluedProp);
                }
            }

            return [.. query];
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
