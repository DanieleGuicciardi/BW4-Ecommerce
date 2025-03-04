using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Ecommerce.Models
{
    public class EditProduct
    {
        public Guid Id { get; set; }
        [Display(Name = "Nome Prodotto")]
        [Required(ErrorMessage = "il nome è obbligatorio")]
        public string? Name { get; set; }
        [Display(Name = "Prezzo")]
        [Required(ErrorMessage = "il prezzo è obbligatorio")]
        public decimal Price { get; set; }
        [Display(Name = "Descrizione")]
        [Required(ErrorMessage = "la descrizione è obbligatoria")]
        public string? Description { get; set; }
        [Display(Name = "Descrizione corta")]
        [Required(ErrorMessage = "la descrizione corta è obbligatoria")]
        public string? DescriptionShort { get; set; }
        [Display(Name = "Immagine Principale")]
        [Required(ErrorMessage = "l'immagine principale è obbligatoria")]
        public string? Img { get; set; }
        [Display(Name = "Immagine 2")]
        public string? Img2 { get; set; }
        [Display(Name = "Immagine 3")]
        public string? Img3 { get; set; }
        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "la categoria è obbligatoria")]
        public int IdCategory { get; set; }
    }
}
