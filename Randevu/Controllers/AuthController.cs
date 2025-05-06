using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppointmentSystem.Data;
using AppointmentSystem.Models;
using Microsoft.AspNetCore.Identity.Data;
using Randevu.Models.Dtos;

namespace AppointmentSystem.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Giriş İşlemi
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var recaptchaValid = await VerifyRecaptchaAsync(request.recaptchaToken);
            if (!recaptchaValid)
                return BadRequest(new { message = "reCAPTCHA doğrulaması başarısız." });


            // var passwordhash = BCrypt.Net.BCrypt.HashPassword("123"); manuel admin kullanıcısı oluştururken sql girilcek şifre
            if (string.IsNullOrEmpty(request.TcNo) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Role))
            {
                return BadRequest(new { message = "TC Kimlik No, Şifre ve Rol alanları zorunludur." });
            }

            var validRoles = new[] { "Admin", "Patient", "Doctor" };
            if (!validRoles.Contains(request.Role))
            {
                return BadRequest(new { message = "Geçersiz rol!" });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.TcNo == request.TcNo);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
              
                
                return Unauthorized(new { message = "TC Kimlik No veya Şifre Hatalı" });
            }

            var token = GenerateJwtToken(user.Id, request.Role);

            // Rol bazlı kimlikleri de al
            int? doctorId = null;
            int? patientId = null;

            if (request.Role == "Doctor")
            {
                doctorId = await _context.Doctors
                    .Where(d => d.UserId == user.Id)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();
            }
            else if (request.Role == "Patient")
            {
                patientId = await _context.Patients
                    .Where(p => p.UserId == user.Id)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();
            }

            return Ok(new
            {
                token,
                role = request.Role,
                userId = user.Id,
                doctorId,
                patientId
            });
        }




        // JWT Token oluşturma işlemi
        private string GenerateJwtToken(int userId, string role)
        {
           
            var jwtKey = _config["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException("Jwt:Key", "JWT Anahtarı 'Jwt:Key' yapılandırma dosyasından alınamıyor.");
            }
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //recaptcha
        private async Task<bool> VerifyRecaptchaAsync(string recaptchaToken)
        {
            var secretKey = _config["Recaptcha:SecretKey"];
            using var httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("secret", secretKey),
        new KeyValuePair<string, string>("response", recaptchaToken)
    });

            var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var recaptchaResult = System.Text.Json.JsonSerializer.Deserialize<RecaptchaResponse>(responseString);

            return recaptchaResult?.success ?? false;
        }
    }
}
