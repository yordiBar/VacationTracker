using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using VacationTracker.Models;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Employee,SystemAdmin")]
    public class HomeController : Controller
    {
        #region Fields
        private readonly Data.ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;
        #endregion

        #region Constructors
        public HomeController(Data.ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }
        #endregion

        #region Actions
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult MyRequests()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Content("HomeController.Test() reached successfully!");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("Error action called");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}
