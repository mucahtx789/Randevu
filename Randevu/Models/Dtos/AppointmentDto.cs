namespace AppointmentSystem.Models.Dtos
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string DoctorFullName { get; set; }
        public string Specialization { get; set; }
        public string ExperienceLevel { get; set; }
    }
}