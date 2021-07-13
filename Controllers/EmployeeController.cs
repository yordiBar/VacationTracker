﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Models;
using VacationTracker.Areas.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace VacationTracker.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly Data.ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        // Creating a connection to the database
        public EmployeeController(VacationTracker.Data.ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            IEnumerable<Employee> employeeList = _db.Employees.Where(x => x.CompanyId == currentUsersCompanyId);
            return View(employeeList);
        }

        // GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Employee());
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            employee.CompanyId = currentUsersCompanyId;
            _db.Employees.Add(employee);           

            try
            {
                await _db.SaveChangesAsync();
                var user = new ApplicationUser { UserName = employee.Email, Email = employee.Email, CompanyId = employee.CompanyId };
                string password = employee.Password;
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    if (employee.IsAdmin)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                        }
                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Admin");
                        }
                    }
                    if (employee.IsApprover)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Approver"))
                        {
                            await _userManager.AddToRoleAsync(user, "Approver");
                        }

                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, "Approver"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Approver");
                        }
                    }

                    if (employee.IsManager)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Manager"))
                        {
                            await _userManager.AddToRoleAsync(user, "Manager");
                        }

                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, "Manager"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Manager");
                        }
                    }

                    if (!await _userManager.IsInRoleAsync(user, "Employee"))
                    {
                        await _userManager.AddToRoleAsync(user, "Employee");
                    }
                }

                //check if there is an allowance for the current year for the employee if not create

                await CreateAllowanceIfRequired(employee, currentUsersCompanyId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index");

        }

        // A boolean method to check if any employees exist
        private bool EmployeeExists(int id)
        {
            return _db.Employees.Any(e => e.Id == id);
        }

        //check if there is an allowance for the current year for the employee if not create
        private async Task CreateAllowanceIfRequired(Employee emp, int currentUsersCompanyId)
        {
            if (_db.Allowances.Where(x => x.EmployeeId == emp.Id && x.CompanyId == currentUsersCompanyId).Count() == 0)
            {
                _db.Allowances.Add(new Allowance
                {
                    From = new DateTime(2020, 1, 1),
                    To = new DateTime(2020, 12, 31),
                    EmployeeId = emp.Id,
                    CompanyId = currentUsersCompanyId,
                    Amount = 20,
                    CarryOver = 0
                });

                _db.Allowances.Add(new Allowance
                {
                    From = new DateTime(2021, 1, 1),
                    To = new DateTime(2021, 12, 31),
                    EmployeeId = emp.Id,
                    CompanyId = currentUsersCompanyId,
                    Amount = 20,
                    CarryOver = 0
                });

                _db.Allowances.Add(new Allowance
                {
                    From = new DateTime(2022, 1, 1),
                    To = new DateTime(2022, 12, 31),
                    EmployeeId = emp.Id,
                    CompanyId = currentUsersCompanyId,
                    Amount = 20,
                    CarryOver = 0
                });

                await _db.SaveChangesAsync();
            };
        }
    }
}