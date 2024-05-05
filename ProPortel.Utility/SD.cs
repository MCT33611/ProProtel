using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProPortel.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProPortel.Utility
{
    public static class SD
    {

        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Worker = "Worker";

        public const string Payment_Failed = "Failed";
        public const string Payment_Success = "Success";

    }
    
}
