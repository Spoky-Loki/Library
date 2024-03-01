using Library.Data.Repository;
using Library.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class BookController : Controller
    {
        private BookRepository bookRepository;

        public BookController()
        {
            bookRepository = BookRepository.GetInstance();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var books = await bookRepository.GetBooks();

            return View(books);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(bookRepository.GetEmptyBook());
        }

        [HttpPost]
        public async Task<ActionResult> Create(BookModel bookModel)
        {
            await bookRepository.AddBook(bookModel);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var book = await bookRepository.GetBookById(id);

            if (book.Title != null)
                return View(book);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, BookModel bookModel)
        {
            await bookRepository.UpdateBook(id, bookModel);

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            await bookRepository.DeleteBook(id);

            return RedirectToAction("Index");
        }
    }
}
