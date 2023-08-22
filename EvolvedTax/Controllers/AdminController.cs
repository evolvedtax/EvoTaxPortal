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
using Microsoft.AspNetCore.Identity;
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
        readonly SignInManager<User> _signInManager;
        private readonly EvolvedtaxContext _evolvedtaxContext;

        public AdminController(IUserService userService, IGeneralQuestionareService generalQuestionareService, IInstituteService instituteService, IMailService mailService, EvolvedtaxContext evolvedtaxContext, SignInManager<User> signInManager)
        {
            _generalQuestionareService = generalQuestionareService;
            _userService = userService;
            _instituteService = instituteService;
            _mailService = mailService;
            _evolvedtaxContext = evolvedtaxContext;
            _signInManager = signInManager;
        }
        #endregion

        #region Methods
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        // POST: AccountController/Login
        public async Task<ActionResult> Login(LoginRequest userDTO, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                //var response = await _userService.Login(userDTO);
                var result = await _signInManager.PasswordSignInAsync(userDTO.UserName, userDTO.Password, false, true);
                if (result.Succeeded)
                {
                    //return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor && User.IsInRole("SuperAdmin"))
                {
                    HttpContext.Session.SetString("EmailId", userDTO.UserName);
                    return RedirectToAction("Auth","Account", new { returnUrl = returnUrl });
                }
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus; // Error
            TempData["Message"] = "Username or password is incorrect!";
            return RedirectToAction(nameof(Login));
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
        [AllowAnonymous]
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
