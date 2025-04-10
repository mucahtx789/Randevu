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
            // Eğer gelen veri eksikse, 400 hatası döndür
            if (string.IsNullOrEmpty(request.TcNo) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "TC Kimlik No ve Şifre zorunludur." });
            }

            // Kullanıcıyı TC Kimlik No ile arıyoruz
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.TcNo == request.TcNo);

            // Eğer kullanıcı bulunamadıysa, 401 Unauthorized döndür
            if (user == null)
            {
                return Unauthorized(new { message = "TC Kimlik No veya Şifre Hatalı" });
            }

            // Şifreyi kontrol et
            if (user.PasswordHash != request.Password)
            {
                return Unauthorized(new { message = "TC Kimlik No veya Şifre Hatalı" });
            }

            // Burada gelen role sabit olarak 'Admin' kabul edilecek
            var role = "Admin"; // Role'ü sabit olarak 'Admin' yapıyoruz

            // JWT token oluştur
            var token = GenerateJwtToken(user.Id, role);

            // Başarılı girişte token ve rolü döndür
            return Ok(new { token, role });
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
    }
}
