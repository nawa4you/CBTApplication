using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBTApplication.Models
{
    public class PasswordResetRequest
    {


        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; } = string.Empty;

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public bool IsProcessed { get; set; } = false;



        public string? VerificationCode { get; set; }
        public DateTime? CodeExpiry { get; set; }

    }
}
