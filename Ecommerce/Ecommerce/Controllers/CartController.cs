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

        public async Task<Object> Banner()
        {
            int quantita = 0;

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query2 = @"SELECT SUM(Quantity) FROM CART";

                await using (SqlCommand command = new SqlCommand(query2, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                quantita = reader.GetInt32(0);
                            }
                            else
                            {
                                quantita = 0;
                            }

                        }
                    };
                }
            }
            return TempData["TotQuantita"] = quantita;
        }

        public async Task<IActionResult> Index()
        {
            int quantita = 0;
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

            await Banner();
            return View(cartViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid cartId)
        {
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM CART WHERE Id = @CartId";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CartId", cartId);
                    await command.ExecuteNonQueryAsync();
                }
            }

            await Banner();
            return RedirectToAction("Index");
        }
    }
}
