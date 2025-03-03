using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace Ecommerce.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly string _connectionString;
        public AdminController(ILogger<HomeController> logger)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<IActionResult> AdminPage()
        {
            var productsList = new AdminProductsViewModel()
            {
                AdminProducts = new List<AdminProduct>()
            };

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT PRODUCTS.Id, PRODUCTS.Name, Price, DescriptionShort, IdCategory, CATEGORIES.Name FROM PRODUCTS INNER JOIN CATEGORIES ON IdCategory = CATEGORIES.Id ORDER BY IdCategory;";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productsList.AdminProducts.Add(
                                new AdminProduct()
                                {
                                    Id = reader.GetGuid(0),
                                    Name = reader.GetString(1),
                                    Price = reader.GetSqlMoney(2),
                                    DescriptionShort = reader.GetString(3),
                                    IdCategory = reader.GetInt32(4),
                                    CategoryName = reader.GetString(5),
                                }
                            );
                        }
                    }
                }

                return View(productsList);
            }
        }

        [HttpGet("Admin/AdminPage/EditPage/{id:guid}")]
        public async Task<IActionResult> EditPage(Guid id)
        {
            var editProduct = new EditProduct();

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM PRODUCTS WHERE Id = @Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            editProduct.Id = reader.GetGuid(0);
                            editProduct.Name = reader.GetString(1);
                            editProduct.Price = reader.GetDecimal(2);
                            editProduct.Description = reader.GetString(3);
                            editProduct.DescriptionShort = reader.GetString(4);
                            editProduct.Img = reader.GetString(5);
                            if (!reader.IsDBNull(6) || !reader.IsDBNull(7))
                            {
                                editProduct.Img2 = reader.GetString(6);
                                editProduct.Img3 = reader.GetString(7);
                            }
                            editProduct.IdCategory = reader.GetInt32(8);
                        }
                    }
                }
            }
            return View(editProduct);
        }

        [HttpPost("Admin/AdminPage/EditPage/SaveEdit/{id:guid}")]
        public async Task<IActionResult> SaveEdit(Guid id, EditProduct editProduct)
        {
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query;
                if (editProduct.Img2 != null && editProduct.Img3 != null)
                {
                    query = $"UPDATE PRODUCTS SET Name=@Name, Price=@Price, Description=@Description, DescriptionShort=@DescriptionShort, Img=@Img, Img2=@Img2, Img3=@Img3, IdCategory=@IdCategory WHERE Id=@Id";
                    await using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", editProduct.Name);
                        command.Parameters.AddWithValue("@Price", editProduct.Price);
                        command.Parameters.AddWithValue("@Description", editProduct.Description);
                        command.Parameters.AddWithValue("@DescriptionShort", editProduct.DescriptionShort);
                        command.Parameters.AddWithValue("@Img", editProduct.Img);
                        command.Parameters.AddWithValue("@Img2", editProduct.Img2);
                        command.Parameters.AddWithValue("@Img3", editProduct.Img3);
                        command.Parameters.AddWithValue("@IdCategory", editProduct.IdCategory);

                        int righeInteressate = await command.ExecuteNonQueryAsync();
                    }
                }
                else if (editProduct.Img2 != null)
                {
                    query = $"UPDATE PRODUCTS SET Name=@Name, Price=@Price, Description=@Description, DescriptionShort=@DescriptionShort, Img=@Img, Img2=@Img2, IdCategory=@IdCategory WHERE Id=@Id";
                    await using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", editProduct.Name);
                        command.Parameters.AddWithValue("@Price", editProduct.Price);
                        command.Parameters.AddWithValue("@Description", editProduct.Description);
                        command.Parameters.AddWithValue("@DescriptionShort", editProduct.DescriptionShort);
                        command.Parameters.AddWithValue("@Img", editProduct.Img);
                        command.Parameters.AddWithValue("@Img2", editProduct.Img2);
                        command.Parameters.AddWithValue("@IdCategory", editProduct.IdCategory);

                        int righeInteressate = await command.ExecuteNonQueryAsync();
                    }
                }
                else if (editProduct.Img3 != null)
                {
                    query = $"UPDATE PRODUCTS SET Name=@Name, Price=@Price, Description=@Description, DescriptionShort=@DescriptionShort, Img=@Img, Img3=@Img3, IdCategory=@IdCategory WHERE Id=@Id";
                    await using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", editProduct.Name);
                        command.Parameters.AddWithValue("@Price", editProduct.Price);
                        command.Parameters.AddWithValue("@Description", editProduct.Description);
                        command.Parameters.AddWithValue("@DescriptionShort", editProduct.DescriptionShort);
                        command.Parameters.AddWithValue("@Img", editProduct.Img);
                        command.Parameters.AddWithValue("@Img3", editProduct.Img3);
                        command.Parameters.AddWithValue("@IdCategory", editProduct.IdCategory);

                        int righeInteressate = await command.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    query = $"UPDATE PRODUCTS SET Name=@Name, Price=@Price, Description=@Description, DescriptionShort=@DescriptionShort, Img=@Img, IdCategory=@IdCategory WHERE Id=@Id";
                    await using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", editProduct.Name);
                        command.Parameters.AddWithValue("@Price", editProduct.Price);
                        command.Parameters.AddWithValue("@Description", editProduct.Description);
                        command.Parameters.AddWithValue("@DescriptionShort", editProduct.DescriptionShort);
                        command.Parameters.AddWithValue("@Img", editProduct.Img);
                        command.Parameters.AddWithValue("@IdCategory", editProduct.IdCategory);

                        int righeInteressate = await command.ExecuteNonQueryAsync();
                    }
                }
                
            }
            return RedirectToAction("AdminPage");
        }
    }
}
