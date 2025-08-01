﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VacationTracker.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ClaimsIdentity GenerateUserIdentity(UserManager<ApplicationUser> manager)
        {
            ClaimsIdentity userIdentity = new ClaimsIdentity("Cookies");
            userIdentity.AddClaim(new Claim("CompanyId", this.CompanyId.ToString()));
            return userIdentity;
        }
        public string ContactName { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSystemAdmin => CompanyId == -1;
    }
}
