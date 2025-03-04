using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Ecommerce.Models;

namespace Ecommerce.Controllers
{
    public class DetailsController : Controller
    {
        private readonly string _connectionString;

        public DetailsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("details/{id:guid}")]
        public async Task<IActionResult> Index(Guid id)
        {
            Product product = null;

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, Name, Price, Description, Img FROM PRODUCTS WHERE Id = @Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new Product
                            {
                                Id = reader.GetGuid(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Description = reader.GetString(3),
                                Img = reader.GetString(4)
                            };
                        }
                    }
                }
            }

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost("details/add-to-cart/{id:guid}")]
        public async Task<IActionResult> AddToCart(Guid id)
        {
            int categoryId = 0;

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string getCategoryQuery = "SELECT IdCategory FROM PRODUCTS WHERE Id = @Id";
                await using (SqlCommand categoryCommand = new SqlCommand(getCategoryQuery, connection))
                {
                    categoryCommand.Parameters.AddWithValue("@Id", id);
                    var result = await categoryCommand.ExecuteScalarAsync();

                    if (result != null)
                    {
                        categoryId = Convert.ToInt32(result);
                    }
                }

                //funzione che controlla se il product e gia nel cart
                string checkQuery = "SELECT Quantity FROM CART WHERE IdProduct = @IdProduct";
                await using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@IdProduct", id);
                    var existingQuantity = await checkCommand.ExecuteScalarAsync();

                    if (existingQuantity != null)
                    {
                        int newQuantity = Convert.ToInt32(existingQuantity) + 1;
                        string updateQuery = "UPDATE CART SET Quantity = @Quantity WHERE IdProduct = @IdProduct";

                        await using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@Quantity", newQuantity);
                            updateCommand.Parameters.AddWithValue("@IdProduct", id);
                            await updateCommand.ExecuteNonQueryAsync();
                        }
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO CART (Id, Quantity, IdProduct) VALUES (@Id, @Quantity, @IdProduct)";

                        await using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@Id", Guid.NewGuid());
                            insertCommand.Parameters.AddWithValue("@Quantity", 1);
                            insertCommand.Parameters.AddWithValue("@IdProduct", id);
                            await insertCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }

            return RedirectToAction("PrintProducts", "Home", new { id = categoryId });
        }


    }
}
