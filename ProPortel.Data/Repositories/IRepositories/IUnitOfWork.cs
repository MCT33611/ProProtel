
using ProPortel.Data.Repositories.IRepositories;
namespace ProPortel.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        public IUserRepository user { get; set; }

        public IPlanRepository plan { get; set; }
        public ISubscriptionRepository subscription { get; set; }
        Task<int> Save();
    }
}
