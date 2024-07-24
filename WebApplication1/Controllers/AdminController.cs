using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly string _connectionString;

        public AdminController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public ActionResult Admindashboard()
        {
            return View();
        }

        public IActionResult Admin()
        {
            List<Register> admins = new List<Register>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM register WHERE status = 'pending'";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Register admin = new Register
                            {
                                user_id = Convert.ToInt32(reader["user_id"]),
                                name = reader["name"].ToString(),
                                position = reader["position"].ToString(),
                                email = reader["email"].ToString(),
                                username = reader["username"].ToString(),
                                password = reader["password"].ToString(),
                                status = reader["status"].ToString(),
                                UserType = reader["usertype"].ToString()
                            };
                            admins.Add(admin);
                        }
                    }
                }
            }


            return View(admins);
        }

        public ActionResult Adminapproved()
        {
            List<Register> users = new List<Register>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM register WHERE status = 'Approved'";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Register user = new Register
                            {
                                user_id = Convert.ToInt32(reader["user_id"]),
                                name = reader["name"].ToString(),
                                position = reader["position"].ToString(),
                                email = reader["email"].ToString(),
                                username = reader["username"].ToString(),
                                password = reader["password"].ToString(),
                                status = reader["status"].ToString(),
                                UserType = reader["UserType"].ToString()
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return View("~/Views/Admin/adminapproved.cshtml", users);
        }
        public ActionResult Admindenied()
        {
            List<Register> users = new List<Register>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM register WHERE status = 'Denied'";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Register user = new Register
                            {
                                user_id = Convert.ToInt32(reader["user_id"]),
                                name = reader["name"].ToString(),
                                position = reader["position"].ToString(),
                                email = reader["email"].ToString(),
                                username = reader["username"].ToString(),
                                password = reader["password"].ToString(),
                                status = reader["status"].ToString(),
                                UserType = reader["UserType"].ToString()
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return View("~/Views/Admin/admindenied.cshtml", users);
        }
        private void UpdateUserApprovalStatus(Register user, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE register SET status = @Status WHERE username = @Username";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.username);
                    command.Parameters.AddWithValue("@Status", status);
                    command.ExecuteNonQuery();
                }
            }
        }

        private string GetUserPosition(string username)
        {
            string position = "";

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT position FROM register WHERE username = @Username";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();

                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        position = result.ToString();
                    }
                }
            }

            return position;
        }

        [HttpPost]
        public ActionResult AdminApproval(string action, Register user)
        {
            string username = HttpContext.Session.GetString("Username");
            string userPosition = GetUserPosition(username);

            if (action == "Approved")
            {
                UpdateUserApprovalStatus(user, "Approved");
            }
            else if (action == "Deny")
            {
                UpdateUserApprovalStatus(user, "Denied");
            }

            return RedirectToAction("admin");
        }

        [HttpPost]
        public IActionResult DeleteAdmin(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM register WHERE username = @Username";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected <= 0)
                    {
                        // Admin not found or delete operation failed
                        // Handle accordingly
                    }
                }
            }

            return RedirectToAction("admin");
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "User");
        }
    }
}
