using System.Reflection.PortableExecutable;
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
                            } else
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

        public async Task<IActionResult> AdminPage()
        {
            await IsUserLogged();

            var productsList = new AdminProductsViewModel()
            {
                AdminProducts = new List<AdminProduct>()
            };

            var categoryList = new CategoryViewModel()
            {
                Categories = await GetCategories()
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
                                    Price = reader.GetDecimal(2),
                                    DescriptionShort = reader.GetString(3),
                                    IdCategory = reader.GetInt32(4),
                                    CategoryName = reader.GetString(5),
                                }
                            );
                        }
                    }
                }

                await Banner();
                ViewBag.CategoryList = categoryList;

                return View(productsList);
            }
        }

        private async Task<List<Category>> GetCategories()
        {
            List<Category> listaCategorie = new List<Category>();
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM CATEGORIES";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            listaCategorie.Add(
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
            return listaCategorie;

        }

        [HttpGet("Admin/AdminPage/EditPage/{id:guid}")]
        public async Task<IActionResult> EditPage(Guid id)
        {
            await IsUserLogged();

            var editProduct = new EditProduct();
            var categoryList = new CategoryViewModel()
            {
                Categories = await GetCategories()
            };

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

            ViewBag.CategoryList = categoryList;
            await Banner();
            return View(editProduct);
        }

        [HttpPost("Admin/AdminPage/EditPage/SaveEdit/{id:guid}")]
        public async Task<IActionResult> SaveEdit(Guid id, EditProduct editProduct)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Errore nel modello del form";
                return RedirectToAction("EditPage", new { id });
            }

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
            await Banner();
            return RedirectToAction("AdminPage");
        }

        [HttpGet("Admin/AdminPage/EditCategoryPage/{id:int}")]
        public async Task<IActionResult> EditCategoryPage(int id)
        {
            await IsUserLogged();

            var editCategory = new Category();

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM CATEGORIES WHERE Id = @Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            editCategory.Id = id;
                            editCategory.Name = reader.GetString(1);
                            editCategory.Img = reader.GetString(2);
                        }
                    }
                }
            }
            await Banner();
            return View(editCategory);
        }

        [HttpPost("Admin/AdminPage/EditCategoryPage/SaveEditCategory/{id:int}")]
        public async Task<IActionResult> EditCategory(int id, Category editCategory)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Errore nel modello del form";
                return RedirectToAction("EditCategoryPage", new { id });
            }

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = $"UPDATE CATEGORIES SET Name=@Name, Img=@Img WHERE Id=@Id";
                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    command.Parameters.AddWithValue("@Name", editCategory.Name);
                    command.Parameters.AddWithValue("@Img", editCategory.Img);

                    int righeInteressate = await command.ExecuteNonQueryAsync();
                }
            }


            await Banner();
            return RedirectToAction("AdminPage");
        }

        public async Task<IActionResult> AddPage()
        {
            await IsUserLogged();

            var model = new AddProductModel()
            {
                Categories = await GetCategories()
            };

            await Banner();
            return View(model);
        }

        public async Task<IActionResult> AddCategoryPage()
        {
            await IsUserLogged();
            await Banner();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category addCategory)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Errore nel modello del form";
                return RedirectToAction("AddCategoryPage");
            }

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = $"INSERT INTO CATEGORIES(Name, Img) VALUES (@Name, @Img)";
                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", addCategory.Name);
                    command.Parameters.AddWithValue("@Img", addCategory.Img);

                    int righeInteressate = await command.ExecuteNonQueryAsync();
                }
            }

            await Banner();
            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductModel addProduct)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Errore nel modello del form";
                return RedirectToAction("AddPage");
            }

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                if (addProduct.Img2 != null && addProduct.Img3 != null)
                {
                    var query = $"INSERT INTO PRODUCTS VALUES (@Id, @Name, @Price, @Description, @DescriptionShort, @Img, @Img2, @Img3, @IdCategory)";
                    await using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        command.Parameters.AddWithValue("@Name", addProduct.Name);
                        command.Parameters.AddWithValue("@Price", addProduct.Price);
                        command.Parameters.AddWithValue("@Description", addProduct.Description);
                        command.Parameters.AddWithValue("@DescriptionShort", addProduct.DescriptionShort);
                        command.Parameters.AddWithValue("@Img", addProduct.Img);
                        command.Parameters.AddWithValue("@Img2", addProduct.Img2);
                        command.Parameters.AddWithValue("@Img3", addProduct.Img3);
                        command.Parameters.AddWithValue("@IdCategory", addProduct.IdCategory);

                        int righeInteressate = await command.ExecuteNonQueryAsync();
                    }
                }
                else if (addProduct.Img2 != null)
                {
                    var query = $"INSERT INTO PRODUCTS(Id, Name, Price, Description, DescriptionShort, Img, Img2, IdCategory) VALUES (@Id, @Name, @Price, @Description, @DescriptionShort, @Img, @Img2, @IdCategory)";
                    await using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        command.Parameters.AddWithValue("@Name", addProduct.Name);
                        command.Parameters.AddWithValue("@Price", addProduct.Price);
                        command.Parameters.AddWithValue("@Description", addProduct.Description);
                        command.Parameters.AddWithValue("@DescriptionShort", addProduct.DescriptionShort);
                        command.Parameters.AddWithValue("@Img", addProduct.Img);
                        command.Parameters.AddWithValue("@Img2", addProduct.Img2);
                        command.Parameters.AddWithValue("@IdCategory", addProduct.IdCategory);

                        int righeInteressate = await command.ExecuteNonQueryAsync();
                    }
                }
                else if (addProduct.Img3 != null)
                {
                    var query = $"INSERT INTO PRODUCTS(Id, Name, Price, Description, DescriptionShort, Img, Img3, IdCategory) VALUES (@Id, @Name, @Price, @Description, @DescriptionShort, @Img, @Img3, @IdCategory)";
                    await using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        command.Parameters.AddWithValue("@Name", addProduct.Name);
                        command.Parameters.AddWithValue("@Price", addProduct.Price);
                        command.Parameters.AddWithValue("@Description", addProduct.Description);
                        command.Parameters.AddWithValue("@DescriptionShort", addProduct.DescriptionShort);
                        command.Parameters.AddWithValue("@Img", addProduct.Img);
                        command.Parameters.AddWithValue("@Img3", addProduct.Img3);
                        command.Parameters.AddWithValue("@IdCategory", addProduct.IdCategory);

                        int righeInteressate = await command.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    var query = $"INSERT INTO PRODUCTS(Id, Name, Price, Description, DescriptionShort, Img, IdCategory) VALUES (@Id, @Name, @Price, @Description, @DescriptionShort, @Img, @IdCategory)";
                    await using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        command.Parameters.AddWithValue("@Name", addProduct.Name);
                        command.Parameters.AddWithValue("@Price", addProduct.Price);
                        command.Parameters.AddWithValue("@Description", addProduct.Description);
                        command.Parameters.AddWithValue("@DescriptionShort", addProduct.DescriptionShort);
                        command.Parameters.AddWithValue("@Img", addProduct.Img);
                        command.Parameters.AddWithValue("@IdCategory", addProduct.IdCategory);

                        int righeInteressate = await command.ExecuteNonQueryAsync();
                    }
                }

            }

            await Banner();
            return RedirectToAction("AdminPage");
        }

        [HttpGet("Admin/AdminPage/Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM PRODUCTS WHERE Id = @Id";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int righeInteressate = await command.ExecuteNonQueryAsync();
                }
            }
            await Banner();
            return RedirectToAction("AdminPage");
        }

        [HttpGet("Admin/AdminPage/Delete/{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            int pippo = 1;
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var queryCheck = "SELECT COUNT(*) FROM PRODUCTS WHERE IdCategory = @Id";
                var query = "DELETE FROM CATEGORIES WHERE Id = @Id";
                await using (SqlCommand command = new SqlCommand(queryCheck, connection))
                {
                    command.Parameters.AddWithValue("@Id", id); 
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            pippo = reader.GetInt32(0);
                        }
                    }
                }
                if (pippo == 0)
                {
                    await using (SqlCommand command2 = new SqlCommand(query, connection))
                    {
                        command2.Parameters.AddWithValue("@Id", id);

                        int righeInteressate = await command2.ExecuteNonQueryAsync();
                    }
                }
                else { TempData["ErrorDel"] = "Errore: Impossibile cancellare la categoria, presenti prodotti"; }

            }

            await Banner();
            return RedirectToAction("AdminPage");
        }
    }
}
