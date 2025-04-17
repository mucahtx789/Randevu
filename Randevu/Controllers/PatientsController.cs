using AppointmentSystem.Data;
using AppointmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(PatientRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(x => x.TcNo == dto.TcNo))
            return BadRequest("Bu TC numarası ile kayıtlı bir kullanıcı zaten var.");

        var user = new User
        {
            TcNo = dto.TcNo,
            FullName = dto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Patient
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var patient = new Patient
        {
            UserId = user.Id,
            BirthDate = dto.BirthDate
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return Ok("Kayıt başarılı.");
    }
}
public class PatientRegisterDto
{
    public string TcNo { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public DateTime BirthDate { get; set; }
}
