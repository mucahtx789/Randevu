using System.Text.Json.Serialization;

namespace Randevu.Models.Dtos
{
    public class RecaptchaResponse
    {
     
        public bool success { get; set; }

        
        public float score { get; set; }

        public string action { get; set; }

     
        public DateTime challengeTs { get; set; }

     
        public string hostname { get; set; }

   
        public List<string> errorCodes { get; set; }
    }
}
