using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity;
using VacationTracker.Data;
using VacationTracker.Areas.Identity.Data;

namespace VacationTracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, MyUserClaimsPrincipalFactory>();
            services.AddControllersWithViews();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20.0);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
            //added
            services.ConfigureApplicationCookie(options => {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20.0);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            try
            {
                CreateRoles(services).Wait();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            // create RoleManager and UserManager Interfaces
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult roleResult;

            //here in this line we are adding SystemAdmin Role
            var adminRole = await RoleManager.RoleExistsAsync("Admin");
            if (!adminRole)
            {
                //here in this line we are creating admin role and seed it to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create a manager role
            var managerRole = await RoleManager.RoleExistsAsync("Manager");
            if (!managerRole)
            {
                //here in this line we are creating Manager role and seed it to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Manager"));
            }

            // Create an approver role
            var approverRole = await RoleManager.RoleExistsAsync("Approver");
            if (!approverRole)
            {
                //here in this line we are creating Approver role and seed it to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Approver"));
            }

            var employeeRole = await RoleManager.RoleExistsAsync("Employee");
            if (!employeeRole)
            {
                //here in this line we are creating Employee role and seed it to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Employee"));
            }
        }        
    }
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public MyUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAssessor)
            : base(userManager, roleManager, optionsAssessor)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(
            ApplicationUser user)
        {
            ClaimsPrincipal principal = await base.CreateAsync(user);
            ((ClaimsIdentity)principal.Identity).AddClaims((IEnumerable<Claim>)new Claim[1]
        {
                new Claim("CompanyId", user.CompanyId.ToString())
        });
            return principal;
        }
    }
}
