using Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    interface IUserRepository
    {
        Task<UserModel> GetUserById(int id);

        Task<List<UserModel>> GetUsers();

        Task AddUser(UserModel userModel);

        Task UpdateUser(int id, UserModel userModel);

        Task DeleteUser(int id);

        UserModel GetEmptyUser();

        Task<bool> ExistUserWithId(int id);
    }
}
