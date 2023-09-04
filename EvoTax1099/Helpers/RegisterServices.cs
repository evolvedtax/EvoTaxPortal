using EvolvedTax.Business.Services.UserService;
using EvolvedTax1099.Business.MailService;
using EvolvedTax1099.Data.Models.Entities;
using EvolvedTax1099.Web.Middlewares;
using Microsoft.AspNetCore.Identity;

namespace EvolvedTax1099.Helpers
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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomClaimsPrincipalFactory>();
            #endregion

            #region @@@[------Repository]

            #endregion
        }
    }
}
