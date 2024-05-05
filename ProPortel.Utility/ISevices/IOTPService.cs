using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPortel.Utility.ISevices
{
    public interface IOTPService
    {
        public  Task<string> GenerateOTPAsync(string userId);

        public Task<bool> ValidateOTPAsync(string userId, string otpCode);

        public string GenerateOTP();
    }
}
