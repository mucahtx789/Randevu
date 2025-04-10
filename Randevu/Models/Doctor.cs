using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentSystem.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required, MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string ExperienceLevel { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public List<DateTime> LeaveDays { get; set; } = new();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
