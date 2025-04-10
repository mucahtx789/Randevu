using Microsoft.AspNetCore.Mvc;
using AppointmentSystem.Data;
using AppointmentSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> CreateDoctor([FromBody] Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return Ok(doctor);
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

    public class DoctorLeaveRequest
    {
        public int DoctorId { get; set; }
        public DateTime LeaveDate { get; set; }
    }
}
