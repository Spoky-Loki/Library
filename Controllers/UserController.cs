using Library.Data.Repository;
using Library.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class UserController : Controller
    {
        UserRepository userRepository;

        public UserController()
        {
            userRepository = UserRepository.GetInstance();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var users = await userRepository.GetUsers();

            return View(users);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(userRepository.GetEmptyUser());
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserModel userModel)
        {
            await userRepository.AddUser(userModel);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var user = await userRepository.GetUserById(id);

            if (user.FirstName != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, UserModel userModel)
        {
            await userRepository.UpdateUser(id, userModel);

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            await userRepository.DeleteUser(id);

            return RedirectToAction("Index");
        }
    }
}
