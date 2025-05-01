using Microsoft.AspNetCore.Mvc;
using AppointmentSystem.Data;
using AppointmentSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Randevu.Models.Dtos;
namespace AppointmentSystem.Controllers
{
    [Route("api/admin/doctors")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Doktor kaydı oluşturma
        [HttpPost("create")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorRequest model)
        {
            // Önce kullanıcıyı oluştur
            var newUser = new User
            {
                TcNo = model.TcNo,
                FullName = $"{model.FirstName} {model.LastName}",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = UserRole.Doctor
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(); // UserId alınacak

            // Doktoru oluştur
            var doctor = new Doctor
            {
                UserId = newUser.Id,
                Specialization = model.Specialization,
                ExperienceLevel = model.ExperienceLevel,
                IsAvailable = true,
                
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Doktor başarıyla oluşturuldu." });
        }

        // Doktorları listeleme
        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Select(d => new DoctorDtoGet
                {
                    Id = d.Id,
                    FullName = d.User.FullName,
                    Specialization = d.Specialization,
                    ExperienceLevel = d.ExperienceLevel,
                    LeaveDays = d.LeaveDays
                })
                .ToListAsync();

            if (!doctors.Any())
            {
                return NotFound("Hiç doktor bulunamadı");
            }

            return Ok(doctors);
        }

        // Doktorun izinlerini ekleme
        [HttpPost("add-leave")]
        public async Task<IActionResult> AddLeave([FromBody] DoctorLeaveRequest leaveRequest)
        {
            var doctor = await _context.Doctors.FindAsync(leaveRequest.DoctorId);
            if (doctor == null)
            {
                return NotFound("Doktor bulunamadı");
            }

            // Mevcut izin günlerini al (List<DateTime>)
            var leaveDays = doctor.LeaveDays;

            // Aynı tarihte izin olup olmadığını kontrol et
            if (leaveDays.Contains(leaveRequest.LeaveDate))
            {
                return BadRequest("Bu tarihte zaten izin var.");
            }

            // Yeni izni ekle
            doctor.LeaveDays.Add(leaveRequest.LeaveDate);
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();

            return Ok("İzin eklenmiştir");
        }

        // Doktordan izin kaldırma
        [HttpDelete("remove-leave")]
        public async Task<IActionResult> RemoveLeave([FromBody] DoctorLeaveRequest leaveRequest)
        {
            var doctor = await _context.Doctors.FindAsync(leaveRequest.DoctorId);
            if (doctor == null)
            {
                return NotFound("Doktor bulunamadı");
            }

            // Mevcut izinleri al
            var leaveDays = doctor.LeaveDays;

            // Eğer kaldırılacak tarih yoksa hata döndür
            if (!leaveDays.Contains(leaveRequest.LeaveDate))
            {
                return BadRequest("Kaldırılacak izin bulunamadı.");
            }

            // İzinleri güncelle
            doctor.LeaveDays.Remove(leaveRequest.LeaveDate);
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();

            return Ok("İzin kaldırılmıştır");
        }

        // Doktorun izinlerini listeleme
        [HttpGet("leaves/{doctorId}")]
        public async Task<IActionResult> GetDoctorLeaves(int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
            {
                return NotFound("Doktor bulunamadı");
            }

            // LeaveDays listesini döndür
            return Ok(doctor.LeaveDays);
        }
    }

    
}
