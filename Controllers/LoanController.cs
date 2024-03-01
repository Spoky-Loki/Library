using Library.Data.Repository;
using Library.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class LoanController : Controller
    {
        private LoanRepository loanRepository;
        private BookRepository bookRepository;
        private UserRepository userRepository;

        public LoanController()
        {
            loanRepository = LoanRepository.GetInstance();
            bookRepository = BookRepository.GetInstance();
            userRepository = UserRepository.GetInstance();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var loans = await loanRepository.GetLoans();

            return View(loans);
        }

        [HttpGet]
        public async Task<ActionResult> History()
        {
            var loans = await loanRepository.GetReturned();

            return View(loans);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(loanRepository.GetEmptyLoan());
        }

        [HttpPost]
        public async Task<ActionResult> Create(LoanCreateViewModel loanModel)
        {
            if (ModelState.IsValid)
            {
                if (!await bookRepository.ExistBookWithId(loanModel.BookId)) 
                {
                    ModelState.AddModelError("", "Книги с таким индификатором не существует");
                    if (!await userRepository.ExistUserWithId(loanModel.UserId))
                    {
                        ModelState.AddModelError("", "Пользователя с таким индификатором не существует");
                    }
                    return View(loanModel);
                }

                int bookCount = await bookRepository.GetBookCount(loanModel.BookId);
                if (bookCount == 0)
                    return RedirectToAction("Index");

                await loanRepository.AddLoan(loanModel);

                await bookRepository.ReduceBookCount(loanModel.BookId);

                return RedirectToAction("Index");
            }

            return View(loanModel);
        }

        [HttpGet]
        public async Task<ActionResult> ReturnBook(int id)
        {
            var isReturned = await loanRepository.IsReturned(id);
            if (isReturned)
                return RedirectToAction("Index");

            await loanRepository.SetReturnDate(id);

            var bookId = await loanRepository.GetBookId(id);

            await bookRepository.AddBookCount(bookId);

            return RedirectToAction("Index");
        }
    }
}
