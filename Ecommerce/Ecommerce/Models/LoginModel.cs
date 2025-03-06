using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class LoginModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "l'username è obbligatorio")]
        public string? Username { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "la password è obbligatoria")]
        public string? Password { get; set; }
    }
}
