using System.Data.SqlTypes;

namespace Ecommerce.Models
{
    public class AdminProduct
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SqlMoney Price { get; set; }
        public string DescriptionShort { get; set; }
        public int IdCategory { get; set; }
        public string CategoryName { get; set; }
    }
}
