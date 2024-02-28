using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class LoanCreateViewModel
    {
        public int Id { get; set; }

        [DisplayName("Индификатор книги")]
        public int BookId { get; set; }

        [DisplayName("Индификатор пользователя")]
        public int UserId { get; set; }

        [DisplayName("Дата выдачи")]
        [DataType(DataType.Date)]
        public string LoanDate { get; set; }

        public LoanCreateViewModel() {}

        public LoanCreateViewModel(int id, int bookId, int userId, string loanDate)
        {
            Id = id;
            BookId = bookId;
            UserId = userId;
            LoanDate = loanDate;
        }
    }
}