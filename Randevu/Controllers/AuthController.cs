using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppointmentSystem.Data;
using AppointmentSystem.Models;
using Microsoft.AspNetCore.Identity.Data;

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
            if (string.IsNullOrEmpty(request.TcNo) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Role))
            {
                return BadRequest(new { message = "TC Kimlik No, Şifre ve Rol alanları zorunludur." });
            }

            // Sadece belirli roller kabul edilecek
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

            return Ok(new { token, role = request.Role });
        }



        // JWT Token oluşturma işlemi
        private string GenerateJwtToken(int userId, string role)
        {
            if (string.IsNullOrEmpty(role)) // role parametresi kontrol ediliyor
            {
                role = "Admin";  // Eğer null veya boşsa, Admin olarak ayarlanıyor
            }
            var jwtKey = _config["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException("Jwt:Key", "JWT Anahtarı 'Jwt:Key' yapılandırma dosyasından alınamıyor. böyle kodun aq");
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
    }

    // Kullanıcı giriş istekleri için model
    public class LoginRequest
    {
        public string TcNo { get; set; }
        public string Password { get; set; }

        public string Role { get; set; }
    }
}
