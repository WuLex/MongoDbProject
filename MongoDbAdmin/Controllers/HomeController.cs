using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDbAdmin.Models;
using MongoDbAdmin.Services;
using System.Diagnostics;

namespace MongoDbAdmin.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
     
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult ImageIndex()
        {
            return View();
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
}