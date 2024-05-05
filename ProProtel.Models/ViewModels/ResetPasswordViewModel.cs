using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPortel.Models.VModels
{
    public class ResetPasswordViewModel
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? Password { get; set; }
    }

}
