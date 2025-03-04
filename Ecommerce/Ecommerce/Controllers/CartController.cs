using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Ecommerce.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CartController : Controller
    {
        private readonly string _connectionString;

        public CartController()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IActionResult> Index()
        {
            var cartViewModel = new CartViewModel()
            {
                CartItems = new List<CartItem>()
            };

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT C.Id AS CartId, C.Quantity, P.Id AS ProductId, 
                                 P.Name AS ProductName, P.Price, P.Img 
                                 FROM CART C 
                                 INNER JOIN PRODUCTS P ON C.IdProduct = P.Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cartViewModel.CartItems.Add(new CartItem()
                            {
                                CartId = reader.GetGuid(0),
                                Quantity = reader.GetInt32(1),
                                Product = new Product()
                                {
                                    Id = reader.GetGuid(2),
                                    Name = reader.GetString(3),
                                    Price = reader.GetDecimal(4),
                                    Img = reader.GetString(5)
                                }
                            });
                        }
                    }
                }
            }
            return View(cartViewModel);
        }
    }
}
