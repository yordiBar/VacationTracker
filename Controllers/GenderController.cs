using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Interfaces;
using VacationTracker.Models;
using VacationTracker.Models.DTO;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Admin,SystemAdmin")]
    public class GenderController : Controller
    {
        #region Fields
        private readonly IGenderRepository _genderRepository;
        private readonly ICompanyService _companyService;
        private readonly ILogger _logger = Log.ForContext<GenderController>();
        #endregion

        #region Constructors
        public GenderController(IGenderRepository genderRepository, ICompanyService companyService)
        {
            _genderRepository = genderRepository;
            _companyService = companyService;
        }
        #endregion

        #region Actions

        public async Task<IActionResult> Index()
        {
            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();
            if (currentUsersCompanyId == 0 && !_companyService.IsSystemAdmin())
            {
                _logger.Error("User does not have a valid company ID");
                return Unauthorized("You do not have access to any company data.");
            }

            IEnumerable<Gender> genderList = await _genderRepository.GetGendersByCompanyIdAsync(currentUsersCompanyId);
            var genderDTO = genderList.Select(gender => new GenderDetailsDTO
            {
                Id = gender.Id,
                GenderName = gender.Name,
                CompanyId = gender.CompanyId
            });
            return View(genderDTO);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.Error("Details method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            var companyForRepo = new Company { Id = currentUsersCompanyId };
            Gender gender = await _genderRepository.GetGenderByIdAndCompanyIdAsync(id.Value, companyForRepo);

            if (gender == null)
            {
                _logger.Error("Gender not found with ID {GenderId}", id);
                return NotFound();
            }

            var genderDto = new GenderDetailsDTO
            {
                Id = gender.Id,
                GenderName = gender.Name,
                CompanyId = gender.CompanyId
            };

            return View(genderDto);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Gender());
        }

        // HttpPost method to create genders
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gender gender)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while creating gender");
                return View(gender);
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            gender.CompanyId = currentUsersCompanyId;

            await _genderRepository.AddGenderAsync(gender);
            _logger.Information("Gender created with ID {GenderId}", gender.Id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.Error("Edit method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            var companyForRepo = new Company { Id = currentUsersCompanyId };
            Gender gender = await _genderRepository.GetGenderByIdAndCompanyIdAsync(id.Value, companyForRepo);

            if (gender == null)
            {
                _logger.Error("Gender not found with ID {GenderId}", id);
                return NotFound();
            }
            return View(gender);
        }

        // HttpPost method to edit genders
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Gender gender)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while editing gender with ID {GenderId}", gender.Id);
                return View(gender);
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();
            gender.CompanyId = currentUsersCompanyId;

            await _genderRepository.UpdateGenderAsync(gender);
            _logger.Information("Gender updated with ID {GenderId}", gender.Id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.Error("Delete method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            var companyForRepo = new Company { Id = currentUsersCompanyId };
            Gender gender = await _genderRepository.GetGenderByIdAndCompanyIdAsync(id.Value, companyForRepo);

            if (gender == null)
            {
                _logger.Error("Gender not found with ID {GenderId}", id);
                return NotFound();
            }
            return View(gender);
        }

        // HttpPost method to delete genders
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Gender gender)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while deleting gender with ID {GenderId}", gender.Id);
                return View(gender);
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();
            gender.CompanyId = currentUsersCompanyId;

            await _genderRepository.DeleteGenderAsync(gender);
            _logger.Information("Gender deleted with ID {GenderId}", gender.Id);

            return RedirectToAction("Index");
        }
        #endregion
    }
}
