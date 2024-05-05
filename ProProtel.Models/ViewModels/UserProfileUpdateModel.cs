using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPortel.Models.VModels
{
    public class UserProfileUpdateModel
    {
        public string? FirstName { get; set; }
        

        public string? LastName { get; set; }

        public string? ProfilePicture { get; set; }

        public string? StreetAdderss { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public string? Bio { get; set; }
    }
}
