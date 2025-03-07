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
        public async Task<Object> Banner()
        {
            int quantita = 0;

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query2 = @"SELECT SUM(Quantity) FROM CART INNER JOIN LOGIN ON CART.Id = LOGIN.Id WHERE LOGIN.IsLogged=1";

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

        public async Task<Object> IsUserLogged()
        {
            int loggedCount = new();
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT COUNT(IsLogged) FROM LOGIN WHERE IsLogged=1";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            loggedCount = reader.GetInt32(0);
                        }
                    }
                }
            }
            if (loggedCount >= 1)
            {
                return ViewData["IsLogged"] = true;
            }
            else
            {
                return ViewData["IsLogged"] = false;
            }
        }

        public DetailsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("details/{id:guid}")]
        public async Task<IActionResult> Index(Guid id, bool toastAttivo)
        {
            await IsUserLogged();
            Product product = new Product();
            int categoryId = 0;

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, Name, Price, Description, Img, Img2, Img3, IdCategory FROM PRODUCTS WHERE Id = @Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string img2 = reader.IsDBNull(5) ? null : reader.GetString(5);
                            string img3 = reader.IsDBNull(6) ? null : reader.GetString(6);
                            categoryId = reader.GetInt32(7);

                            product = new Product
                            {
                                Id = reader.GetGuid(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Description = reader.GetString(3),
                                Img = reader.GetString(4),
                                Img2 = img2,
                                Img3 = img3,
                                Category = categoryId
                            };
                        }
                    }
                }
            }

            TempData["Toast"] = toastAttivo;
            await Banner();
            ViewBag.CategoryId = categoryId; 
            return View(product);
        }

        [HttpPost("details/add-to-cart/{id:guid}")]
        public async Task<IActionResult> AddToCart(Guid id, int quantity)
        {
            if (quantity <= 0) quantity = 1; 

            int categoryId = 0;
            Guid CartId = new Guid();

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string isLoggedQuery = "SELECT Id FROM LOGIN WHERE IsLogged = 1";
                await using (SqlCommand isLoggedCommand = new SqlCommand(isLoggedQuery, connection))
                {
                    await using (SqlDataReader reader = await isLoggedCommand.ExecuteReaderAsync())
                    {
                        if (reader.HasRows) 
                        {
                            while (reader.Read())
                            {
                                CartId = reader.GetGuid(0);
                            }
                        }
                        else
                        {
                            TempData["Login"] = "Devi effettuare il Login per questa operazione";
                            return RedirectToAction("Index", new { id });
                        }
                    }
                }

                string getCategoryQuery = "SELECT IdCategory FROM PRODUCTS WHERE Id = @Id";
                await using (SqlCommand categoryCommand = new SqlCommand(getCategoryQuery, connection))
                {
                    categoryCommand.Parameters.AddWithValue("@Id", id);
                    var result = await categoryCommand.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                    {
                        categoryId = Convert.ToInt32(result);
                    }
                }

                string checkQuery = "SELECT Quantity FROM CART INNER JOIN LOGIN ON CART.Id = LOGIN.Id WHERE IdProduct = @IdProduct AND LOGIN.IsLogged=1";
                await using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@IdProduct", id);
                    var existingQuantity = await checkCommand.ExecuteScalarAsync();

                    if (existingQuantity != null && existingQuantity != DBNull.Value)
                    {
                        int newQuantity = Convert.ToInt32(existingQuantity) + quantity;
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
                            insertCommand.Parameters.AddWithValue("@Id", CartId);
                            insertCommand.Parameters.AddWithValue("@Quantity", quantity);
                            insertCommand.Parameters.AddWithValue("@IdProduct", id);
                            await insertCommand.ExecuteNonQueryAsync();
                        }
                    }
                }

            }

            await Banner();
            var toastAttivo = true;
            return RedirectToAction("Index", new { id, toastAttivo });
        }

    }
}
