﻿using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    interface IBookRepository
    {
        Task<List<BookModel>> GetBooks();

        Task AddBook(BookModel book);

        Task<BookModel> GetBookById(int id);

        Task UpdateBook(int id, BookModel book);

        Task DeleteBook(int id);

        Task<int> GetBookCount(int id);

        Task ReduceBookCount(int id);

        Task AddBookCount(int id);

        BookModel GetEmptyBook();
    }
}
