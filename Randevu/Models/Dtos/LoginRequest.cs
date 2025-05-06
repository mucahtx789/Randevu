namespace AppointmentSystem.Controllers
{
    // Kullanıcı giriş istekleri için model
    public class LoginRequest
    {
        public string TcNo { get; set; }
        public string Password { get; set; }

        public string Role { get; set; }
        public string recaptchaToken { get; set; }
    }
}
