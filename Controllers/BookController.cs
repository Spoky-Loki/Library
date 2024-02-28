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
        private NpgsqlConnection DbConnection;
        private NpgsqlCommand DbCommand;

        public BookController()
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

            DbCommand.CommandText = $"SELECT * FROM Books";
            NpgsqlDataReader reader = await DbCommand.ExecuteReaderAsync();

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
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"INSERT INTO Books (title, author, description, count) VALUES (@title, @author, @description, @count)";
            DbCommand.Parameters.AddWithValue("@title", bookModel.Title);
            DbCommand.Parameters.AddWithValue("@author", bookModel.Author);
            DbCommand.Parameters.AddWithValue("@description", bookModel.Description);
            DbCommand.Parameters.AddWithValue("@count", bookModel.Count);
            await DbCommand.ExecuteNonQueryAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"SELECT * FROM Books WHERE id = {id}";
            NpgsqlDataReader reader = await DbCommand.ExecuteReaderAsync();

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
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"UPDATE Books " +
                $"SET title='{bookModel.Title}'," +
                $"author='{bookModel.Author}'," +
                $"description='{bookModel.Description}'," +
                $"count='{bookModel.Count}' " +
                $"WHERE id={id}";
            await DbCommand.ExecuteNonQueryAsync();

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"DELETE FROM Books WHERE id={id}";
            await DbCommand.ExecuteNonQueryAsync();

            return RedirectToAction("Index");
        }
    }
}
