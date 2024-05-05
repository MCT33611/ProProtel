using ProPortel.Data;
using ProPortel.Repositories.IRepositories;
using ProPortel.Data.Repositories.IRepositories;
using ProPortel.Data.Repositories;

namespace ProPortel.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public IUserRepository user { get; set; }

        public IPlanRepository plan { get; set; }
        public ISubscriptionRepository subscription { get; set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            user = new UserRepository(_db);
            plan = new PlanRepository(_db);
            subscription = new SubscriptionRepository(_db);
        }


        public async Task<int> Save()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
