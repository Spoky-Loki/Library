using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class LoanModel
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int UserId { get; set; }

        public string LoanDate { get; set; }

        public string ReturnDate { get; set; }

        public LoanModel() { }

        public LoanModel(int id, int bookId, int userId, string loanDate, string returnDate)
        {
            Id = id;
            BookId = bookId;
            UserId = userId;
            LoanDate = loanDate;
            ReturnDate = returnDate;
        }
    }
}