using ProPortel.Data.Repositories.IRepositories;
using ProPortel.Models;
using ProPortel.Repositories;

namespace ProPortel.Data.Repositories
{
    public class SubscriptionRepository(ApplicationDbContext db) : Repository<Subscription>(db), ISubscriptionRepository
    {
        private readonly ApplicationDbContext _db = db;

        public void Update(Subscription subscription)
        {
            _db.Subscriptions.Update(subscription);
        }
    }
}
