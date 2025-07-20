using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Data;
using VacationTracker.SystemAdmin.Data;
using Company = VacationTracker.SystemAdmin.Models.Company;

namespace VacationTracker.Areas.SystemAdmin.Controllers
{
    [Authorize(Roles = "SystemAdmin")]
    [Area("SystemAdmin")]
    public class AdminController : Controller
    {
        private readonly MasterDbContext _masterDbContext;
        private readonly ILogger<AdminController> _logger;
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration, MasterDbContext masterDbContext, ILogger<AdminController> logger)
        {
            _masterDbContext = masterDbContext;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _masterDbContext.Companies
                .Where(c => c.IsActive)
                .OrderBy(c => c.CompanyName)
                .ToListAsync();

            return View(companies);
        }

        [HttpGet]
        public IActionResult CreateCompany()
        {
            return View(new Company());
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsCompany(int companyId)
        {
            var company = await _masterDbContext.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive);

            if (company == null)
            {
                _logger.LogError("Company not found with ID {CompanyId}", companyId);
                return NotFound();
            }

            // Ensure the company database exists
            try
            {
                await CreateCompanyDatabase(company);
                _logger.LogInformation("Company database verified/created: {DatabaseName}", company.DatabaseName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/verifying company database: {DatabaseName}", company.DatabaseName);
            }

            // Set session to indicate system admin mode for this company
            HttpContext.Session.SetInt32("CurrentCompanyId", companyId);
            HttpContext.Session.SetString("SystemAdminMode", "true");

            _logger.LogInformation("System admin logged into company {CompanyName} (ID: {CompanyId})",
                company.CompanyName, companyId);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(Company company)
        {
            if (!ModelState.IsValid)
                return View(company);

            // Additional validation for database name safety
            if (string.IsNullOrWhiteSpace(company.CompanyName))
            {
                ModelState.AddModelError("CompanyName", "Company name cannot be empty");
                return View(company);
            }

            // Check if company name already exists
            var existingCompany = await _masterDbContext.Companies
                .FirstOrDefaultAsync(c => c.CompanyName.ToLower() == company.CompanyName.ToLower());
            
            if (existingCompany != null)
            {
                ModelState.AddModelError("CompanyName", "A company with this name already exists");
                return View(company);
            }

            // Generate database name using company name
            var sanitizedCompanyName = company.CompanyName
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace(".", "_")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace(";", "")
                .Replace("--", "")
                .Replace("/*", "")
                .Replace("*/", "")
                .ToUpper();
            
            if (!char.IsLetter(sanitizedCompanyName[0]))
            {
                sanitizedCompanyName = "C" + sanitizedCompanyName;
            }
            
            company.DatabaseName = $"VacationTracker_{sanitizedCompanyName}";

            // Create connection string
            var baseConnectionString = _configuration.GetConnectionString("DefaultConnection");
            company.ConnectionString = baseConnectionString.Replace("Database=VacationTracker", $"Database={company.DatabaseName}");

            company.IsActive = true;
            company.CreatedDate = DateTime.UtcNow;

            try
            {
                _masterDbContext.Companies.Add(company);
                await _masterDbContext.SaveChangesAsync();

                // Create the actual database
                await CreateCompanyDatabase(company);

                _logger.LogInformation("Company created successfully: {CompanyName} with database: {DatabaseName}", 
                    company.CompanyName, company.DatabaseName);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company: {CompanyName}", company.CompanyName);
                ModelState.AddModelError("", "An error occurred while creating the company. Please try again.");
                return View(company);
            }
        }

        [HttpGet]
        public IActionResult ExitCompanyMode()
        {
            // Clear the session variables to exit company mode
            HttpContext.Session.Remove("CurrentCompanyId");
            HttpContext.Session.Remove("SystemAdminMode");

            _logger.LogInformation("System admin exited company mode");

            return RedirectToAction("Index", "Admin", new { area = "SystemAdmin" });
        }

        private async Task CreateCompanyDatabase(Company company)
        {
            // Create new database with migrations
            using var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(company.ConnectionString)
                    .Options);

            await context.Database.MigrateAsync();
        }
    }
} 