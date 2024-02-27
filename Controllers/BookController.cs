using Library.Models;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class BookController : Controller
    {
        private NpgsqlConnection connection;
        private NpgsqlCommand command;

        public BookController()
        {
            connection = new NpgsqlConnection(
                connectionString: WebConfigurationManager.ConnectionStrings["DbContext"].ConnectionString);

            command = new NpgsqlCommand();
            command.Connection = connection;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            command.Connection.Open();

            command.CommandText = $"SELECT * FROM Books";
            NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            var books = new List<BookModel>();
            while (await reader.ReadAsync())
            {
                books.Add(new BookModel(
                    (int)reader["id"],
                    reader["title"] as string,
                    reader["author"] as string,
                    reader["description"] as string,
                    (int)reader["count"]));
            }

            return View(books);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new BookModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(BookModel bookModel)
        {
            command.Connection.Open();

            command.CommandText = $"INSERT INTO Books (title, author, description, count) VALUES (@title, @author, @description, @count)";
            command.Parameters.AddWithValue("@title", bookModel.Title);
            command.Parameters.AddWithValue("@author", bookModel.Author);
            command.Parameters.AddWithValue("@description", bookModel.Description);
            command.Parameters.AddWithValue("@count", bookModel.Count);
            await command.ExecuteNonQueryAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            command.Connection.Open();

            command.CommandText = $"SELECT * FROM Books WHERE id = {id}";
            NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            BookModel book = new BookModel();
            while (await reader.ReadAsync())
            {
                book = new BookModel(
                    (int)reader["id"],
                    reader["title"] as string,
                    reader["author"] as string,
                    reader["description"] as string,
                    (int)reader["count"]);
            }

            if (book.Title != null)
                return View(book);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, BookModel bookModel)
        {
            command.Connection.Open();

            command.CommandText = $"UPDATE Books " +
                $"SET title='{bookModel.Title}'," +
                $"author='{bookModel.Author}'," +
                $"description='{bookModel.Description}'," +
                $"count='{bookModel.Count}' " +
                $"WHERE id={id}";
            await command.ExecuteNonQueryAsync();

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            command.Connection.Open();

            command.CommandText = $"DELETE FROM Books WHERE id={id}";
            await command.ExecuteNonQueryAsync();

            return RedirectToAction("Index");
        }
    }
}
