using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Reflection.PortableExecutable;

namespace Ecommerce.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly string _connectionString;
        public HomeController(ILogger<HomeController> logger)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }
        public async Task<Object> Banner()
        {
            int quantita = 0;

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query2 = @"SELECT ISNULL(SUM(Quantity), 0) FROM CART;";

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

                await using(SqlCommand command = new SqlCommand(query, connection))
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
                return TempData["IsLogged"] = true;
            }
            else
            {
                return TempData["IsLogged"] = false;
            }
        }


        public async Task<IActionResult> Index()
        {
            await IsUserLogged();
            var categoryList = new CategoryViewModel()
            {
                Categories = new List<Category>()
            };

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM CATEGORIES";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categoryList.Categories.Add(
                                new Category()
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Img = reader.GetString(2),
                                }
                            );
                        }
                    }
                }
            }
            await Banner();

            return View(categoryList);
        }

        [HttpGet("home/printproducts/{id:int}")]
        public async Task<IActionResult> PrintProducts(Int32 id)
        {
            await IsUserLogged();
            var printProducts = new ProductsViewModel()
            {
                Products = new List<Product>()
            };

            await using (SqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT P.Id, P.Name, P.Price, P.Description, P.DescriptionShort, P.Img, P.Img2, P.Img3, C.Id FROM PRODUCTS P INNER JOIN CATEGORIES C ON P.IdCategory = C.Id WHERE P.IdCategory = @Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string img2;
                            string img3;
                            if (!reader.IsDBNull(6) && !reader.IsDBNull(7))
                            {
                                img2 = reader.GetString(6);
                                img3 = reader.GetString(7);
                            }
                            else
                            {
                                img2 = null;
                                img3 = null;
                            }

                            printProducts.Products.Add(
                                new Product()
                                {
                                    Id = reader.GetGuid(0),
                                    Name = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    Description = reader.GetString(3),
                                    DescriptionShort = reader.GetString(4),
                                    Img = reader.GetString(5),
                                    Img2 = img2,
                                    Img3 = img3,
                                    Category = reader.GetInt32(8),

                                }
                            );

                        }
                    }
                }
            }

            await Banner();
            return View(printProducts);
        }

    }
}
