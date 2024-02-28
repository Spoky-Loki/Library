using Library.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class LoanController : Controller
    {
        private NpgsqlConnection DbConnection;
        private NpgsqlCommand DbCommand;

        public LoanController()
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

            DbCommand.CommandText = $"SELECT l.id, l.loan_date, l.return_date, u.first_name, u.last_name, b.title " +
                $"FROM loans l " +
                $"JOIN users u ON l.user_id = u.id " +
                $"JOIN books b ON l.book_id = b.id";
            NpgsqlDataReader reader = await DbCommand.ExecuteReaderAsync();

            var loans = new List<LoanIndexViewModel>();
            while (await reader.ReadAsync())
            {
                loans.Add(new LoanIndexViewModel(
                    (int)reader["id"],
                    reader["title"] as string,
                    reader["first_name"] as string,
                    reader["last_name"] as string,
                    reader["loan_date"].ToString().Split(' ').First(),
                    reader["return_date"].ToString().Split(' ').First()));
            }

            return View(loans);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new LoanCreateViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(LoanCreateViewModel loanModel)
        {
            int bookCount = await GetBookCount(loanModel.BookId);
            if (bookCount == 0)
                return RedirectToAction("Index");

            DbCommand.Connection.Open();

            DbCommand.CommandText = $"INSERT INTO Loans (book_id, user_id, loan_date) VALUES (@book, @user, @date)";
            DbCommand.Parameters.AddWithValue("@book", loanModel.BookId);
            DbCommand.Parameters.AddWithValue("@user", loanModel.UserId);
            DbCommand.Parameters.AddWithValue("@date", DateTime.Parse(loanModel.LoanDate));
            await DbCommand.ExecuteNonQueryAsync();

            DbCommand.Connection.Close();

            await ReduceBookCount(loanModel.BookId, bookCount);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> ReturnBook(int id)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"UPDATE Loans " +
                $"SET return_date='{DateTime.Now}' " +
                $"WHERE id = {id}";
            await DbCommand.ExecuteNonQueryAsync();

            DbCommand.Connection.Close();

            var tuple = await GetBookIdAndBookCountOnLoanId(id);

            await AddBookCount(tuple.Item1, tuple.Item2);

            return RedirectToAction("Index");
        }

        private async Task<int> GetBookCount(int BookId)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"SELECT count FROM Books where id={BookId}";
            NpgsqlDataReader reader = await DbCommand.ExecuteReaderAsync();

            await reader.ReadAsync();
            int count = (int)reader["count"];

            DbCommand.Connection.Close();

            return count;
        }

        private async Task ReduceBookCount(int BookId, int count)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"UPDATE Books " +
                $"SET count={--count} " +
                $"WHERE id = {BookId}";
            await DbCommand.ExecuteNonQueryAsync();

            DbCommand.Connection.Close();
        }

        private async Task AddBookCount(int BookId, int count)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"UPDATE Books " +
                $"SET count={++count} " +
                $"WHERE id = {BookId}";
            await DbCommand.ExecuteNonQueryAsync();

            DbCommand.Connection.Close();
        }

        private async Task<Tuple<int, int>> GetBookIdAndBookCountOnLoanId(int loanId)
        {
            DbCommand.Connection.Open();

            DbCommand.CommandText = $"SELECT b.id, b.count FROM Loans l " +
                $"JOIN books b ON l.book_id=b.id " +
                $"WHERE l.id = {loanId}";
            NpgsqlDataReader reader = await DbCommand.ExecuteReaderAsync();

            await reader.ReadAsync();
            int count = (int)reader["count"];
            int bookId = (int)reader["id"];

            DbCommand.Connection.Close();

            return Tuple.Create(bookId, count);
        }
    }
}
