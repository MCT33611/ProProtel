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
        string GenerateJwtToken(ApplicationUser user);
        string GenerateJwtTokenForWorker(ApplicationUser user);
        string GenerateTokenForAdmin(ApplicationUser user);
    }
}
