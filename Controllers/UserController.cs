using Library.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class UserController : Controller
    {
        private NpgsqlConnection DbConnection;
        private NpgsqlCommand DbCommand;

        public UserController()
        {
            DbConnection = new NpgsqlConnection(
                connectionString: WebConfigurationManager.ConnectionStrings["DbContext"].ConnectionString);

            DbCommand = new NpgsqlCommand();
            DbCommand.Connection = DbConnection;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"SELECT * FROM Users";
            NpgsqlDataReader reader = await DbCommand.ExecuteReaderAsync();

            var users = new List<UserModel>();
            while (await reader.ReadAsync())
            {
                users.Add(new UserModel(
                    (int)reader["id"],
                    reader["first_name"] as string,
                    reader["last_name"] as string,
                    reader["email"] as string,
                    reader["phone_number"] as string,
                    reader["address"] as string));
            }

            return View(users);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new UserModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserModel userModel)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"INSERT INTO Users (first_name, last_name, email, phone_number, address) VALUES (@firstName, @lastName, @email, @phoneNumber, @address)";
            DbCommand.Parameters.AddWithValue("@firstName", userModel.FirstName);
            DbCommand.Parameters.AddWithValue("@lastName", userModel.LastName);
            DbCommand.Parameters.AddWithValue("@email", userModel.Email);
            DbCommand.Parameters.AddWithValue("@phoneNumber", userModel.PhoneNumber);
            DbCommand.Parameters.AddWithValue("@address", userModel.Address);
            await DbCommand.ExecuteNonQueryAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"SELECT * FROM Users WHERE id = {id}";
            NpgsqlDataReader reader = await DbCommand.ExecuteReaderAsync();

            UserModel user = new UserModel();
            while (await reader.ReadAsync())
            {
                user = new UserModel(
                    (int)reader["id"],
                    reader["first_name"] as string,
                    reader["last_name"] as string,
                    reader["email"] as string,
                    reader["phone_number"] as string,
                    reader["address"] as string);
            }

            if (user.FirstName != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, UserModel userModel)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"UPDATE Users " +
                $"SET first_name='{userModel.FirstName}'," +
                $"last_name='{userModel.LastName}'," +
                $"email='{userModel.Email}'," +
                $"phone_number='{userModel.PhoneNumber}'," +
                $"address='{userModel.Address}' " +
                $"WHERE id={id}";
            await DbCommand.ExecuteNonQueryAsync();

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"DELETE FROM Users WHERE id={id}";
            await DbCommand.ExecuteNonQueryAsync();

            return RedirectToAction("Index");
        }
    }
}
