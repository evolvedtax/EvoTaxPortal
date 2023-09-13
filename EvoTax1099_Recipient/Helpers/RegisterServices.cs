using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Data.EFRepository;
using EvolvedTax.Data.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvolvedTax.Helpers
{
    public static class ApplicationServices
    {
        /// <summary>
        /// Method to configure application level services with the di container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment Env)
        {

            services.AddRazorPages();
            services.AddDbContext<EvolvedtaxContext>(options =>
              options.UseSqlServer(
                  configuration.GetConnectionString("EvolvedTaxConnection")));
            services.AddIdentity<User, IdentityRole>(option=> {
                        option.Password.RequiredLength = 7;
                        option.Password.RequireDigit = false;
                        option.Password.RequireUppercase = false;

                        option.User.RequireUniqueEmail = true;

                        option.SignIn.RequireConfirmedEmail = true;
                     })
                    .AddEntityFrameworkStores<EvolvedtaxContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");

            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);//locks out user for half an hour,if u need to unlock a user before declared time, simply simply set LockoutEnabled=false in dbo.AspNetUsers 
                options.Lockout.MaxFailedAccessAttempts = 5;//counts invalid login attempts
                options.Lockout.AllowedForNewUsers = true;
            });

            // configure DI for application services
            services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddHttpContextAccessor();

            services.AddSession(options =>
            {
                // Session settings
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = false;
            });

            //services.AddDevExpressControls();
            services.AddAutoMapper(typeof(AutoMapperProfileConfig));

            #region @@@[------Services]
            services.AddScoped<ITrailAudit1099Service, TrailAudit1099Service>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IInstituteService, InstituteService>();
            #endregion

            #region @@@[------Repository]

            #endregion
        }
    }
}
