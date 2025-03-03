using System.Data.SqlTypes;

namespace Ecommerce.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SqlMoney Price { get; set; }
        public string Description { get; set; }
        public string DescriptionShort { get; set; }
        public int Category { get; set; }
        public string Img { get; set; }

    }
}
