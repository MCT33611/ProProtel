using ProPortel.Data.Repositories.IRepositories;
using ProPortel.Models;
using ProPortel.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPortel.Data.Repositories
{
    public class PlanRepository(ApplicationDbContext db) : Repository<Plan>(db),IPlanRepository
    {
        
        public void Update(Plan plan)
        {
            db.Update(plan);
        }
    }
}
