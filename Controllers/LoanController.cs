using Library.Data.Repository;
using Library.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class LoanController : Controller
    {
        private LoanRepository loanRepository;
        private BookRepository bookRepository;

        public LoanController()
        {
            loanRepository = LoanRepository.GetInstance();
            bookRepository = BookRepository.GetInstance();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var loans = await loanRepository.GetLoans();

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
            int bookCount = await bookRepository.GetBookCount(loanModel.BookId);
            if (bookCount == 0)
                return RedirectToAction("Index");

            await loanRepository.AddLoan(loanModel);

            await bookRepository.ReduceBookCount(loanModel.BookId);

            return RedirectToAction("Index");
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
