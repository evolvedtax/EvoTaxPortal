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
    public class AccountController : BaseController
    {
        #region Fields
        #endregion

        #region Ctor
        readonly IUserService _userService;
        readonly IGeneralQuestionareService _generalQuestionareService;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        private readonly EvolvedtaxContext _evolvedtaxContext;

        public AccountController(IUserService userService, IGeneralQuestionareService generalQuestionareService, IInstituteService instituteService, IMailService mailService, EvolvedtaxContext evolvedtaxContext)
        {
            _generalQuestionareService = generalQuestionareService;
            _userService = userService;
            _instituteService = instituteService;
            _mailService = mailService;
            _evolvedtaxContext = evolvedtaxContext;
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
                    HttpContext.Session.SetString("EmailId", response.EmailId);
                    var bytes = Base32Encoding.ToBytes("JBSWY3DPEHPK3PXP");
                    var totp = new Totp(bytes);
                    var otp = totp.ComputeTotp();

                    _mailService.SendOTPAsync(otp, response.EmailId, "Action Required: Your One Time Password (OTP) with EvoTax Portal", response.UserName, "");
                    _userService.UpdateInstituteMasterOTP(response.EmailId, otp, DateTime.Now.AddMinutes(60));

                    return RedirectToAction(nameof(Auth));
                }
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus; // Error
            TempData["Message"] = "Username or password is incorrect!";
            return RedirectToAction(nameof(Login));
        }
        public IActionResult Auth()
        {
            var emailId = HttpContext.Session.GetString("EmailId");
            if (string.IsNullOrEmpty(emailId))
            {
                return RedirectToAction(nameof(Login));
            }
            return View();
        }
        [HttpPost]
        public IActionResult Auth(IFormCollection formVals)
        {
            var emailId = HttpContext.Session.GetString("EmailId");
            var response = _userService.GetUserbyEmailId(emailId ?? "");
            string Otp = string.Concat(
                formVals["Otp1"].ToString(),
                formVals["Otp2"].ToString(),
                formVals["Otp3"].ToString(),
                formVals["Otp4"].ToString(),
                formVals["Otp5"].ToString(),
                formVals["Otp6"].ToString());
            if (response.OTP == "")
            {
                TempData["Type"] = ResponseMessageConstants.ErrorStatus;
                TempData["Message"] = "OTP has expired";
                return View(nameof(Auth));
            }
            if (Otp == response.OTP)
            {
                HttpContext.Session.SetInt32("InstId", response.InstId);
                HttpContext.Session.SetString("UserName", response.UserName);
                HttpContext.Session.SetString("EmailId", response.EmailId);
                HttpContext.Session.SetString("InstituteName", response.InstituteName);
                _userService.UpdateInstituteMasterOTP(response.EmailId, "", DateTime.Now);
                return RedirectToAction("Entities", "Institute");
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus;
            TempData["Message"] = "Please enter correct OTP";
            return View(nameof(Auth));
        }
        public async Task<IActionResult> OTP(string? s = "", string e = "")
        {
            if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(s))
            {
                s = EncryptionHelper.Decrypt(s.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
                e = EncryptionHelper.Decrypt(e.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
                if (await _instituteService.CheckIfClientRecordExist(s, e))
                {
                    TempData["EntityName"] = _instituteService.GetEntityDataByClientEmailId(s).EntityName;
                    TempData["InstituteEmail"] = _instituteService.GetInstituteDataByClientEmailId(s).EmailAddress;
                    return RedirectToAction("AccessDenied", new { statusCode = 400 });
                }
                HttpContext.Session.SetString("ClientEmail", s);
                var bytes = Base32Encoding.ToBytes("JBSWY3DPEHPK3PXP");
                var totp = new Totp(bytes);
                var otp = totp.ComputeTotp();
                var userName = _instituteService.GetClientDataByClientEmailId(s);
                await _mailService.SendOTPAsync(otp, s, "Action Required: Your One Time Password (OTP) with EvoTax Portal", string.Concat(userName?.PartnerName1, " ", userName?.PartnerName2), "");
                _userService.UpdateInstituteClientOTP(s, otp, DateTime.Now.AddMinutes(60));
                ViewBag.ClientEmail = s;
            }
            return View();
        }
        [HttpPost]
        public IActionResult OTP(IFormCollection formVals)
        {
            var response = _instituteService.GetClientDataByClientEmailId(formVals["clientEmail"]);
            string Otp = string.Concat(
                formVals["Otp1"].ToString(),
                formVals["Otp2"].ToString(),
                formVals["Otp3"].ToString(),
                formVals["Otp4"].ToString(),
                formVals["Otp5"].ToString(),
                formVals["Otp6"].ToString());
            if (response.Otp == "")
            {
                TempData["Type"] = ResponseMessageConstants.ErrorStatus;
                TempData["Message"] = "OTP has expired";
                return View(nameof(OTP));
            }
            if (Otp == response?.Otp)
            {
                HttpContext.Session.SetString("ClientEmail", formVals["clientEmail"]);
                return RedirectToAction("Index", "Status");
                //return RedirectToAction("Entities", "Institute");
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus;
            TempData["Message"] = "Please enter correct OTP";
            return View(nameof(OTP));
        }
        [HttpPost]
        public IActionResult SecurityInformation(ForgetPasswordRequest request)
        {
            var result = _userService.GetSecurityQuestionsByInstituteEmail(request.EmailAddress);
            return Json(result);
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            ViewBag.SecuredQ1 = _evolvedtaxContext.PasswordSecurityQuestions.Select(p => new SelectListItem
            {
                Text = p.SecurityQuestion,
                Value = p.PasswordSecurityQuestionId.ToString(),
            });

            ViewBag.SecuredQ2 = _evolvedtaxContext.PasswordSecurityQuestions.Select(p => new SelectListItem
            {
                Text = p.SecurityQuestion,
                Value = p.PasswordSecurityQuestionId.ToString(),
            });

            ViewBag.SecuredQ3 = _evolvedtaxContext.PasswordSecurityQuestions.Select(p => new SelectListItem
            {
                Text = p.SecurityQuestion,
                Value = p.PasswordSecurityQuestionId.ToString(),
            });
            return View();
        }
        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordRequest request, string email)
        {
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
            var fullUrl = $"{scheme}://{host}";
            request.EmailAddress = email;
            var result = _userService.ValidateSecurityQuestions(request);
            if (result)
            {
                var PasswordResetToken = Guid.NewGuid().ToString().Replace("-", "");
                var PasswordResetTokenExpiration = DateTime.UtcNow.AddMinutes(60);
                var response = _userService.UpdateResetToeknInfo(email, PasswordResetToken, PasswordResetTokenExpiration);
                if (response)
                {
                    string resetUrl = Path.Combine(fullUrl, "Account", "ResetPassword?token=" + PasswordResetToken);
                    _mailService.SendResetPassword(request.EmailAddress, "Action Required:Your Password Reset Request with EvoTax Portal", resetUrl);
                    return Json(response);
                }
                return Json(response);
            }
            return Json(result);
        }
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            // Verify the token and check if it's still valid
            var user = _evolvedtaxContext.InstituteMasters.FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpiryTime > DateTime.Now);

            if (user == null)
            {
                // Token is invalid or expired
                return BadRequest("Invalid or expired token.");
            }

            // Render the password reset page where the user can enter a new password
            return View(new ForgetPasswordRequest { ResetToken = token });
        }
        [HttpPost]
        public IActionResult ResetPassword(ForgetPasswordRequest request)
        {
            var result = _userService.ResetPassword(request);
            return Json(result);
        }
        [SessionTimeout]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
        [UserSession]
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
        public IActionResult ValidateEmailAddress(string EmailAddress)
        {
            var result = _evolvedtaxContext.InstituteMasters.Any(p => p.EmailAddress == EmailAddress);
            return Json(result);
        }
        #endregion
    }
}
