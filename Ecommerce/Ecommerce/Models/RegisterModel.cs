using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class RegisterModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Username")]
        [Required(ErrorMessage = "l'username è obbligatorio")]
        public string? Username { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "la password è obbligatoria")]
        public string? Password { get; set; }
        [Display(Name = "Conferma Password")]
        [Required(ErrorMessage = "la conferma della password è obbligatoria")]
        public string? ConfirmPassword { get; set; }
        public bool IsLogged {  get; set; }
    }
}
