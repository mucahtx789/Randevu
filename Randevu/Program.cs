using AppointmentSystem.Data;
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


            /* builder.Services.AddDbContext<ApplicationDbContext>(opts =>
             {
                 opts.UseSqlServer("SERVER=DESKTOP-VI5LI79;Database=Randevu;Trusted_Connection=True;TrustServerCertificate=True");

             });*/
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // DbContext'i servis olarak ekliyoruz
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

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

            builder.Services.AddAuthorization();



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            var app = builder.Build();
            app.UseCors("AllowAll");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
