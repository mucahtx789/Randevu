namespace Randevu.Models.Dtos
{
    public class DoctorLeaveRequest
    {
        public int DoctorId { get; set; }
        public DateTime LeaveDate { get; set; }
    }

    public class CreateDoctorRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TcNo { get; set; }
        public string Password { get; set; }

        public string Specialization { get; set; }
        public string ExperienceLevel { get; set; }

    }

    public class DoctorDtoGet
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string ExperienceLevel { get; set; }
        public List<DateTime> LeaveDays { get; set; }
    }
}
