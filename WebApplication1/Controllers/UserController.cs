using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Xml.Linq;
using WebApplication1.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly string _connectionString;
        const string SessionId = "_AdminID";
        const string SessionName = "_AdminName";
        const string SessionLayout = "_AdminLayout";
        const string SessionTitle = "_AdminJob";
        public void GetContext()
        {
            ViewBag.JobTitle = HttpContext.Session.GetString(SessionTitle);
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            ViewBag.Id = HttpContext.Session.GetString(SessionId);
        }
        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public ActionResult Login()
        {
            ViewBag.InvalidCredentials = false;
            return View();
        }

        public ActionResult TKdashboard()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }
            string name;
            int deniedByOMCount = 0;

            // Retrieve the user's name
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }
                string countByDenyOMSql = "SELECT COUNT(*) FROM employee WHERE status = 'Denied by OM'";
                using (var countByDenyCommand = new SqlCommand(countByDenyOMSql, connection))
                {
                    deniedByOMCount = (int)countByDenyCommand.ExecuteScalar();
                }
            }
            List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int siteId = reader.GetInt32(0);
                            string siteName = reader.GetString(1);
                            string clientName = reader.GetString(2);
                            siteAndClientNames.Add((siteId, siteName, clientName));
                        }
                    }
                }
            }

            ViewBag.SiteAndClientNames = siteAndClientNames;
            ViewBag.Username = string.IsNullOrEmpty(name) ? "Unknown" : name;
            ViewBag.DeniedByOM = deniedByOMCount;
            return View("/Views/User/TK/tkdashboard.cshtml");
        }

        public ActionResult OMdashboard()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }
            string name;
            int approvedEmployeeCount = 0;
            int deniedBySOMCount = 0;

            // Retrieve the user's name
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }

                string countSql = "SELECT COUNT(*) FROM employee WHERE status = 'pending'";
                using (var countCommand = new SqlCommand(countSql, connection))
                {
                    approvedEmployeeCount = (int)countCommand.ExecuteScalar();
                }
                string countByDenySOMSql = "SELECT COUNT(*) FROM employee WHERE status = 'Denied by SOM'";
                using (var countByDenyCommand = new SqlCommand(countByDenySOMSql, connection))
                {
                    deniedBySOMCount = (int)countByDenyCommand.ExecuteScalar();
                }

            }

            ViewBag.Username = string.IsNullOrEmpty(name) ? "Unknown" : name;
            ViewBag.ApprovedEmployeeCount = approvedEmployeeCount;
            ViewBag.DeniedBySOM = deniedBySOMCount;
            return View("/Views/User/OM/omdashboard.cshtml");
        }

        public ActionResult SOMdashboard()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            string name;
            int approvedEmployeeCount = 0;
            int deniedByIMCount = 0;

            // Retrieve the user's name
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }

                string countSql = "SELECT COUNT(*) FROM employee WHERE status = 'Approved by OM'";
                using (var countCommand = new SqlCommand(countSql, connection))
                {
                    approvedEmployeeCount = (int)countCommand.ExecuteScalar();
                }
                string countByDenyIMSql = "SELECT COUNT(*) FROM employee WHERE status = 'Denied by IM'";
                using (var countByDenyCommand = new SqlCommand(countByDenyIMSql, connection))
                {
                    deniedByIMCount = (int)countByDenyCommand.ExecuteScalar();
                }
            }

            ViewBag.Username = string.IsNullOrEmpty(name) ? "Unknown" : name;
            ViewBag.ApprovedEmployeeCount = approvedEmployeeCount;
            ViewBag.DeniedByIM = deniedByIMCount;
            return View("/Views/User/SOM/somdashboard.cshtml");
        }
        public ActionResult IMdashboard()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            string name;
            int approvedEmployeeCount = 0;
            int deniedByAccCount = 0;

            // Retrieve the user's name
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }

                string countSql = "SELECT COUNT(*) FROM employee WHERE status = 'Approved by SOM'";
                using (var countCommand = new SqlCommand(countSql, connection))
                {
                    approvedEmployeeCount = (int)countCommand.ExecuteScalar();
                }
                string countByDenyAccSql = "SELECT COUNT(*) FROM employee WHERE status = 'Denied by Accounting'";
                using (var countByDenyCommand = new SqlCommand(countByDenyAccSql, connection))
                {
                    deniedByAccCount = (int)countByDenyCommand.ExecuteScalar();
                }
            }

            ViewBag.Username = string.IsNullOrEmpty(name) ? "Unknown" : name;
            ViewBag.ApprovedEmployeeCount = approvedEmployeeCount;
            ViewBag.DeniedByAcc = deniedByAccCount;
            return View("/Views/User/IM/imdashboard.cshtml");
        }

        public ActionResult Accdashboard()
        {
            // Check if the user is logged in
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            string name;
            int approvedEmployeeCount = 0; 
            int approvedByIMCount = 0;

            // Retrieve the user's name
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }

                string countSql = "SELECT COUNT(*) FROM employee WHERE status = 'Employee Approved'";
                using (var countCommand = new SqlCommand(countSql, connection))
                {
                    approvedEmployeeCount = (int)countCommand.ExecuteScalar();
                }
                string countByIMSql = "SELECT COUNT(*) FROM employee WHERE status = 'Approved by IM'";
                using (var countByIMCommand = new SqlCommand(countByIMSql, connection))
                {
                    approvedByIMCount = (int)countByIMCommand.ExecuteScalar();
                }
            }

            ViewBag.Username = string.IsNullOrEmpty(name) ? "Unknown" : name; 
            ViewBag.ApprovedEmployeeCount = approvedEmployeeCount;
            ViewBag.ApprovedByIM = approvedByIMCount;

            
            return View("/Views/User/ACCOUNTING/accdashboard.cshtml");
        }



        public ActionResult Approval()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id,s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'pending'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    site_id = Convert.ToInt32(reader["site_id"]),
                                    status = reader["status"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }

                List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int siteId = reader.GetInt32(0);
                                string siteName = reader.GetString(1);
                                string clientName = reader.GetString(2);
                                siteAndClientNames.Add((siteId, siteName, clientName));
                            }
                        }
                    }
                }


                ViewBag.SiteAndClientNames = siteAndClientNames;

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }

                
                ViewBag.Username = name;
                return View("/Views/User/OM/approval.cshtml", employees);
            }
            catch (Exception ex)
            {
                
                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult Som()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Approved by OM'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    site_id = Convert.ToInt32(reader["site_id"]),
                                    status = reader["status"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int siteId = reader.GetInt32(0);
                                string siteName = reader.GetString(1);
                                string clientName = reader.GetString(2);
                                siteAndClientNames.Add((siteId, siteName, clientName));
                            }
                        }
                    }
                }


                ViewBag.SiteAndClientNames = siteAndClientNames;

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/SOM/som.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }
        public ActionResult Im()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Approved by SOM'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    site_id = Convert.ToInt32(reader["rate"]),
                                    status = reader["status"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int siteId = reader.GetInt32(0);
                                string siteName = reader.GetString(1);
                                string clientName = reader.GetString(2);
                                siteAndClientNames.Add((siteId, siteName, clientName));
                            }
                        }
                    }
                }


                ViewBag.SiteAndClientNames = siteAndClientNames;

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/IM/IM.cshtml", employees);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult Accounting()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Approved by IM'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    site_id = Convert.ToInt32(reader["site_id"]),
                                    status = reader["status"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int siteId = reader.GetInt32(0);
                                string siteName = reader.GetString(1);
                                string clientName = reader.GetString(2);
                                siteAndClientNames.Add((siteId, siteName, clientName));
                            }
                        }
                    }
                }


                ViewBag.SiteAndClientNames = siteAndClientNames;

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/ACCOUNTING/accounting.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult SaveSiteInfoToDatabaseIM(string siteName, string clientName, string om)
        {
            string status = "Active";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("INSERT INTO site (site_name, client_name, om, status) VALUES (@SiteName, @ClientName, @OM, @Status)", connection))
                {
                    command.Parameters.AddWithValue("@SiteName", siteName);
                    command.Parameters.AddWithValue("@ClientName", clientName);
                    command.Parameters.AddWithValue("@OM", om);
                    command.Parameters.AddWithValue("@Status", status);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("imdashboard");
        }


        public ActionResult IMAddSite()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            string name;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }

                var operationManagers = new List<string>();
                using (var command = new SqlCommand("SELECT name FROM register WHERE position = 'OPERATION MANAGER'", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            operationManagers.Add(reader.GetString(0));
                        }
                    }
                }
                ViewBag.OperationManagers = operationManagers;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "Unknown";
            }

            ViewBag.Username = name;
            return View("/Views/User/IM/IMAddSite.cshtml");
        }

        public ActionResult IMViewSite()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            string name;
            List<Site> sites = new List<Site>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }

                using (var command = new SqlCommand("SELECT site_id, site_name, client_name, om, status FROM site", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sites.Add(new Site
                            {
                                site_id = Convert.ToInt32(reader["site_id"]),
                                site_name = reader["site_name"].ToString(),
                                client_name = reader["client_name"].ToString(),
                                om = reader["om"].ToString(),
                                status = reader["status"].ToString()
                            });
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "Unknown";
            }

            ViewBag.Username = name;
            ViewBag.Sites = sites;
            return View("/Views/User/IM/IMViewSite.cshtml");
        }

        //acoounting sites
        public ActionResult AccAddSite()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            string name;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }

                var operationManagers = new List<string>();
                using (var command = new SqlCommand("SELECT name FROM register WHERE position = 'OPERATION MANAGER'", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            operationManagers.Add(reader.GetString(0));
                        }
                    }
                }
                ViewBag.OperationManagers = operationManagers;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "Unknown";
            }

            ViewBag.Username = name;
            return View("/Views/User/ACCOUNTING/accAddSite.cshtml");
        }
        public ActionResult AccViewSite()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            string name;
            List<Site> sites = new List<Site>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }

                using (var command = new SqlCommand("SELECT site_id, site_name, client_name, om, status FROM site", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sites.Add(new Site
                            {
                                site_id = Convert.ToInt32(reader["site_id"]),
                                site_name = reader["site_name"].ToString(),
                                client_name = reader["client_name"].ToString(),
                                om = reader["om"].ToString(),
                                status = reader["status"].ToString()
                            });
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "Unknown";
            }

            ViewBag.Username = name;
            ViewBag.Sites = sites; 
            return View("/Views/User/ACCOUNTING/accViewSite.cshtml");
        }

        public ActionResult GetEmployeeIM(int siteId)
        {
            try
            {
                List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.employeeId, e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id, s.site_name, s.client_name, e.rate, e.date_hired, e.status " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Employee Approved' AND e.site_id = @SiteId";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@SiteId", siteId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    position = reader["position"].ToString(),
                                    site_id = Convert.ToInt32(reader["site_id"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"]),
                                    status = reader["status"].ToString()
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }
                // Ensure that the view name is correctly referred
                return View("~/Views/User/IM/IMViewEmployeeDetails.cshtml", employees);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error view
                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }

        [HttpPost]
        public ActionResult EditEmployeeIM(EmployeeViewModel employee)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE employee SET position = @Position, rate = @Rate WHERE employeeId = @EmployeeId";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Position", employee.position);
                        command.Parameters.AddWithValue("@Rate", employee.rate);
                        command.Parameters.AddWithValue("@EmployeeId", employee.employeeId);

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("IMViewSite");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult GetEmployeeACC(int siteId) 
        {
            try
            {
                List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.employeeId, e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id, s.site_name, s.client_name, e.rate, e.date_hired, e.status " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Employee Approved' AND e.site_id = @SiteId";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@SiteId", siteId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    position = reader["position"].ToString(),
                                    site_id = Convert.ToInt32(reader["site_id"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"]),
                                    status = reader["status"].ToString()
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }
                
                return View("~/Views/User/ACCOUNTING/ACCViewEmployeeDetails.cshtml", employees);
            }
            catch (Exception ex)
            {
               
                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }

        [HttpPost]
        public ActionResult EditEmployeeACC(EmployeeViewModel employee)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE employee SET position = @Position, rate = @Rate WHERE employeeId = @EmployeeId";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Position", employee.position);
                        command.Parameters.AddWithValue("@Rate", employee.rate);
                        command.Parameters.AddWithValue("@EmployeeId", employee.employeeId);

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("accViewSite");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }

        private void UpdateSiteStatus(int siteId, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE site SET status = @Status WHERE site_id = @SiteID";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@SiteID", siteId);
                    command.Parameters.AddWithValue("@Status", status);
                    command.ExecuteNonQuery();
                }
            }
        }


        [HttpPost]
        public ActionResult ToggleStatusIM(int siteId)
        {
            
            string currentStatus = GetSiteStatus(siteId);

            // Determine the new status
            string newStatus = (currentStatus == "Active") ? "Inactive" : "Active";


            UpdateSiteStatus(siteId, newStatus);


            return RedirectToAction("IMViewSite");
        }

        [HttpPost]
        public ActionResult ToggleStatusACC(int siteId) //accounting
        {
            
            string currentStatus = GetSiteStatus(siteId);

            // Determine the new status
            string newStatus = (currentStatus == "Active") ? "Inactive" : "Active";

            
            UpdateSiteStatus(siteId, newStatus);

           
            return RedirectToAction("accViewSite");
        }

        private string GetSiteStatus(int siteId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT status FROM site WHERE site_id = @SiteID";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@SiteID", siteId);
                    return command.ExecuteScalar().ToString();
                }
            }
        }


        private void UpdateEmployeeApprovalStatus(EmployeeViewModel employee, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE employee SET status = @Status WHERE employeeId = @EmployeeID";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employee.employeeId);
                    command.Parameters.AddWithValue("@Status", status);
                    command.ExecuteNonQuery();
                }
            }
        }

        [HttpPost]
        public ActionResult SaveEmployeetk(int employeeId, string newName, string position, string site, int rate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE employee SET name = @NewName, position = @Position, site = @Site, rate = @Rate WHERE employeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Site", site);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("tkdashboard");
        }


        [HttpPost]
        public ActionResult SaveEmployeeom(int employeeId, string newName, string site, string position, string address, int rate)
        {
            int site_id;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var getSiteIdCommand = new SqlCommand("SELECT site_id FROM site WHERE site_name = @SiteName", connection))
                {
                    getSiteIdCommand.Parameters.AddWithValue("@SiteName", site);

                    var result = getSiteIdCommand.ExecuteScalar();
                    if (result == null)
                    {
                        return Content("Site not found.");
                    }

                    site_id = (int)result;
                }

                string query = "UPDATE employee SET name = @NewName, position = @Position, address = @Address, rate = @Rate, site_id = @SiteId WHERE employeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.Parameters.AddWithValue("@SiteId", site_id);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("approval");
        }
        [HttpPost]
        public ActionResult SaveEmployeeomdeny(int employeeId, string newName, string site, string position, string address, int rate)
        {
            int site_id;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var getSiteIdCommand = new SqlCommand("SELECT site_id FROM site WHERE site_name = @SiteName", connection))
                {
                    getSiteIdCommand.Parameters.AddWithValue("@SiteName", site);

                    var result = getSiteIdCommand.ExecuteScalar();
                    if (result == null)
                    {
                        return Content("Site not found.");
                    }

                    site_id = (int)result;
                }

                string query = "UPDATE employee SET name = @NewName, position = @Position, address = @Address, rate = @Rate, site_id = @SiteId WHERE employeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.Parameters.AddWithValue("@SiteId", site_id);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("omdeny");
        }

        public ActionResult SaveEmployeesom(int employeeId, string newName, string site, string position, string address, int rate)
        {
            int site_id;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var getSiteIdCommand = new SqlCommand("SELECT site_id FROM site WHERE site_name = @SiteName", connection))
                {
                    getSiteIdCommand.Parameters.AddWithValue("@SiteName", site);

                    var result = getSiteIdCommand.ExecuteScalar();
                    if (result == null)
                    {
                        return Content("Site not found.");
                    }

                    site_id = (int)result;
                }

                string query = "UPDATE employee SET name = @NewName, position = @Position, address = @Address, rate = @Rate, site_id = @SiteId WHERE employeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.Parameters.AddWithValue("@SiteId", site_id);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("som");
        }
        [HttpPost]
        public ActionResult SaveEmployeesomdeny(int employeeId, string newName, string site, string position, string address, int rate)
        {
            int site_id;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var getSiteIdCommand = new SqlCommand("SELECT site_id FROM site WHERE site_name = @SiteName", connection))
                {
                    getSiteIdCommand.Parameters.AddWithValue("@SiteName", site);

                    var result = getSiteIdCommand.ExecuteScalar();
                    if (result == null)
                    {
                        return Content("Site not found.");
                    }

                    site_id = (int)result;
                }

                string query = "UPDATE employee SET name = @NewName, position = @Position, address = @Address, rate = @Rate, site_id = @SiteId WHERE employeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.Parameters.AddWithValue("@SiteId", site_id);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("somdeny");
        }

        public ActionResult SaveEmployeeim(int employeeId, string newName, string site, string position, string address, int rate)
        {
            int site_id;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var getSiteIdCommand = new SqlCommand("SELECT site_id FROM site WHERE site_name = @SiteName", connection))
                {
                    getSiteIdCommand.Parameters.AddWithValue("@SiteName", site);

                    var result = getSiteIdCommand.ExecuteScalar();
                    if (result == null)
                    {
                        return Content("Site not found.");
                    }

                    site_id = (int)result;
                }

                string query = "UPDATE employee SET name = @NewName, position = @Position, address = @Address, rate = @Rate, site_id = @SiteId WHERE employeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.Parameters.AddWithValue("@SiteId", site_id);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("IM");
        }

        [HttpPost]
        public ActionResult SaveEmployeeimdeny(int employeeId, string newName, string site, string position, string address, int rate)
        {
            int site_id;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var getSiteIdCommand = new SqlCommand("SELECT site_id FROM site WHERE site_name = @SiteName", connection))
                {
                    getSiteIdCommand.Parameters.AddWithValue("@SiteName", site);

                    var result = getSiteIdCommand.ExecuteScalar();
                    if (result == null)
                    {
                        return Content("Site not found.");
                    }

                    site_id = (int)result;
                }

                string query = "UPDATE employee SET name = @NewName, position = @Position, address = @Address, rate = @Rate, site_id = @SiteId WHERE employeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.Parameters.AddWithValue("@SiteId", site_id);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("imdeny");
        }

        public ActionResult SaveEmployeeacc(int employeeId, string newName, string site, string position, string address, int rate)
        {
            int site_id;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var getSiteIdCommand = new SqlCommand("SELECT site_id FROM site WHERE site_name = @SiteName", connection))
                {
                    getSiteIdCommand.Parameters.AddWithValue("@SiteName", site);

                    var result = getSiteIdCommand.ExecuteScalar();
                    if (result == null)
                    {
                        return Content("Site not found.");
                    }

                    site_id = (int)result;
                }

                string query = "UPDATE employee SET name = @NewName, position = @Position, address = @Address, rate = @Rate, site_id = @SiteId WHERE employeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.Parameters.AddWithValue("@SiteId", site_id);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("accounting");
        }
        public ActionResult SaveEmployeeaccapproved(int employeeId, string newName, string site, string position, string address, int rate)
        {
            int site_id;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var getSiteIdCommand = new SqlCommand("SELECT site_id FROM site WHERE site_name = @SiteName", connection))
                {
                    getSiteIdCommand.Parameters.AddWithValue("@SiteName", site);

                    var result = getSiteIdCommand.ExecuteScalar();
                    if (result == null)
                    {
                        return Content("Site not found.");
                    }

                    site_id = (int)result;
                }

                string query = "UPDATE employee SET name = @NewName, position = @Position, address = @Address, rate = @Rate, site_id = @SiteId WHERE employeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.Parameters.AddWithValue("@SiteId", site_id);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("approved");
        }


        private void UpdateEmployeeRemarks(EmployeeViewModel employee, string remarks)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var commandText = "UPDATE employee SET remarks = @Remarks WHERE EmployeeId = @EmployeeId";
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Remarks", remarks);
                    command.Parameters.AddWithValue("@EmployeeId", employee.employeeId);
                    command.ExecuteNonQuery();
                }
            }
        }


        [HttpPost]
        public ActionResult ApprovalAction(string action, string remarks, EmployeeViewModel employee)
        {
            string username = HttpContext.Session.GetString("Username");
            string userPosition = GetUserPosition(username);

            if (userPosition == "TIMEKEEPER")
            {
                if (action == "Approved")
                {

                    UpdateEmployeeApprovalStatus(employee, "pending");


                    return RedirectToAction("tkdeny");
                }
            }
            if (userPosition == "OPERATION MANAGER")
            {
                if (action == "Approved")
                {
                   
                    UpdateEmployeeApprovalStatus(employee, "Approved by OM");

                    
                    return RedirectToAction("approval");
                }
                else if (action == "Denied")
                {
                   
                    UpdateEmployeeApprovalStatus(employee, "Denied by OM");
                    UpdateEmployeeRemarks(employee, remarks);
                    return RedirectToAction("omdashboard");
                }
            }
            else if (userPosition == "SENIOR OPERATION MANAGER")
            {
                if (action == "Approved")
                {
                    
                    UpdateEmployeeApprovalStatus(employee, "Approved by SOM");

                    
                    return RedirectToAction("som");
                }
                else if (action == "Denied")
                {
                    
                    UpdateEmployeeApprovalStatus(employee, "Denied by SOM");
                    UpdateEmployeeRemarks(employee, remarks);
                    return RedirectToAction("somdashboard");
                }
            }
            else if (userPosition == "INSPECTOR MANAGER")
            {
                if (action == "Approved")
                {
                    
                    UpdateEmployeeApprovalStatus(employee, "Approved by IM");

                    
                    return RedirectToAction("im");
                }
                else if (action == "Denied")
                {
                   
                    UpdateEmployeeApprovalStatus(employee, "Denied by IM");
                    UpdateEmployeeRemarks(employee, remarks);
                    return RedirectToAction("imdashboard");
                }
            }
            else if (userPosition == "ACCOUNTING")
            {
                if (action == "Approved")
                {

                    UpdateEmployeeApprovalStatus(employee, "Employee Approved");


                    return RedirectToAction("approved");
                }
                else if (action == "Denied")
                {

                    UpdateEmployeeApprovalStatus(employee, "Deleted by Accounting");
                    UpdateEmployeeRemarks(employee, remarks);
                    return RedirectToAction("accdashboard");
                }
            }
            return RedirectToAction("Login");
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

        private string GetUserType(string username)
        {
            string UserType = "";

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT UserType FROM register WHERE username = @Username";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();

                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        UserType = result.ToString();
                    }
                }
            }

            return UserType;
        }
        private string GetUserStatus(string username)
        {
            string status = "";

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT status FROM register WHERE username = @Username";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();

                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        status = result.ToString();
                    }
                }
            }

            return status;
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (IsValidCredentials(username, password))
            {
                string status = GetUserStatus(username);

                if (status == "Approved")
                {
                    string userType = GetUserType(username);
                    HttpContext.Session.SetString("Username", username);

                    if (userType == "admin")
                    {
                        return RedirectToAction("admindashboard", "Admin");
                    }
                    else if (userType == "user")
                    {
                        string position = GetUserPosition(username);

                        if (position == "TIMEKEEPER")
                        {
                            return RedirectToAction("tkdashboard", "User");
                        }
                        else if (position == "OPERATION MANAGER")
                        {
                            return RedirectToAction("omdashboard", "User");
                        }
                        else if (position == "SENIOR OPERATION MANAGER")
                        {
                            return RedirectToAction("somdashboard", "User");
                        }
                        else if (position == "INSPECTOR MANAGER")
                        {
                            return RedirectToAction("imdashboard", "User");
                        }
                        else if (position == "ACCOUNTING")
                        {
                            return RedirectToAction("accdashboard", "User");
                        }
                        else
                        {
                            ModelState.AddModelError("Invalid", "Invalid username or password");
                            return View();
                        }
                    }
                }
                else if (status == "Denied" || status == "pending")
                {
                    ModelState.AddModelError("Username", "Your account has not yet approved.");
                    return View();
                }
            }
            ModelState.AddModelError("Invalid", "Invalid username or password");
            return View();
        }
            

        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Register(string name, string position, string username, string UserType, string email, string password)
        {
            if (IsUsernameTaken(username))
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View();
            }
            if (IsEmailTaken(email))
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO register (name, username, position, email, password, UserType,status) VALUES (@Name, @Username, @Position, @Email, @Password, @Usertype, @Status)", connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Usertype", UserType);
                    command.Parameters.AddWithValue("@Status", "pending");
                    command.ExecuteNonQuery();
                }
            }

            HttpContext.Session.SetString("Username", username); 
            return RedirectToAction("Login");

        }

        private bool IsUsernameTaken(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        int count = Convert.ToInt32(result);
                        return count > 0;
                    }
                    return false;
                }
            }
        }
        private bool IsEmailTaken(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM register WHERE email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        int count = Convert.ToInt32(result);
                        return count > 0;
                    }
                    return false;
                }
            }
        }
        private bool IsValidCredentials(string username, string password)
        {
            bool isValid = false;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM register WHERE username = @Username AND password = @Password", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    var result = (int)command.ExecuteScalar();
                    isValid = result > 0;
                }
            }
            return isValid;
        }

        private bool IsAdmin(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM register WHERE username = @Username AND UserType = 'admin'", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    var result = (int)command.ExecuteScalar();
                    return result > 0;
                }
            }
        }


        [HttpPost]
        public ActionResult EmployeeViewModel(string name, string position, string site, int rate, string contact, string date_hired, string birthday, string address)
        {
            if (string.IsNullOrEmpty(site))
            {
                
                return Content("Site is required.");
            }

            int site_id;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                
                using (var getSiteIdCommand = new SqlCommand("SELECT site_id FROM site WHERE site_name = @SiteName", connection))
                {
                    getSiteIdCommand.Parameters.AddWithValue("@SiteName", site);

                    var result = getSiteIdCommand.ExecuteScalar();
                    if (result == null)
                    {
                       
                        return Content("Site not found.");
                    }

                    site_id = (int)result;
                }

                
                using (var command = new SqlCommand("INSERT INTO employee (name, position, contact_no, site_id, rate, date_hired, status, birthday, address) VALUES (@Name, @Position, @Contact, @SiteId, @Rate, @DateHired, @Status, @Birthday, @Address)", connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Contact", contact);
                    command.Parameters.AddWithValue("@Birthday", birthday);
                    command.Parameters.AddWithValue("@SiteId", site_id);
                    command.Parameters.AddWithValue("@Rate", rate);
                    command.Parameters.AddWithValue("@DateHired", date_hired);
                    command.Parameters.AddWithValue("@Status", "pending");
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("tkdashboard");
        }

        public ActionResult AddEmployee()
        {
            string userType = HttpContext.Session.GetString("UserType");
            if (userType == "admin")
            {
                return RedirectToAction("Login", "User");
            }

            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            string name;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    name = (string)command.ExecuteScalar();
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "Unknown"; 
            }

            
            ViewBag.Username = name;


            List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int siteId = reader.GetInt32(0);
                            string siteName = reader.GetString(1);
                            string clientName = reader.GetString(2);
                            siteAndClientNames.Add((siteId, siteName, clientName));
                        }
                    }
                }
            }


            ViewBag.SiteAndClientNames = siteAndClientNames;
            return View("/Views/User/TK/addemployee.cshtml");
        }

        public ActionResult Tkdeny()
            {
                List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_name, s.client_name, e.rate, e.date_hired, e.status,e.remarks, e.employeeId " +
                                     "FROM employee e " +
                                     "JOIN site s ON e.site_id = s.site_id " +
                                     "WHERE e.status = 'Denied by OM'";
                        using (var command = new SqlCommand(sql, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    EmployeeViewModel employee = new EmployeeViewModel
                                    {
                                        employeeId = Convert.ToInt32(reader["employeeId"]),
                                        name = reader["name"].ToString(),
                                        address = reader["address"].ToString(),
                                        birthday = Convert.ToDateTime(reader["birthday"]),
                                        site_name = reader["site_name"].ToString(),
                                        client_name = reader["client_name"].ToString(),
                                        position = reader["position"].ToString(),
                                        rate = Convert.ToInt32(reader["rate"]),
                                        contact_no = reader["contact_no"].ToString(),
                                        status = reader["status"].ToString(),
                                        remarks = reader["remarks"].ToString(),
                                        date_hired = Convert.ToDateTime(reader["date_hired"])
                                    };

                                    employees.Add(employee);
                                }
                            }
                        }
                    }
                List<(string siteName, string clientName)> siteAndClientNames = new List<(string, string)>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT site_name, client_name FROM site", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                siteAndClientNames.Add((reader.GetString(0), reader.GetString(1)));
                            }
                        }
                    }
                }


                ViewBag.SiteAndClientNames = siteAndClientNames;

                string username = HttpContext.Session.GetString("Username");
                    if (string.IsNullOrEmpty(username))
                    {
                        return RedirectToAction("Login");
                    }

                    string name;
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                        {
                            command.Parameters.AddWithValue("@Username", username);
                            name = (string)command.ExecuteScalar();
                        }
                    }

                    if (string.IsNullOrEmpty(name))
                    {
                        name = "Unknown";
                    }


                    ViewBag.Username = name;
                    return View("/Views/User/TK/tkdeny.cshtml", employees);
                }
                catch (Exception ex)
                {

                    ViewBag.Error = "An error occurred: " + ex.Message;
                    return View("/Views/Shared/Error.cshtml");
                }
            }

            public ActionResult Omdeny()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id, s.site_name, s.client_name, e.rate, e.date_hired, e.status,e.remarks, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Denied by SOM'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    site_id = Convert.ToInt32(reader["site_id"]),
                                    status = reader["status"].ToString(),
                                    remarks = reader["remarks"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int siteId = reader.GetInt32(0);
                                string siteName = reader.GetString(1);
                                string clientName = reader.GetString(2);
                                siteAndClientNames.Add((siteId, siteName, clientName));
                            }
                        }
                    }
                }
                ViewBag.SiteAndClientNames = siteAndClientNames;

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/OM/omdeny.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult Somdeny()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.remarks, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Denied by IM'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    site_id = Convert.ToInt32(reader["site_id"]),
                                    status = reader["status"].ToString(),
                                    remarks = reader["remarks"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int siteId = reader.GetInt32(0);
                                string siteName = reader.GetString(1);
                                string clientName = reader.GetString(2);
                                siteAndClientNames.Add((siteId, siteName, clientName));
                            }
                        }
                    }
                }
                ViewBag.SiteAndClientNames = siteAndClientNames;

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/SOM/somdeny.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }
        public ActionResult Imdeny()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.remarks, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Denied by Accounting'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    site_id = Convert.ToInt32(reader["site_id"]),
                                    status = reader["status"].ToString(),
                                    remarks = reader["remarks"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int siteId = reader.GetInt32(0);
                                string siteName = reader.GetString(1);
                                string clientName = reader.GetString(2);
                                siteAndClientNames.Add((siteId, siteName, clientName));
                            }
                        }
                    }
                }
                ViewBag.SiteAndClientNames = siteAndClientNames;

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/IM/imdeny.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }
        public ActionResult Approved()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_id, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Employee Approved'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    site_id = Convert.ToInt32(reader["site_id"]),
                                    status = reader["status"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                List<(int siteId, string siteName, string clientName)> siteAndClientNames = new List<(int, string, string)>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT site_id, site_name, client_name FROM site", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int siteId = reader.GetInt32(0);
                                string siteName = reader.GetString(1);
                                string clientName = reader.GetString(2);
                                siteAndClientNames.Add((siteId, siteName, clientName));
                            }
                        }
                    }
                }
                ViewBag.SiteAndClientNames = siteAndClientNames;

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/ACCOUNTING/approved.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }
        public ActionResult ApprovedOM()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Employee Approved'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    status = reader["status"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/OM/approvedOM.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }
        public ActionResult ApprovedSOM()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday, e.position, e.contact_no, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Employee Approved'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    status = reader["status"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/SOM/approvedSOM.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }
        public ActionResult ApprovedIM()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name, e.address, e.birthday,e.position, e.contact_no, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Employee Approved'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    status = reader["status"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/IM/approvedIM.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }
        public ActionResult ApprovedTK()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT e.name,e.address, e.birthday, e.position, e.contact_no, s.site_name, s.client_name, e.rate, e.date_hired, e.status, e.employeeId " +
                                 "FROM employee e " +
                                 "JOIN site s ON e.site_id = s.site_id " +
                                 "WHERE e.status = 'Employee Approved'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeViewModel employee = new EmployeeViewModel
                                {
                                    employeeId = Convert.ToInt32(reader["employeeId"]),
                                    name = reader["name"].ToString(),
                                    address = reader["address"].ToString(),
                                    birthday = Convert.ToDateTime(reader["birthday"]),
                                    site_name = reader["site_name"].ToString(),
                                    client_name = reader["client_name"].ToString(),
                                    position = reader["position"].ToString(),
                                    rate = Convert.ToInt32(reader["rate"]),
                                    contact_no = reader["contact_no"].ToString(),
                                    status = reader["status"].ToString(),
                                    date_hired = Convert.ToDateTime(reader["date_hired"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }

                string username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                string name;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM register WHERE username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        name = (string)command.ExecuteScalar();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }


                ViewBag.Username = name;
                return View("/Views/User/TK/approvedTK.cshtml", employees);
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("/Views/Shared/Error.cshtml");
            }
        }


        public ActionResult Logout()
        {
            
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "User");
        }


    }
}

