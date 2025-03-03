using System.Diagnostics;
using Microsoft.Data.SqlClient;
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

        public async Task<IActionResult> Index()
        {
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

            return View(categoryList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
    }
}
