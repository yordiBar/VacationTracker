using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Areas.Identity.Interfaces;
using VacationTracker.Data;
using VacationTracker.Interfaces;
using VacationTracker.Repositories;
using VacationTracker.Services;
using VacationTracker.SystemAdmin.Data;
using VacationTracker.SystemAdmin.Services;
using VacationTracker.SystemAdmin.Services.Interfaces;

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
            services.AddDbContext<MasterDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("MasterConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, MyUserClaimsPrincipalFactory>();
            services.AddControllersWithViews();

            services.AddScoped<ILocationRepository>(provider => 
            {
                var companyService = provider.GetService<ICompanyService>();
                if (companyService == null)
                {
                    var dbContext = provider.GetService<ApplicationDbContext>();
                    return new LocationRepository(dbContext);
                }

                var companyId = companyService.GetCurrentUserCompanyId();

                // Use company-specific context if we have a valid company ID (not -1 for system admin)
                if (companyId > 0)
                {
                    var dbContextFactory = provider.GetService<ICompanyDbContextFactory>();
                    if (dbContextFactory != null)
                    {
                        try
                        {
                            var dbContext = dbContextFactory.CreateDbContext(companyId);
                            return new LocationRepository(dbContext);
                        }
                        catch
                        {
                            var dbContext = provider.GetService<ApplicationDbContext>();
                            return new LocationRepository(dbContext);
                        }
                    }
                }

                var defaultDbContext = provider.GetService<ApplicationDbContext>();
                return new LocationRepository(defaultDbContext);
            });
            
            services.AddScoped<IDepartmentRepository>(provider => 
            {
                var companyService = provider.GetService<ICompanyService>();
                if (companyService == null)
                {
                    var dbContext = provider.GetService<ApplicationDbContext>();
                    return new DepartmentRepository(dbContext);
                }

                var companyId = companyService.GetCurrentUserCompanyId();

                // Use company-specific context if we have a valid company ID (not -1 for system admin)
                if (companyId > 0)
                {
                    var dbContextFactory = provider.GetService<ICompanyDbContextFactory>();
                    if (dbContextFactory != null)
                    {
                        try
                        {
                            var dbContext = dbContextFactory.CreateDbContext(companyId);
                            return new DepartmentRepository(dbContext);
                        }
                        catch
                        {
                            var dbContext = provider.GetService<ApplicationDbContext>();
                            return new DepartmentRepository(dbContext);
                        }
                    }
                }

                var defaultDbContext = provider.GetService<ApplicationDbContext>();
                return new DepartmentRepository(defaultDbContext);
            });
            
            services.AddScoped<IGenderRepository>(provider =>
            {
                var companyService = provider.GetService<ICompanyService>();
                if (companyService == null)
                {
                    var dbContext = provider.GetService<ApplicationDbContext>();
                    return new GenderRepository(dbContext);
                }

                var companyId = companyService.GetCurrentUserCompanyId();

                // Use company-specific context if we have a valid company ID (not -1 for system admin)
                if (companyId > 0)
                {
                    var dbContextFactory = provider.GetService<ICompanyDbContextFactory>();
                    if (dbContextFactory != null)
                    {
                        try
                        {
                            var dbContext = dbContextFactory.CreateDbContext(companyId);
                            return new GenderRepository(dbContext);
                        }
                        catch
                        {
                            var dbContext = provider.GetService<ApplicationDbContext>();
                            return new GenderRepository(dbContext);
                        }
                    }
                }

                var defaultDbContext = provider.GetService<ApplicationDbContext>();
                return new GenderRepository(defaultDbContext);
            });
            services.AddScoped<IAllowanceRepository, AllowanceRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();            
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            // SystemAdmin services
            services.AddScoped<ICompanySelectionService, CompanySelectionService>();
            services.AddScoped<ICompanyDbContextFactory, CompanyDbContextFactory>();

            services.AddScoped<ICompanyService, CompanyService>();

            services.AddScoped<ISystemAdminSeedService, SystemAdminSeedService>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddHttpContextAccessor();
            services.AddSingleton<IRoleSeed, RoleSeed>();
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
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20.0);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            // Authorization policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("RequireManagerRole", policy =>
                    policy.RequireRole("Manager"));

                options.AddPolicy("RequireApproverRole", policy =>
                    policy.RequireRole("Approver"));

                options.AddPolicy("RequireEmployeeRole", policy =>
                    policy.RequireRole("Employee"));
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

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            try
            {
                var roleSeed = services.GetRequiredService<IRoleSeed>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                roleSeed.SeedAsync(roleManager).Wait();
                roleSeed.SeedSystemAdminAsync(services).Wait();

                var systemAdminSeedService = services.GetRequiredService<ISystemAdminSeedService>();
                systemAdminSeedService.SeedSystemAdminAsync().Wait();
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger<Startup>>();
                logger.LogError(e, "An error occurred while seeding the roles.");
            }
        }

        public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public MyUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IOptions<IdentityOptions> optionsAssessor)
                : base(userManager, roleManager, optionsAssessor)
            {
                _userManager = userManager;
                _roleManager = roleManager;
            }

            public override async Task<ClaimsPrincipal> CreateAsync(
                ApplicationUser user)
            {
                ClaimsPrincipal principal = await base.CreateAsync(user);
                ((ClaimsIdentity)principal.Identity).AddClaims(new List<Claim>
            {
                new Claim(UserClaims.CompanyId, user.CompanyId.ToString())
            });
                return principal;
            }
        }
    }
}
