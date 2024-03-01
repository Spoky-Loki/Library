using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class LoanCreateViewModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Индификатор книги")]
        [Range(1, int.MaxValue, ErrorMessage = "Индификатор не может быть меньше единицы")]
        public int BookId { get; set; }

        [Required]
        [DisplayName("Индификатор пользователя")]
        [Range(1, int.MaxValue, ErrorMessage = "Индификатор не может быть меньше единицы")]
        public int UserId { get; set; }

        [Required]
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