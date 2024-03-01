using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class BookModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Название")]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [DisplayName("Автор")]
        [DataType(DataType.Text)]
        public string Author { get; set; }

        [Required]
        [DisplayName("Описание")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Required]
        [DisplayName("Колличестов")]
        [Range(0, int.MaxValue, ErrorMessage = "Колличество не может быть отрицательным")]
        public int Count { get; set; }

        public BookModel() { }

        public BookModel(int id, string title, string author, string description, int count)
        {
            Id = id;
            Title = title;
            Author = author;
            Description = description;
            Count = count;
        }
    }
}