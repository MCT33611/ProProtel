using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPortel.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? PaymentId { get; set; }
        [Required]
        public string? Status { get; set; }
        [Required]
        public string? WorkerId { get; set; }
        [Required]
        public int? PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
