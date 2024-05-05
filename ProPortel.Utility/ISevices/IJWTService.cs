using ProPortel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPortel.Utility
{
    public interface IJWTService
    {
        public string GenerateJwtToken(ApplicationUser user);
        public string GenerateTokenForAdmin(string username);
    }
}
