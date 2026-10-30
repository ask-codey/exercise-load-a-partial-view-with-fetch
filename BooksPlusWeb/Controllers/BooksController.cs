using System.Text.Json;
using BooksPlusWeb.Data;
using BooksPlusWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksPlusWeb.Controllers;

[Route("books")]
public class BooksController : Controller
{
    private const int PageSize = 12;
    private const string RecentlyViewedKey = "RecentlyViewed";

    private readonly BookDao bookDao;

    public BooksController(BookDao bookDao)
    {
        this.bookDao = bookDao;
    }

    // GET /books, /books?page=2
    [HttpGet("")]
    public IActionResult Index(int page = 1)
    {
        int pageCount = (bookDao.Count() + PageSize - 1) / PageSize;
        if (page < 1 || page > pageCount)
        {
            return NotFound();
        }

        ViewData["Page"] = page;
        ViewData["PageCount"] = pageCount;
        ViewData["RecentlyViewed"] = RecentlyViewedBooks();
        return View(bookDao.GetPage(page, PageSize));
    }

    // GET /books/21
    [HttpGet("{id:int}")]
    public IActionResult Details(int id)
    {
        Book? book = bookDao.GetById(id);
        if (book is null)
        {
            return NotFound();
        }

        RememberViewed(id);
        return View(book);
    }

    // GET /books/search?q=SQL
    [HttpGet("search")]
    public IActionResult Search([FromQuery(Name = "q")] string? text)
    {
        ViewData["SearchText"] = text;
        List<Book> books = string.IsNullOrWhiteSpace(text) ? new List<Book>() : bookDao.SearchByTitle(text);
        return View(books);
    }

    // GET /books/new
    [Authorize(Roles = "Admin")]
    [HttpGet("new")]
    public IActionResult Create()
    {
        ViewData["Categories"] = bookDao.GetCategories();
        return View(new Book { PublishDate = DateTime.Today });
    }

    // POST /books/new
    [Authorize(Roles = "Admin")]
    [HttpPost("new")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Book book)
    {
        if (bookDao.TitleExists(book.Title))
        {
            ModelState.AddModelError(nameof(Book.Title), "There's already a book with that title.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["Categories"] = bookDao.GetCategories();
            return View(book);
        }

        int newId = bookDao.Add(book);
        return RedirectToAction(nameof(Details), new { id = newId });
    }

    private List<int> RecentlyViewedIds()
    {
        string? json = HttpContext.Session.GetString(RecentlyViewedKey);
        return json is null ? new List<int>() : JsonSerializer.Deserialize<List<int>>(json)!;
    }

    private void RememberViewed(int bookId)
    {
        List<int> ids = RecentlyViewedIds();
        ids.Remove(bookId);
        ids.Insert(0, bookId);
        HttpContext.Session.SetString(RecentlyViewedKey, JsonSerializer.Serialize(ids.Take(3).ToList()));
    }

    // The recently viewed books, most recent first.
    private List<Book> RecentlyViewedBooks()
    {
        List<int> ids = RecentlyViewedIds();
        List<Book> books = bookDao.GetByIds(ids);
        List<Book> inOrder = new List<Book>();
        foreach (int id in ids)
        {
            Book? book = books.Find(b => b.BookId == id);
            if (book is not null)
            {
                inOrder.Add(book);
            }
        }
        return inOrder;
    }
}
