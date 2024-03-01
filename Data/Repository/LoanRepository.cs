using Library.Data.Interfaces;
using Library.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Data.Repository
{
    public class LoanRepository : ILoanRepository
    {
        private static LoanRepository instance;

        private readonly DbContext _dbContext;

        private LoanRepository()
        {
            _dbContext = DbContext.GetContext();
        }

        public static LoanRepository GetInstance() 
        {
            if (instance == null)
                instance = new LoanRepository();
            return instance;
        }

        public async Task AddLoan(LoanCreateViewModel loanModel)
        {
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"INSERT INTO Loans (book_id, user_id, loan_date) VALUES (@book, @user, @date)";
                _dbContext.GetCommand().Parameters.AddWithValue("@book", loanModel.BookId);
                _dbContext.GetCommand().Parameters.AddWithValue("@user", loanModel.UserId);
                _dbContext.GetCommand().Parameters.AddWithValue("@date", DateTime.Parse(loanModel.LoanDate));

                await _dbContext.GetCommand().ExecuteNonQueryAsync();
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }
        }

        public async Task<int> GetBookId(int id)
        {
            int bookId;
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"SELECT book_id FROM Loans WHERE id = {id}";
                NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

                await reader.ReadAsync();
                bookId = (int)reader["book_id"];
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }
            return bookId;
        }

        public LoanCreateViewModel GetEmptyLoan()
        {
            return new LoanCreateViewModel();
        }

        public async Task<List<LoanIndexViewModel>> GetLoans()
        {
            List<LoanIndexViewModel> loans = new List<LoanIndexViewModel>();
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"SELECT l.id, l.loan_date, l.return_date, u.first_name, u.last_name, b.title " +
                    $"FROM loans l " +
                    $"JOIN users u ON l.user_id = u.id " +
                    $"JOIN books b ON l.book_id = b.id " +
                    $"WHERE l.return_date IS NULL";
                NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

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
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }
            return loans;
        }

        public async Task SetReturnDate(int id)
        {
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"UPDATE Loans " +
                    $"SET return_date='{DateTime.Now}' " +
                    $"WHERE id = {id}";
                await _dbContext.GetCommand().ExecuteNonQueryAsync();
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }
        }

        public async Task<bool> IsReturned(int id)
        {
            string date = null;
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"SELECT return_date " +
                    $"FROM Loans " +
                    $"WHERE id = {id}";
                var reader = await _dbContext.GetCommand().ExecuteReaderAsync();

                await reader.ReadAsync();
                date = reader["return_date"].ToString();
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }

            if (date == null || date == "")
                return false;
            return true;
        }

        public async Task<List<LoanIndexViewModel>> GetReturned()
        {
            List<LoanIndexViewModel> loans = new List<LoanIndexViewModel>();
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"SELECT l.id, l.loan_date, l.return_date, u.first_name, u.last_name, b.title " +
                    $"FROM loans l " +
                    $"JOIN users u ON l.user_id = u.id " +
                    $"JOIN books b ON l.book_id = b.id " +
                    $"WHERE l.return_date IS NOT NULL";
                NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

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
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }
            return loans;
        }
    }
}