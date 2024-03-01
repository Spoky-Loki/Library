using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Имя")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Фамилия")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [DisplayName("E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Номер телефона")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DisplayName("Адрес")]
        [DataType(DataType.Text)]
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