using ProPortel.Models;
using ProPortel.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPortel.Data.Repositories.IRepositories
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        public void Update(Subscription subscription);
    }
}
