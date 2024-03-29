﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Models;

namespace VacationTracker.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly VacationTracker.Data.ApplicationDbContext _db;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            VacationTracker.Data.ApplicationDbContext db)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Company Name")]
            public string CompanyName { get; set; }
            public string Address { get; set; }
            [Required]
            [Display(Name = "Phone No.")]
            public string PhoneNumber { get; set; }
            [Required]
            public string ContactName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    Company company = new()
                    {
                        Address = Input.Address,
                        CompanyName = Input.CompanyName,
                        ContactEmail = Input.Email,
                        PhoneNumber = Input.PhoneNumber,
                        ContactName = Input.ContactName
                    };

                    var c = _db.Companies.Add(company);
                    await _db.SaveChangesAsync();

                    Employee employee = new()
                    {
                        DisplayName = Input.ContactName,
                        Email = Input.Email,
                        IsDeleted = false,
                        CompanyId = company.Id,
                        StartDate = DateTime.Now,
                        IsAdmin = true,
                        Firstname = "admin",
                        Surname = "admin",
                        JobTitle = "admin"
                    };

                    var e = _db.Employees.Add(employee);
                    await _db.SaveChangesAsync();

                    user.CompanyId = company.Id;
                    await _userManager.UpdateAsync(user);

                    //add user to all roles
                    ApplicationUser newuser = await _userManager.FindByEmailAsync(Input.Email);

                    await _userManager.AddToRoleAsync(newuser, "Admin");
                    await _userManager.AddToRoleAsync(newuser, "Manager");
                    await _userManager.AddToRoleAsync(newuser, "Approver");
                    await _userManager.AddToRoleAsync(newuser, "Employee");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }                
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
