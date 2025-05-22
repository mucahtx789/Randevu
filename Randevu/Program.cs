using AppointmentSystem.Data;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace Randevu
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Kestrel yapýlandýrmasý (backend portu)
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5229); // Vue frontend'in eriþeceði backend portu
            });
           
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // DbContext'i servis olarak ekliyoruz
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllers();

            // CORS ayarlarý
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueApp", policy =>
                {
                    policy.WithOrigins("http://localhost:55007") //  Vue app adresi
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // JWT, cookie vs. izin ver
                });
            });

            // JWT Authentication yapýlandýrmasý
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // Geliþtirme ortamýnda HTTPS gerekmez
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Issuer'ý konfigürasyon dosyasýndan alýyoruz
                        ValidAudience = builder.Configuration["Jwt:Audience"],  // Audience'ý konfigürasyon dosyasýndan alýyoruz
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))  // Anahtarýnýzý konfigürasyon dosyasýndan alýyoruz
                    };
                });
            // Authorization yapýlandýrmasý
            builder.Services.AddAuthorization();

            // MemoryCache ve Rate Limiting servisleri  API Rate Limiting (API Ýstek Sýnýrlamasý)
            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();


            var app = builder.Build();

            app.UseCors("AllowVueApp");

            // Rate Limiting middleware'i ekle
            app.UseIpRateLimiting();

            app.UseHttpsRedirection();


            // Authentication ve Authorization middleware'lerini doðru sýrayla ekliyoruz
            app.UseAuthentication();  // Öncelikle Authentication
            app.UseAuthorization();   // Sonrasýnda Authorization


            app.MapControllers();

            app.Run();
        }
    }
}
