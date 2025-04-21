
using System;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.Dtos
{
    public class AppointmentCreateDto
    {
        public int Id { get; set; } // Yanıt için
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string? Status { get; set; } // Yanıt için
    }

}
