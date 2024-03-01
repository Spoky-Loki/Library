using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class LoanIndexViewModel
    {
        public int Id { get; set; }

        [DisplayName("Название книги")]
        public string BookTitle { get; set; }

        [DisplayName("Имя пользователя")]
        public string UserFirstName { get; set; }

        [DisplayName("Фамилия пользователя")]
        public string UserLastName { get; set; }

        [DisplayName("Дата выдачи")]
        [DataType(DataType.Date)]
        public string LoanDate { get; set; }

        [DisplayName("Дата возврата")]
        [DataType(DataType.Date)]
        public string ReturnDate { get; set; }

        public LoanIndexViewModel() { }

        public LoanIndexViewModel(int id, string bookTitle, string userFirstName, 
            string userLastName, string loanDate, string returnDate)
        {
            Id = id;
            BookTitle = bookTitle;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            LoanDate = loanDate;
            ReturnDate = returnDate;
        }
    }
}