using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [DisplayName("Имя")]
        public string FirstName { get; set; }

        [DisplayName("Фамилия")]
        public string LastName { get; set; }

        [DisplayName("E-mail")]
        public string Email { get; set; }

        [DisplayName("Номер телефона")]
        public string PhoneNumber { get; set; }

        [DisplayName("Адрес")]
        public string Address { get; set; }

        public UserModel() { }

        public UserModel(int id, string firstName, string lastName, 
            string email, string phoneNumber, string address)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }
    }
}