using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProPortel.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public string? ProfilePicture { get; set; }

        public string? StreetAdderss { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public string?  Role { get; set; }
       

        public string? Bio { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsActive{ get; set; }
        public bool IsAproved{ get; set; }


    }
}
