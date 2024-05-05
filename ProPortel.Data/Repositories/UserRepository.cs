using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProPortel.Data;
using ProPortel.Models;
using ProPortel.Repositories.IRepositories;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace ProPortel.Repositories
{
    public class UserRepository :Repository<ApplicationUser>, IUserRepository
    {
        public readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }

        public void Update(ApplicationUser applicationUser)
        {
            _db.AppUsers.Update(applicationUser);
        }

        public async Task ChangeUserRole(string userId, string newRoleName)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return;
            }

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == newRoleName);

            if (role == null)
            {
                return;
            }

            var userRole = await _db.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);

            if (userRole != null)
            {
                _db.UserRoles.Remove(userRole);
                await _db.SaveChangesAsync();
            }

            _db.UserRoles.Add(new IdentityUserRole<string> { UserId = userId, RoleId = role.Id });
            await _db.SaveChangesAsync();
        }


    }
}

