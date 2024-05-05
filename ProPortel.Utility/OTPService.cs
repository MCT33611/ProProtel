using Microsoft.EntityFrameworkCore;
using ProPortel.Data;
using ProPortel.Models;
using ProPortel.Utility.ISevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPortel.Utility
{
    public class OTPService : IOTPService
    {
        private readonly ApplicationDbContext _context;

        public OTPService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateOTPAsync(string Email)
        {
            string otpCode = GenerateOTP(); // Implement your own OTP generation logic
            DateTime expirationTime = DateTime.Now.AddMinutes(5); // Set expiration time (e.g., 5 minutes from now)

            var otp = new OTP
            {
                Email = Email,
                Code = otpCode,
                ExpirationTime = expirationTime
            };

            _context.OTPs.Add(otp);
            await _context.SaveChangesAsync();

            return otpCode;
        }

        public async Task<bool> ValidateOTPAsync(string email, string otpCode)
        {
            var otp = await _context.OTPs.FirstOrDefaultAsync(o => o.Email == email && o.Code == otpCode && o.ExpirationTime >= DateTime.Now);
            if (otp != null)
            {
                _context.OTPs.Remove(otp); 
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(1000, 9990).ToString();
        }
    }

}
