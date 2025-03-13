using System.ComponentModel.DataAnnotations;

namespace PaymentCVSTS.MVCWebApp.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        public string password { get; set; }
    }
}