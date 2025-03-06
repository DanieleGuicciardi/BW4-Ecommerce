using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Display(Name = "Nome Categoria")]
        [Required(ErrorMessage = "il nome è obbligatorio")]
        public string? Name { get; set; }
        [Display(Name = "Immagine Categoria")]
        [Required(ErrorMessage = "l'immagine è obbligatoria")]
        [Url(ErrorMessage = "Errore: inserisci un URL valido")]
        public string? Img { get; set; }
    }
}
