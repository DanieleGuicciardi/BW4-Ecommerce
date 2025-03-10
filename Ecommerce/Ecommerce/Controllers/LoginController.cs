﻿using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Ecommerce.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;
        public LoginController(ILogger<HomeController> logger)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }


        public async Task<Object> IsUserLogged()
        {
            //query per controllare se ci sono account loggati
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

            //query per controllare Admin
            bool isAdmin = false;
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Admin FROM LOGIN WHERE IsLogged=1";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            isAdmin = reader.GetBoolean(0);
                        }
                    }
                }
            }

            if (loggedCount >= 1)
            {
                return (ViewData["IsLogged"] = true, ViewData["IsAdmin"] = isAdmin);

            }
            else
            {
                return ViewData["IsLogged"] = false;
            }
        }


        public async Task<IActionResult> LoginPage()
        {
            await IsUserLogged();
            return View();
        }

        public async Task<IActionResult> Login(LoginModel giovanni)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Errore nel modello del form";
                return RedirectToAction("LoginPage");
            }
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE LOGIN SET IsLogged=1 WHERE Username=@Username AND Password=@Password ";
                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", giovanni.Username);
                    command.Parameters.AddWithValue("@Password", giovanni.Password);
                    int righeInteressate = await command.ExecuteNonQueryAsync();
                    if (righeInteressate == 0)
                    {
                        TempData["ErrorLogin"] = "Attenzione! Username o Password errati";
                        return RedirectToAction("LoginPage");
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE LOGIN SET IsLogged=0";
                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            ViewData["IsLogged"] = false;
            return RedirectToAction("LoginPage");
        }

        public async Task<IActionResult> RegisterPage()
        {
            await IsUserLogged();
            return View();
        }

        public async Task<IActionResult> RegisterSave(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Errore nel modello del form";
                return RedirectToAction("RegisterPage");
            }
            if (registerModel.Password != registerModel.ConfirmPassword)
            {
                TempData["ErrorPassword"] = "Le password non coincidono";
                return RedirectToAction("RegisterPage");
            }
            try
            {
                await using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO LOGIN VALUES (@Id, @Username, @Password, 0, 0)";

                    await using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        command.Parameters.AddWithValue("@Username", registerModel.Username);
                        command.Parameters.AddWithValue("@Password", registerModel.Password);
                        int righeInteressate = await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                var eccezione = ex.Message;
                TempData["UniqueUsername"] = "Username già in uso!";
                return RedirectToAction("RegisterPage");
            }
            
            return RedirectToAction("LoginPage");
        }
    }
}
