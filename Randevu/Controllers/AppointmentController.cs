using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentSystem.Data;
using AppointmentSystem.Models;
using System.Globalization;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AppointmentSystem.Models.Dtos;

namespace AppointmentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return NotFound("Hasta kaydı bulunamadı.");

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.PatientId == patient.Id)
                .OrderByDescending(a => a.AppointmentTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    AppointmentTime = a.AppointmentTime,
                    DoctorFullName = a.Doctor.User.FullName,
                    Specialization = a.Doctor.Specialization,
                    ExperienceLevel = a.Doctor.ExperienceLevel
                })
                .ToListAsync();

            return Ok(appointments);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto dto)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId);

            if (doctor == null)
                return NotFound("Doktor bulunamadı.");

            var appointmentDate = dto.AppointmentTime.Date;

            if (doctor.LeaveDays.Contains(appointmentDate))
                return BadRequest("Bu tarihte doktor izinli.");

            if (appointmentDate.DayOfWeek == DayOfWeek.Saturday || appointmentDate.DayOfWeek == DayOfWeek.Sunday)
                return BadRequest("Hafta sonuna randevu alınamaz.");

            var time = dto.AppointmentTime.TimeOfDay;
            if (time < TimeSpan.FromHours(8) ||
                (time >= TimeSpan.FromHours(12) && time < TimeSpan.FromHours(13)) ||
                time >= TimeSpan.FromHours(17))
            {
                return BadRequest("Geçersiz randevu saati.");
            }

            var exists = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                a.AppointmentTime == dto.AppointmentTime);

            if (exists)
                return BadRequest("Bu saatte zaten randevu alınmış.");

            // Veritabanına eklenecek model
            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                AppointmentTime = dto.AppointmentTime,
                Status = AppointmentStatus.Scheduled
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Yanıt için DTO'ya bilgileri aktar
            dto.Id = appointment.Id;
            dto.Status = appointment.Status.ToString();

            return Ok(dto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound("Randevu bulunamadı.");

            var timeRemaining = appointment.AppointmentTime - DateTime.Now;
            if (timeRemaining < TimeSpan.FromHours(1))
                return BadRequest("Randevuya 1 saatten az kaldığı için iptal edilemez.");

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Ok("Randevu iptal edildi.");
        }

        [HttpGet("available-times")]
        public async Task<IActionResult> GetAvailableTimes(int doctorId, DateTime date)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
                return NotFound("Doktor bulunamadı.");

            var appointmentDate = date.Date;

            if (doctor.LeaveDays.Contains(appointmentDate))
                return Ok(new { isLeaveDay = true, times = new List<string>() });

            if (appointmentDate.DayOfWeek == DayOfWeek.Saturday || appointmentDate.DayOfWeek == DayOfWeek.Sunday)
                return Ok(new { isWeekend = true, times = new List<string>() });

            var start = TimeSpan.FromHours(8);
            var end = TimeSpan.FromHours(17);
            var breakStart = TimeSpan.FromHours(12);
            var breakEnd = TimeSpan.FromHours(13);
            var step = TimeSpan.FromMinutes(15);

            var takenTimes = doctor.Appointments
                .Where(a => a.AppointmentTime.Date == appointmentDate)
                .Select(a => a.AppointmentTime.TimeOfDay)
                .ToList();

            var available = new List<string>();

            for (var time = start; time < end; time += step)
            {
                if (time >= breakStart && time < breakEnd)
                    continue;

                if (!takenTimes.Contains(time))
                {
                    available.Add(DateTime.Today.Add(time).ToString("HH:mm"));
                }
            }

            return Ok(new { times = available });
        }

        [HttpGet("by-specialization/{specialization}")]
        public IActionResult GetDoctorsBySpecialization(string specialization)
        {
            // Gelen specialization'ı doğru şekilde alıp doktorları filtreliyoruz
            var doctors = (from doctor in _context.Doctors
                           join user in _context.Users on doctor.UserId equals user.Id
                           where doctor.Specialization == specialization
                           select new
                           {
                               DoctorId = doctor.Id,
                               FullName = user.FullName,
                               ExperienceLevel = doctor.ExperienceLevel
                           }).ToList();

            // Eğer doktor bulunamazsa 404 döndürüyoruz
            if (!doctors.Any())
            {
                return NotFound("No doctors found for this specialization.");
            }

            // Doktorları başarıyla döndürüyoruz
            return Ok(doctors);
        }
    }
}
