using AutoMapper;
using Azure;
using EvolvedTax.Business.Services.UserService;
using EvolvedTax1099.Business.MailService;
using EvolvedTax1099.Common.Constants;
using EvolvedTax1099.Data.Models.DTOs.Request;
using EvolvedTax1099.Data.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace EvolvedTax_Institute.Controllers
{
    public class AccountController : Controller
    {
        #region Fields
        readonly IUserService _userService;
        readonly IMailService _mailService;
        readonly IWebHostEnvironment _webHostEnvironment;
        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;
        readonly IMapper _mapper;
        private readonly EvolvedtaxContext _evolvedtaxContext;
        #endregion

        #region Ctor
        public AccountController(IUserService userService, IMailService mailService, EvolvedtaxContext evolvedtaxContext, IMapper mapper = null, IWebHostEnvironment webHostEnvironment = null, UserManager<User> userManager = null, SignInManager<User> signInManager = null)
        {
            _userService = userService;
            _mailService = mailService;
            _evolvedtaxContext = evolvedtaxContext;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Methods
        [Authorize]
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
        public async Task<ActionResult> Login(LoginRequest userDTO, string? returnUrl = null)
        {
            var user = await _userManager.FindByNameAsync(userDTO.UserName);
            var IsSuperAdmin = true;
            if (user != null)
            {
                IsSuperAdmin = user.IsSuperAdmin;
            }
            if (ModelState.IsValid && !IsSuperAdmin)
            {
                var result = await _signInManager.PasswordSignInAsync(userDTO.UserName, userDTO.Password, false, true);
                if (result.Succeeded)
                {
                    //return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    HttpContext.Session.SetString("EmailId", userDTO.UserName);
                    return RedirectToAction("Auth", new { returnUrl = returnUrl });
                }
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus; // Error
            TempData["Message"] = "Username or password is incorrect!";
            return RedirectToAction(nameof(Login));
        }
        [Authorize]
        public async Task<IActionResult> Auth(string? returnUrl = null)
        {
            var emailId = HttpContext.Session.GetString("EmailId");
            //if (string.IsNullOrEmpty(emailId))
            //{
            //    return RedirectToAction(nameof(Login));
            //}
            var user = await _userManager.FindByEmailAsync(emailId);
            if (user == null)
            {
                return View(nameof(Error));
            }
            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
            {
                return View(nameof(Error));
            }
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _mailService.SendOTPAsync(token, user.Email, "Action Required: Your One Time Password (OTP) with EvoTax Portal", user.FirstName + " " + user.LastName, "");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Auth(IFormCollection formVals)
        {
            var emailId = HttpContext.Session.GetString("EmailId");
            //var response = _userService.GetUserbyEmailId(emailId ?? "");
            string Otp = string.Concat(
                formVals["Otp1"].ToString(),
                formVals["Otp2"].ToString(),
                formVals["Otp3"].ToString(),
                formVals["Otp4"].ToString(),
                formVals["Otp5"].ToString(),
                formVals["Otp6"].ToString());
            //if (response.OTP == "")
            //{
            //    TempData["Type"] = ResponseMessageConstants.ErrorStatus;
            //    TempData["Message"] = "OTP has expired";
            //    return View(nameof(Auth));
            //}
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                TempData["Type"] = ResponseMessageConstants.ErrorStatus;
                TempData["Message"] = "OTP has expired";
                return View(nameof(Auth));
            }
            var result = await _signInManager.TwoFactorSignInAsync("Email", Otp, false, rememberClient: false);
            if (result.Succeeded)
            {
                HttpContext.Session.SetInt32("InstId", user.InstituteId);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("EmailId", user.Email);
                HttpContext.Session.SetString("UserId", user.Id);
                return RedirectToAction("Index", "Dashboard");
            }
            else if (result.IsLockedOut)
            {
                //Same logic as in the Login action
                ModelState.AddModelError("", "The account is locked out");
                TempData["Type"] = ResponseMessageConstants.ErrorStatus;
                TempData["Message"] = "The account is locked out";
                return View();
            }
            //if (Otp == response.OTP)
            //{
            //var institute = _instituteService.GetInstituteDataById(user.InstituteId);
            //    HttpContext.Session.SetInt32("InstId", user.InstituteId);
            //    HttpContext.Session.SetString("UserName", user.UserName);
            //    HttpContext.Session.SetString("EmailId", user.Email);
            //    HttpContext.Session.SetString("InstituteName", institute.InstitutionName);
            //    HttpContext.Session.SetString("ProfileImage", institute.InstituteLogo ?? "");
            //    _userService.UpdateInstituteMasterOTP(response.EmailId, "", DateTime.Now);
            //    return RedirectToAction("Index", "Dashboard");
            //}
            TempData["Type"] = ResponseMessageConstants.ErrorStatus;
            TempData["Message"] = "Please enter correct OTP";
            return View(nameof(Auth));
        }
        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        public ActionResult LogoutUser()
        {
            HttpContext.Session.Clear();
            // HttpContext.Session.Remove("FormName");
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
