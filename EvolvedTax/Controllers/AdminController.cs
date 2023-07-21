using Azure;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.UserService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Common.ExtensionMethods;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace EvolvedTax.Controllers
{
    public class AdminController : BaseController
    {
        #region Fields
        #endregion

        #region Ctor
        readonly IUserService _userService;
        readonly IGeneralQuestionareService _generalQuestionareService;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        private readonly EvolvedtaxContext _evolvedtaxContext;

        public AdminController(IUserService userService, IGeneralQuestionareService generalQuestionareService, IInstituteService instituteService, IMailService mailService, EvolvedtaxContext evolvedtaxContext)
        {
            _generalQuestionareService = generalQuestionareService;
            _userService = userService;
            _instituteService = instituteService;
            _mailService = mailService;
            _evolvedtaxContext = evolvedtaxContext;
        }
        #endregion

        #region Methods
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        // POST: AccountController/Login
        public ActionResult Login(LoginRequest userDTO, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.Login(userDTO);
                if (response.IsLoggedIn && response.IsAdmin)
                {
                    HttpContext.Session.SetString("EmailId", response.EmailId);
                    HttpContext.Session.SetInt32("InstId", response.InstId);
                    HttpContext.Session.SetString("IsAdmin", response.IsAdmin.ToString());
                    return RedirectToAction("Master","Institute");
                }
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus; // Error
            TempData["Message"] = "Username or password is incorrect!";
            return RedirectToAction(nameof(Login));
        }
        [SessionTimeout]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
        public ActionResult AccessDenied(short statusCode)
        {
            ViewBag.StatusCode = statusCode;
            return View();
        }
        #endregion

        #region Utilities
        public IActionResult ValidateEmailAddress(string EmailAddress)
        {
            var result = _evolvedtaxContext.InstituteMasters.Any(p => p.EmailAddress == EmailAddress);
            return Json(result);
        }
        #endregion
    }
}
