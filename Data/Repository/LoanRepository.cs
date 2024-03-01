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
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"INSERT INTO Loans (book_id, user_id, loan_date) VALUES (@book, @user, @date)";
            _dbContext.GetCommand().Parameters.AddWithValue("@book", loanModel.BookId);
            _dbContext.GetCommand().Parameters.AddWithValue("@user", loanModel.UserId);
            _dbContext.GetCommand().Parameters.AddWithValue("@date", DateTime.Parse(loanModel.LoanDate));
            
            await _dbContext.GetCommand().ExecuteNonQueryAsync();
            _dbContext.GetCommand().Connection.Close();
        }

        public async Task<int> GetBookId(int id)
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"SELECT book_id FROM Loans WHERE id = {id}";
            NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

            await reader.ReadAsync();
            int bookId = (int)reader["book_id"];

            _dbContext.GetCommand().Connection.Close();
            return bookId;
        }

        public LoanCreateViewModel GetEmptyLoan()
        {
            return new LoanCreateViewModel();
        }

        public async Task<List<LoanIndexViewModel>> GetLoans()
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"SELECT l.id, l.loan_date, l.return_date, u.first_name, u.last_name, b.title " +
                $"FROM loans l " +
                $"JOIN users u ON l.user_id = u.id " +
                $"JOIN books b ON l.book_id = b.id";
            NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

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

            _dbContext.GetCommand().Connection.Close();
            return loans;
        }

        public async Task SetReturnDate(int id)
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"UPDATE Loans " +
                $"SET return_date='{DateTime.Now}' " +
                $"WHERE id = {id}";
            await _dbContext.GetCommand().ExecuteNonQueryAsync();

            _dbContext.GetCommand().Connection.Close();
        }

        public async Task<bool> IsReturned(int id)
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"SELECT return_date " +
                $"FROM Loans " +
                $"WHERE id = {id}";
            var reader = await _dbContext.GetCommand().ExecuteReaderAsync();

            await reader.ReadAsync();
            var date = reader["return_date"].ToString();

            _dbContext.GetCommand().Connection.Close();

            if (date == null)
                return false;
            return true;
        }
    }
}