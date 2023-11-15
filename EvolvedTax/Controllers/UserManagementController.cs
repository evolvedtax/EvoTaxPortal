using AutoMapper;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.UserService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Common.ExtensionMethods;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using NuGet.Common;
using System.Text.RegularExpressions;

namespace EvolvedTax.Controllers
{
    public class UserManagementController : BaseController
    {
        #region Fields
        #endregion

        #region Ctor
        readonly IUserService _userService;
        readonly IMailService _mailService;
        readonly IInstituteService _instituteService;
        readonly IWebHostEnvironment _webHostEnvironment;
        readonly UserManager<User> _userManager;
        readonly RoleManager<IdentityRole> _roleManager;
        readonly IMapper _mapper;
        private readonly EvolvedtaxContext _evolvedtaxContext;

        public UserManagementController(IUserService userService, IMailService mailService, EvolvedtaxContext evolvedtaxContext, IMapper mapper, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IInstituteService instituteService)
        {
            _userService = userService;
            _mailService = mailService;
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
            _instituteService = instituteService;
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;

            var model = new UserManagementRequest();
            model.Roles = _roleManager.Roles.Where(p => p.Name != "SuperAdmin").ToList();
            ViewBag.Roles = model.Roles.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id,
            }).ToList();
            model.InstituteEntities = _instituteService.GetEntitiesByInstId(instId).ToList();
            model.Users = _userManager.Users.Where(p => p.InstituteId == instId);
            return View(model);
        }
        [HttpPost]
        public IActionResult AddEmail()
        {
            return PartialView("_invitaionEmail");
        }
        [HttpPost]
        public IActionResult SendInvitaionLink(UserManagementRequest request)
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var instituteName = HttpContext.Session.GetString("InstituteName");
            var URL = Url.Action("SignUpForInvite", "Account", new { i = "id", e = "email" }, Request.Scheme) ?? "";
            _mailService.SendInvitaionEmail(request.InvitationEmailDetails, URL, instId, "Action Required: You have been invited to signup with EvoForms", instituteName);
            return Json(new { Status = true });
        }
        #endregion

        #region Utilities
        [HttpGet]
        public ActionResult ValidateEmailDomainAddress(string InvitaionEmail)
        {

            try
            {
                string domainEmail = InvitaionEmail.Split('@')[1];
                var emails = _evolvedtaxContext.EmailDomains.Where(e => e.EmailDomain1.ToLower().Contains(domainEmail.ToLower())).ToList();
                if (emails.Any())
                {
                    // return Json(false);
                    string errorMessage = "<span style='color:red;'>Email address is not allowed</span>";
                    return Json(errorMessage);
                }
                bool emailExists = _userService.IsEmailExist(InvitaionEmail);

                if (emailExists)
                {
                    string errorMessage = "<span style='color:red;'>Email already exists</span>";
                    return Json(errorMessage);

                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        #endregion
    }
}
