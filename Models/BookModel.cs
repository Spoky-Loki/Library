using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class BookModel
    {
        public int Id { get; set; }

        [DisplayName("Название")]
        public string Title { get; set; }

        [DisplayName("Автор")]
        public string Author { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Колличестов")]
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