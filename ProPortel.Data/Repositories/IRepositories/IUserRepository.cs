using ProPortel.Models;

namespace ProPortel.Repositories.IRepositories
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser applicationUser);
        Task ChangeUserRole(string userId, string newRoleName);
    }
}
