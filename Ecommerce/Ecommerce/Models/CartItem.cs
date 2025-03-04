namespace Ecommerce.Models
{
    public class CartItem
    {
        public Guid CartId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }
    }

    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; }
    }
}

