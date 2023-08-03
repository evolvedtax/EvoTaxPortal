using AutoMapper;
using EvolvedTax.Business.MailService;
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
    [SessionTimeout]
    public class UserManagementController : BaseController
    {
        #region Fields
        #endregion

        #region Ctor
        readonly IUserService _userService;
        readonly IMailService _mailService;
        readonly IWebHostEnvironment _webHostEnvironment;
        readonly UserManager<User> _userManager;
        readonly RoleManager<IdentityRole> _roleManager;
        readonly IMapper _mapper;
        private readonly EvolvedtaxContext _evolvedtaxContext;

        public UserManagementController(IUserService userService, IMailService mailService, EvolvedtaxContext evolvedtaxContext, IMapper mapper, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _mailService = mailService;
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {
            var model = new UserManagementRequest();
            model.Roles = _roleManager.Roles.Where(p=>p.Name != "SuperAdmin").ToList();
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
            var instId = HttpContext.Session.GetInt32("InstId");

            var URL = Url.Action("SignUp", "Account", new { i ="[id]", e = "[email]" }, Request.Scheme) ?? "";
            _mailService.SendInvitaionEmail(request.InvitationEmailDetails,URL);
            return Json(false);
        }
        #endregion

        #region Utilities

        #endregion
    }
}
