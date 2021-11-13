using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Models;

namespace VacationTracker.Controllers
{
    public class RequestTypeController : Controller
    {
        private readonly VacationTracker.Data.ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public RequestTypeController(Data.ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        // GET: RequestTypeController
        public ActionResult Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            IEnumerable<RequestType> requestTypeList = _db.RequestTypes.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);
            return View(requestTypeList);
        }

        // GET: RequestTypeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RequestTypeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RequestTypeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RequestTypeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RequestTypeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RequestTypeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RequestTypeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
