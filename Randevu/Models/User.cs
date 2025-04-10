using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentSystem.Models
{
    public enum UserRole { Admin, Doctor, Patient }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(11)]
        public string TcNo { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }
    }
}
