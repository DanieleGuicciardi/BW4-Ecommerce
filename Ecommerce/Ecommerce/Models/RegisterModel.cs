using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class RegisterModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Username")]
        [Required(ErrorMessage = "l'username è obbligatorio")]
        [StringLength(20, ErrorMessage = "Il nome deve essere compreso tra 4 e 20 caratteri.", MinimumLength = 4)]
        public string? Username { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "la password è obbligatoria")]
        [StringLength(20, ErrorMessage = "La password deve essere compreso tra 6 e 20 caratteri.", MinimumLength = 6)]
        public string? Password { get; set; }
        [Display(Name = "Conferma Password")]
        [Required(ErrorMessage = "la conferma della password è obbligatoria")]
        public string? ConfirmPassword { get; set; }
        public bool IsLogged {  get; set; }
    }
}
