using System.Diagnostics;
using BooksPlusWeb.Data;
using BooksPlusWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooksPlusWeb.Controllers;

public class HomeController : Controller
{
    private readonly BookDao bookDao;

    public HomeController(BookDao bookDao)
    {
        this.bookDao = bookDao;
    }

    public IActionResult Index()
    {
        List<Book> featured = bookDao.GetFeatured(5);
        return View(featured);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
