using System.Data.SqlTypes;

namespace Ecommerce.Models
{
    public class EditProduct
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? DescriptionShort { get; set; }
        public string? Img { get; set; }
        public string? Img2 { get; set; }
        public string? Img3 { get; set; }

        public int IdCategory { get; set; }
    }
}
