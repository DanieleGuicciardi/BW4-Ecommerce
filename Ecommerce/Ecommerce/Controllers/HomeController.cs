using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> AdminPage()
        {
            var productsList = new AdminProductsViewModel()
            {
                AdminProducts = new List<AdminProduct>()
            };

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //se non apro la connessione, non mi connetto al db e non posso eseguire query
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
                                    Price = reader.GetDecimal(2),
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
    }
}
