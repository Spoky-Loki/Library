using Library.Data.Interfaces;
using Library.Models;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Data.Repository
{
    public class BookRepository : IBookRepository
    {
        private static BookRepository instance;

        private readonly DbContext _dbContext;

        private BookRepository()
        {
            _dbContext = DbContext.GetContext();
        }

        public static BookRepository GetInstance()
        {
            if (instance == null)
                instance = new BookRepository();
            return instance;
        }

        public async Task AddBook(BookModel bookModel)
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"INSERT INTO Books (title, author, description, available) " +
                $"VALUES (@title, @author, @description, @available)";
            _dbContext.GetCommand().Parameters.AddWithValue("@title", bookModel.Title);
            _dbContext.GetCommand().Parameters.AddWithValue("@author", bookModel.Author);
            _dbContext.GetCommand().Parameters.AddWithValue("@description", bookModel.Description);
            _dbContext.GetCommand().Parameters.AddWithValue("@available", bookModel.Count);
            
            await _dbContext.GetCommand().ExecuteNonQueryAsync();
            _dbContext.GetCommand().Connection.Close();
        }

        public async Task DeleteBook(int id)
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"DELETE FROM Books WHERE id={id}";
            
            await _dbContext.GetCommand().ExecuteNonQueryAsync();
            _dbContext.GetCommand().Connection.Close();
        }

        public async Task<BookModel> GetBookById(int id)
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"SELECT * FROM Books WHERE id = {id}";
            NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

            await reader.ReadAsync();
            BookModel book = new BookModel(
                    (int)reader["id"],
                    reader["title"] as string,
                    reader["author"] as string,
                    reader["description"] as string,
                    (int)reader["available"]);

            _dbContext.GetCommand().Connection.Close();
            return book;
        }

        public async Task<List<BookModel>> GetBooks()
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"SELECT * FROM Books";
            NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

            var books = new List<BookModel>();
            while (await reader.ReadAsync())
            {
                books.Add(new BookModel(
                    (int)reader["id"],
                    reader["title"] as string,
                    reader["author"] as string,
                    reader["description"] as string,
                    (int)reader["available"]));
            }

            _dbContext.GetCommand().Connection.Close();
            return books;
        }

        public async Task UpdateBook(int id, BookModel bookModel)
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"UPDATE Books " +
                $"SET title='{bookModel.Title}'," +
                $"author='{bookModel.Author}'," +
                $"description='{bookModel.Description}'," +
                $"available='{bookModel.Count}' " +
                $"WHERE id={id}";

            await _dbContext.GetCommand().ExecuteNonQueryAsync();
            _dbContext.GetCommand().Connection.Close();
        }

        public BookModel GetEmptyBook()
        {
            return new BookModel();
        }

        public async Task<int> GetBookCount(int id)
        {
            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"SELECT available FROM Books WHERE id={id}";
            NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

            await reader.ReadAsync();
            var r = reader.Rows;
            int count = (int)reader["available"];

            _dbContext.GetCommand().Connection.Close();
            return count;
        }

        public async Task ReduceBookCount(int id)
        {
            int count = await GetBookCount(id);

            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"UPDATE Books " +
                $"SET available={--count} " +
                $"WHERE id = {id}";
            await _dbContext.GetCommand().ExecuteNonQueryAsync();

            _dbContext.GetCommand().Connection.Close();
        }

        public async Task AddBookCount(int id)
        {
            int count = await GetBookCount(id);

            _dbContext.GetCommand().Connection.Open();

            _dbContext.GetCommand().CommandText = $"UPDATE Books " +
                $"SET available={++count} " +
                $"WHERE id = {id}";
            await _dbContext.GetCommand().ExecuteNonQueryAsync();

            _dbContext.GetCommand().Connection.Close();
        }
    }
}