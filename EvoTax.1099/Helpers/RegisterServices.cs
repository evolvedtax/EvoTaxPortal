using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Business.Services.FormReport;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.SessionProfileUser;
using EvolvedTax.Business.Services.SignupService;
using EvolvedTax.Business.Services.UserService;
using EvolvedTax.Business.Services.W8BEN_E_FormService;
using EvolvedTax.Business.Services.W8BenFormService;
using EvolvedTax.Business.Services.W8ECIFormService;
using EvolvedTax.Business.Services.W8EXPFormService;
using EvolvedTax.Business.Services.W8IMYFormService;
using EvolvedTax.Business.Services.W9FormService;

using EvolvedTax.Data.EFRepository;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Web.Middlewares;
using Microsoft.AspNetCore.Identity;
using NPOI.SS.Formula.Functions;

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
            services.AddScoped<UserSessionProfileService>();
            services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomClaimsPrincipalFactory>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGeneralQuestionareService, GeneralQuestionareService>();
            services.AddScoped<IGeneralQuestionareEntityService, GeneralQuestionareEntityService>();
            services.AddScoped<IW9FormService, W9FormService>();
            services.AddScoped<IW8BenFormService, W8BenFormService>();
            services.AddScoped<IW8ECIFormService, W8ECIFormService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IInstituteService, InstituteService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ISignupQuestionareService, SignupQuestionareService>();
            services.AddScoped<IW8EXPFormService, W8EXPFormService>();
            services.AddScoped<IW8IMYFormService, W8IMYFormService>();
            services.AddScoped<IW8BEN_E_FormService, W8BEN_E_FormService>();
            services.AddScoped<IFormReportService, FormReportService>();
            services.AddScoped<IAnnouncementService, AnnouncementService>();
            services.AddScoped<IForm1099_MISC_Service, Form1099_MISC_Service>();
            services.AddScoped<IForm1099_NEC_Service, Form1099_NEC_Service>();
            services.AddScoped<IForm1099_INT_Service, Form1099_INT_Service>();
            services.AddScoped<IForm1099_DIV_Service, Form1099_DIV_Service>();
            services.AddScoped<IForm1099_LS_Service, Form1099_LS_Service>();
            services.AddScoped<IForm1099_B_Service, Form1099_B_Service>();
            services.AddScoped<IForm1099_A_Service, Form1099_A_Service>();
            services.AddScoped<IForm1099_C_Service, Form1099_C_Service>();
            services.AddScoped<IForm1099_CAP_Service, Form1099_CAP_Service>();
            services.AddScoped<IForm1099_G_Service, Form1099_G_Service>();
            services.AddScoped<IForm1099_LTC_Service, Form1099_LTC_Service>();
            services.AddScoped<IForm1099_PATR_Service, Form1099_PATR_Service>();
            services.AddScoped<IForm1099_R_Service, Form1099_R_Service>();
            services.AddScoped<IForm1099_SA_Service, Form1099_SA_Service>();
            services.AddScoped<IForm1099_SB_Service, Form1099_SB_Service>();
            services.AddScoped<ITrailAudit1099Service, TrailAudit1099Service>();
            #endregion

            #region @@@[------Repository]

            #endregion
        }
    }
}
