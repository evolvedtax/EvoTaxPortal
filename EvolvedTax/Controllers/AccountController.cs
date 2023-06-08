using Azure;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.UserService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Identity;

namespace EvolvedTax.Controllers
{
    public class AccountController : BaseController
    {
        #region Fields
        #endregion

        #region Ctor
        readonly IUserService _userService;
        readonly IGeneralQuestionareService _generalQuestionareService;
        public AccountController(IUserService userService, IGeneralQuestionareService generalQuestionareService)
        {
            _generalQuestionareService = generalQuestionareService;
            _userService = userService;
        }
        #endregion

        #region Methods
        [SessionTimeout]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        // GET: AccountController/Login
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
                if (response.IsLoggedIn)
                {
                    HttpContext.Session.SetInt32("InstId", response.InstId);
                    HttpContext.Session.SetString("UserName", response.UserName);
                    HttpContext.Session.SetString("EmailId", response.EmailId);
                    HttpContext.Session.SetString("InstituteName", response.InstituteName);
                    //return RedirectToAction(nameof(OTP));
                    return RedirectToAction("Entities", "Institute");
                }
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus; // Error
            TempData["Message"] = "Username or password is incorrect!";
            return RedirectToAction(nameof(Login));
        }
        [SessionTimeout]
        public IActionResult OTP(string clientEmail)
        {
            if (_generalQuestionareService.IsClientAlreadyExist(clientEmail))
            {
                return RedirectToAction("DownloadForm","Home", new { clientEmail = clientEmail});
            }
            HttpContext.Session.SetString("ClientEmail", clientEmail);
            return View();
        }
        [SessionTimeout]
        [HttpPost]
        public IActionResult OTP(IFormCollection formVals)
        {
            string Otp = string.Concat(
                formVals["Otp1"].ToString(),
                formVals["Otp2"].ToString(),
                formVals["Otp3"].ToString(),
                formVals["Otp4"].ToString(),
                formVals["Otp5"].ToString(),
                formVals["Otp6"].ToString());
            if (Otp == "123456")
            {
                return RedirectToAction("Index", "Status");
                //return RedirectToAction("Entities", "Institute");
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus;
            TempData["Message"] = "Please enter correct OTP";
            return View(nameof(OTP));
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

        #endregion
    }
}
