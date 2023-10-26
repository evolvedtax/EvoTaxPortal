using AutoMapper;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace EvolvedTax_Institute.Controllers
{
    public class AccountController : Controller
    {
        #region Fields
        readonly IUserService _userService;
        readonly IGeneralQuestionareService _generalQuestionareService;
        readonly IInstituteService _instituteService;
        readonly IMailService _mailService;
        readonly IWebHostEnvironment _webHostEnvironment;
        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;
        readonly IMapper _mapper;
        private readonly EvolvedtaxContext _evolvedtaxContext;
        #endregion

        #region Ctor
        public AccountController(IUserService userService, IGeneralQuestionareService generalQuestionareService, IInstituteService instituteService, IMailService mailService, EvolvedtaxContext evolvedtaxContext, IMapper mapper = null, IWebHostEnvironment webHostEnvironment = null, UserManager<User> userManager = null, SignInManager<User> signInManager = null)
        {
            _generalQuestionareService = generalQuestionareService;
            _userService = userService;
            _instituteService = instituteService;
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
            //await _userService.AddRoles();
            if (ModelState.IsValid && !IsSuperAdmin)
            {
                //var response = await _userService.Login(userDTO);
                var result = await _signInManager.PasswordSignInAsync(userDTO.UserName, userDTO.Password, false, true);
                if (result.Succeeded)
                {
                    //return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    HttpContext.Session.SetString("EmailId", userDTO.UserName);
                    return RedirectToAction(nameof(Auth), new { returnUrl = returnUrl });
                }
                //if (result.Succeeded)
                //{
                //    HttpContext.Session.SetString("EmailId", response.EmailId);
                //    var bytes = Base32Encoding.ToBytes("JBSWY3DPEHPK3PXP");
                //    var totp = new Totp(bytes);
                //    var otp = totp.ComputeTotp();

                //    await _mailService.SendOTPAsync(otp, response.EmailId, "Action Required: Your One Time Password (OTP) with EvoTax Portal", response.UserName, "");
                //    _userService.UpdateInstituteMasterOTP(response.EmailId, otp, DateTime.Now.AddMinutes(60));

                //    return RedirectToAction(nameof(Auth));
                //}
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus; // Error
            TempData["Message"] = "Username or password is incorrect!";
            return RedirectToAction(nameof(Login));
        }
        #region Signup

        public async Task<IActionResult> SendEmailToInstitueMaster(string e, string? i = null, string? s = null)
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            i = i ?? EncryptionHelper.Decrypt(i.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
            e = EncryptionHelper.Decrypt(e.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));


            var user = _evolvedtaxContext.Users.FirstOrDefault(p => p.Email == e && p.InstituteId == Convert.ToInt32(i));

            if (user == null)
            {
                return NotFound();
            }

            int InstituteId = user.InstituteId;
            string userId = user.Id;

            var Institute = _evolvedtaxContext.InstituteMasters.FirstOrDefault(e => e.InstId == InstituteId);
            string Institutename = Institute.FirstName + " " + Institute.LastName;

            var currentDate = DateTime.Now;


            var expiredEntityUserData = _evolvedtaxContext.EntitiesUsers
                .Where(ue => ue.UserId == user.Id && ue.ExpirySignupDatetime.HasValue && ue.ExpirySignupDatetime.Value < currentDate)
                .ToList();

            foreach (var entityUserData in expiredEntityUserData)
            {
                var entity = _evolvedtaxContext.InstituteEntities.FirstOrDefault(e => e.EntityId == entityUserData.EntityId);
                if (entity != null)
                {
                    string entityName = entity.EntityName;
                    string entityEmail = user.Email;
                    string Role = entityUserData.Role.Trim();
                    string AssignedByUserId = entityUserData.AssignedBy.Trim();
                    var AssignedBy = _evolvedtaxContext.Users.FirstOrDefault(p => p.Id == AssignedByUserId && p.InstituteId == Convert.ToInt32(i));

                    var scheme = HttpContext.Request.Scheme; // "http" or "https"
                    var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)

                    var fullUrl = $"{scheme}://{host}";
                    await _mailService.SendEmailForExpireSignUp(AssignedBy.Email, entityEmail, entityName, Role, entityUserData.EntryDatetime, Institutename, fullUrl, instId);
                }
            }

            var userData = await _userManager.FindByEmailAsync(e);
            if (userData != null)
            {
                // Delete the user
                await _userManager.DeleteAsync(userData);

                // Delete related roles
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var roleName in userRoles)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }

                var entitiesToDelete = _evolvedtaxContext.EntitiesUsers
                  .Where(ue => ue.UserId == user.Id && ue.ExpirySignupDatetime.HasValue && ue.ExpirySignupDatetime.Value < currentDate);
                _evolvedtaxContext.EntitiesUsers.RemoveRange(entitiesToDelete);

                await _evolvedtaxContext.SaveChangesAsync();
            }
            return View("SignupExpiredMessageView");
        }


        public async Task<IActionResult> SignUpForInvite(string e, string? i = null, string? s = null)
        {
            i = i ?? EncryptionHelper.Decrypt(i.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
            e = EncryptionHelper.Decrypt(e.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
            //s = EncryptionHelper.Decrypt(s.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));

            // Check if the link is expired
            var user = await _userManager.FindByEmailAsync(e);
            bool IsExpired = _userService.IsSignupLinkExpired(user.Id);
            if (!IsExpired)
            {
                TempData["Message"] = "Your invite link may have expired. In this case, please contact the application owner for a new invite by clicking below:";
                return View("SignupExpiredView");
            }

            var items = await _evolvedtaxContext.MstrCountries.ToListAsync();
            ViewBag.CountriesList = items.OrderBy(item => item.Favorite != "0" ? int.Parse(item.Favorite) : int.MaxValue)
                                  .ThenBy(item => item.Country).Select(p => new SelectListItem
                                  {
                                      Text = p.Country,
                                      Value = p.Country,
                                  });

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


            ViewBag.EntityType = new List<SelectListItem>{
                new SelectListItem{ Text = AppConstants.IndividualEntityType, Value = AppConstants.IndividualEntityType},
                new SelectListItem{ Text = AppConstants.BusinessEntityType, Value = AppConstants.BusinessEntityType}
            };

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId.ToString()
            });
            var InstituteName = _instituteService.GetInstituteDataById(Convert.ToInt32(i)).InstitutionName;
            if (!string.IsNullOrEmpty(s) && s == "share")
            {
                var model1 = new UserRequest { SUEmailAddress = e, SUInstitutionName = InstituteName ?? "" };
                return View(model1);
            }
            var model = new UserRequest { SUEmailAddress = e, InstId = Convert.ToInt32(i), SUInstitutionName = InstituteName ?? "" };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SignUpForInvite(UserRequest model)
        {
            if (model.InstId == 0)
            {
                var responseFormShare = await _userService.UpdateInvitedUserForShare(model);
                if (responseFormShare.Succeeded)
                {
                    string fullnaame = model.SUFirstName + " " + model.SULastName;
                    string email = model.SUEmailAddress;

                    return Json(new { Status = true, Message = "Please sign in with your account" });
                }
                else
                {
                    return View(new { Status = false, Message = "Something went wrong. Please try again." });
                }
            }
            var responseForm = await _userService.SaveInvitedUser(model);
            if (responseForm.Succeeded)
            {
                var scheme = HttpContext.Request.Scheme; // "http" or "https"
                var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
                string fullnaame = model.SUFirstName + " " + model.SULastName;
                string email = model.SUEmailAddress;

                return Json(new { Status = true, Message = "An email has been sent to administrator for signup request. Once approved, you will be notified by email." });
            }
            else
            {
                return View(new { Status = false, Message = "Something went wrong. Please try again." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> InviteUserForEntity(string role, int EntityId, string EntityName, string emailAddresses)
        {
            List<string> emails = JsonConvert.DeserializeObject<List<string>>(emailAddresses);
            int InstituteId = HttpContext.Session.GetInt32("InstId") ?? 0;
            string AssignedBy = HttpContext.Session.GetString("UserId");
            var instituteName = HttpContext.Session.GetString("InstituteName");

            foreach (var email in emails)
            {
                var responseForm = await _userService.SaveInvitedUserForShare(role, EntityId, email, InstituteId, AssignedBy);
                if (responseForm)
                {
                    var URL = Url.Action("SignUpForInvite", "Account", new { i = "id", e = "email", s = "share" }, Request.Scheme) ?? "";
                    var user = await _userManager.GetUserAsync(User);
                    var invitee = await _userManager.GetUserAsync(User);
                    await _mailService.SendShareInvitaionEmailSignUp(email, URL, InstituteId.ToString(), "Action Required: You have been invited to signup with EvoTax Portal", string.Concat(user.FirstName, " ", user.LastName), instituteName, EntityName, role, InstituteId);
                }
                else
                {
                    var scheme = HttpContext.Request.Scheme; // "http" or "https"
                    var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
                    var fullUrl = $"{scheme}://{host}";
                    var URL = string.Concat(fullUrl, "Account/", "Login");
                    var user = await _userManager.GetUserAsync(User);
                    var invitee = await _userManager.GetUserAsync(User);
                    await _mailService.SendShareInvitaionEmail(email, URL, string.Concat(invitee.FirstName, " ", invitee.LastName), "Action Required: You have been invited to signup with EvoTax Portal", string.Concat(user.FirstName, " ", user.LastName), instituteName, EntityName, role, InstituteId);
                }
            }
            return Json(new { Status = true, Message = "Invited link has been sent." });

            //return View(new { Status = false, Message = "Something went wrong. Please try again." });
        }

        [HttpPost]
        public async Task<IActionResult> InviteUserForEntities(string role, string emailAddresses, string EntityNamesHidden)
        {
            List<string> emails = JsonConvert.DeserializeObject<List<string>>(emailAddresses);
            string[] entityNamePairs = EntityNamesHidden.Split(',');

            foreach (var entityNamePair in entityNamePairs)
            {
                string[] parts = entityNamePair.Split('$');
                if (parts.Length == 2)
                {
                    string entityName = parts[0].Trim();
                    string entityIdStr = parts[1].Trim();
                    if (int.TryParse(entityIdStr, out int entityId))
                    {
                        int InstituteId = HttpContext.Session.GetInt32("InstId") ?? 0;
                        var instituteName = HttpContext.Session.GetString("InstituteName");
                        string AssignedBy = HttpContext.Session.GetString("UserId");

                        foreach (var email in emails)
                        {
                            var responseForm = await _userService.SaveInvitedUserForShare(role, entityId, email, InstituteId, AssignedBy);
                            if (responseForm)
                            {
                                var URL = Url.Action("SignUpForInvite", "Account", new { i = "id", e = "email", s = "share" }, Request.Scheme) ?? "";
                                var user = await _userManager.GetUserAsync(User);
                                var invitee = await _userManager.GetUserAsync(User);
                                await _mailService.SendShareInvitaionEmailSignUp(email, URL, InstituteId.ToString(), "Action Required: You have been invited to signup with EvoTax Portal", string.Concat(user.FirstName, " ", user.LastName), instituteName, entityName, role, InstituteId);
                            }
                            else
                            {
                                var scheme = HttpContext.Request.Scheme; // "http" or "https"
                                var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
                                var fullUrl = $"{scheme}://{host}";
                                var URL = string.Concat(fullUrl, "/Account/", "Login");
                                var user = await _userManager.GetUserAsync(User);
                                var invitee = await _userManager.GetUserAsync(User);
                                await _mailService.SendShareInvitaionEmail(email, URL, string.Concat(invitee.FirstName, " ", invitee.LastName), "Action Required: You have been invited to signup with EvoTax Portal", string.Concat(user.FirstName, " ", user.LastName), instituteName, entityName, role, InstituteId);
                            }
                        }
                    }
                }
            }

            return Json(new { Status = true, Message = "Invited link has been sent." });
        }



        public async Task<IActionResult> SignUp()
        {
            var items = await _evolvedtaxContext.MstrCountries.ToListAsync();
            ViewBag.CountriesList = items.OrderBy(item => item.Favorite != "0" ? int.Parse(item.Favorite) : int.MaxValue)
                                  .ThenBy(item => item.Country).Select(p => new SelectListItem
                                  {
                                      Text = p.Country,
                                      Value = p.Country,
                                  });

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


            ViewBag.EntityType = new List<SelectListItem>{
                new SelectListItem{ Text = AppConstants.IndividualEntityType, Value = AppConstants.IndividualEntityType},
                new SelectListItem{ Text = AppConstants.BusinessEntityType, Value = AppConstants.BusinessEntityType}
            };

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId.ToString()
            });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserRequest model)
        {
            var responseForm = await _userService.Save(model);
            if (responseForm.Succeeded)
            {
                var scheme = HttpContext.Request.Scheme; // "http" or "https"
                var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
                string fullnaame = model.SUFirstName + " " + model.SULastName;
                string email = model.SUEmailAddress;

                var userModel = new User { UserName = model.SUEmailAddress, Email = model.SUEmailAddress };
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
                //var confirmationLink = Url.Action(nameof(Verify), "Account", new { s = token, e = model.SUEmailAddress }, Request.Scheme) ?? "";
                var confirmationLink = Url.Action("Login", "Account", null, Request.Scheme) ?? "";
                //var confirmationLink = Url.Action(nameof(Login), "Account", Request.Scheme) ?? "";
                await _mailService.EmailVerificationAsync(fullnaame, email, "Action Required: Verify Your Account with EvoForms", "", confirmationLink);
                return View("Index");
            }
            else
            {
                return View(model);
            }
        }
        [HttpGet]
        // GET: AccountController/Login
        public async Task<ActionResult> Verify(string s, string e)
        {
            var user = await _userManager.FindByEmailAsync(e);
            if (user == null)
                return View("Error");
            var result = await _userManager.ConfirmEmailAsync(user, s);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }
        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ConfirmEmail()
        {
            return View();
        }
        #region P.O BOX validation
        [HttpGet]
        public IActionResult ValidateAddress(string SUMMAdd1)
        {
            bool isValid = CheckAddressAgainstForbiddenTerms(SUMMAdd1);

            if (isValid)
            {
                return Json(true); // Address is valid
            }

            return Json(false); // Address is not valid
        }
        [HttpGet]
        public IActionResult ValidatePAddress(string SUMPAdd1)
        {
            bool isValid = CheckAddressAgainstForbiddenTerms(SUMPAdd1);

            if (isValid)
            {
                return Json(true); // Address is valid
            }

            return Json(false); // Address is not valid
        }
        private bool CheckAddressAgainstForbiddenTerms(string address)
        {
            List<string> forbiddenTerms = _evolvedtaxContext.MasterPoboxWildcards.Select(w => w.WildCard.ToLower()).ToList();

            //bool containsForbiddenTerm = forbiddenTerms.Any(term => address.ToLower().Contains(term));
            bool containsForbiddenTerm = forbiddenTerms.Any(term => Regex.IsMatch(address.ToLower(), $@"\b{Regex.Escape(term)}(?:[.]|\b|$)"));

            return !containsForbiddenTerm;
        }
        #endregion

        [HttpGet]
        public ActionResult ValidateEmailDomainAddress(string SUEmailAddress)
        {

            try
            {
                string domainEmail = SUEmailAddress.Split('@')[1];
                var emails = _evolvedtaxContext.EmailDomains.Where(e => e.EmailDomain1.ToLower().Contains(domainEmail.ToLower())).ToList();
                if (emails.Any())
                {
                    // return Json(false);
                    string errorMessage = "<span style='color:red;'>Email address is not allowed</span>";
                    return Json(errorMessage);
                }
                bool emailExists = _userService.IsEmailExist(SUEmailAddress);

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
        [HttpGet]
        public async Task<ActionResult> ValidateEmailDomainAddressOnInvitation(string SUEmailAddress, int EntityId)
        {
            try
            {
                string domainEmail = SUEmailAddress.Split('@')[1];
                var emails = _evolvedtaxContext.EmailDomains.Where(e => e.EmailDomain1.ToLower().Contains(domainEmail.ToLower())).ToList();
                if (emails.Any())
                {
                    return Json(false);
                }
                var user = await _userManager.FindByEmailAsync(SUEmailAddress);
                if (user != null)
                {
                    string userIdString = user.Id.ToString();
                    var emailExists = _evolvedtaxContext.EntitiesUsers.Any(p => p.UserId == userIdString && p.EntityId == EntityId);
                    //var emailExists = _evolvedtaxContext.EntitiesUsers.Any(p=>p.UserId.ToString() == user.Id && p.EntityId == EntityId);
                    if (emailExists)
                    {
                        return Json(false);
                    }
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        #endregion
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
            //var bytes = Base32Encoding.ToBytes("JBSWY3DPEHPK3PXP");
            //var totp = new Totp(bytes);
            //var token = totp.ComputeTotp();
            //var response = await _userService.UpdateUsertOTP(user.Id, token, DateTime.Now.AddMinutes(60)); DateTime.Now.AddMinutes(60);
            // for local email otp 
            //user.Email = "niqbal@mailinator.com";
            //if(response)
            //{
                await _mailService.SendOTPAsync(token, user.Email, "Action Required: Your One Time Password (OTP) with EvoForms", user.FirstName + " " + user.LastName, "");
            //}
            
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
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

            //var user = await _userManager.FindByEmailAsync(emailId);
            //if (user.OTP.Trim() == string.Empty || user.OTPExpiryDate < DateTime.Now)
            //{
            //    TempData["Type"] = ResponseMessageConstants.ErrorStatus;
            //    TempData["Message"] = "OTP has expired";
            //    return View(nameof(Auth));
            //}
            //if (Otp.Trim() == user?.OTP.Trim())
            //{
            
            //    var institute = _instituteService.GetInstituteDataById(user.InstituteId);
            //    HttpContext.Session.SetInt32("InstId", user.InstituteId);
            //    HttpContext.Session.SetString("UserName", user.UserName);
            //    HttpContext.Session.SetString("EmailId", user.Email);
            //    HttpContext.Session.SetString("InstituteName", institute?.InstitutionName ?? "");
            //    HttpContext.Session.SetString("ProfileImage", institute?.InstituteLogo ?? "");
            //    HttpContext.Session.SetString("UserId", user.Id);
            //    return RedirectToAction("Index", "Summary");
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
                var institute = _instituteService.GetInstituteDataById(user.InstituteId);
                HttpContext.Session.SetInt32("InstId", user.InstituteId);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("EmailId", user.Email);
                HttpContext.Session.SetString("InstituteName", institute?.InstitutionName ?? "");
                HttpContext.Session.SetString("ProfileImage", institute?.InstituteLogo ?? "");
                HttpContext.Session.SetString("UserId", user.Id);
                return RedirectToAction("Index", "Summary");
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
            TempData["Message"] = "Invalid OTP entered. Please try again.";
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
                await _mailService.SendOTPAsync(otp, s, "Action Required: Your One Time Password (OTP) with EvoForms", string.Concat(userName?.PartnerName1, " ", userName?.PartnerName2), "");
                _userService.UpdateInstituteClientOTP(s, otp, DateTime.Now.AddMinutes(60));
                ViewBag.ClientEmail = s;
                HttpContext.Session.SetString("OTPClientEmail", s);
            }
            return View();
        }
        [HttpPost]
        public IActionResult OTP(IFormCollection formVals)
        {
            string clientEmail;
            if (!string.IsNullOrEmpty(formVals["clientEmail"]))
            {
                clientEmail = formVals["clientEmail"];
            }
            else
            {

                clientEmail = HttpContext.Session.GetString("OTPClientEmail");
            }

            var response = _instituteService.GetClientDataByClientEmailId(clientEmail);
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
                _userService.UpdateInstituteClientOTP(formVals["clientEmail"], "", DateTime.Now);
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
        public IActionResult ForgetPassword(ForgetPasswordRequest request, string email, int PasswordSecuredQ)
        {
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
            var fullUrl = $"{scheme}://{host}";
            request.EmailAddress = email;
            request.PasswordSecuredA1 = request.PasswordSecuredA1 ?? "";
            request.PasswordSecuredA2 = request.PasswordSecuredA2 ?? "";
            request.PasswordSecuredA3 = request.PasswordSecuredA3 ?? "";
            var result = _userService.ValidateSecurityQuestions(request);
            if (result)
            {
                var PasswordResetToken = Guid.NewGuid().ToString().Replace("-", "");
                var PasswordResetTokenExpiration = DateTime.UtcNow.AddMinutes(60);
                var response = _userService.UpdateResetToeknInfo(email, PasswordResetToken, PasswordResetTokenExpiration);
                if (response)
                {
                    string resetUrl = string.Concat(fullUrl, "/Account", "/ResetPassword?token=" + PasswordResetToken);
                    _mailService.SendResetPassword(request.EmailAddress, "Action Required:Your Password Reset Request with EvoTax Portal", resetUrl);
                    return Json(response);
                }
                return Json(response);
            }
            return Json(result);
        }
        [HttpGet]
        public IActionResult UpdateProfile()
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = _instituteService.GetInstituteDataById(instId);
            var items = _evolvedtaxContext.MstrCountries.ToList();
            ViewBag.CountriesList = items.OrderBy(item => item.Favorite != "0" ? int.Parse(item.Favorite) : int.MaxValue)
                                  .ThenBy(item => item.Country).Select(p => new SelectListItem
                                  {
                                      Text = p.Country,
                                      Value = p.Country,
                                  });

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId.ToString()
            });


            var timezones = _evolvedtaxContext.MasterTimezones
                 .OrderBy(p => p.Id)
        .Select(p => new
        {
            Text = string.Format("({0}) {1}", p.GmtOffset, p.TimeZone),
            Value = string.Format("({0}) {1}", p.GmtOffset, p.TimeZone)
        })
        .ToList();

            ViewBag.TimezonList = new SelectList(timezones, "Text", "Value");

            List<string> dateFormats = new List<string>
    {
                             "MM/DD/YYYY",
                            "MM-DD-YYYY",
                            "MM DD YYYY",
                            "MM.DD.YYYY",
                            "DD/MM/YYYY",
                            "DD-MM-YYYY",
                            "DD MM YYYY",
                            "DD.MM.YYYY",
                            "YYYY-MM-DD",
                            "YYYY/MM/DD",
                            "YYYY MM DD",
                            "YYYY.MM.DD",
                            "YYYY-DD-MM",
                            "YYYY/DD/MM",
                            "YYYY DD MM",
                            "YYYY.DD.MM"
    };

            ViewBag.DateFormats = new SelectList(dateFormats);
            return View(_mapper.Map<InstituteMasterRequest>(response));
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(SettingRequest request)
        {
            if (request.InstituteMasterRequest.ProfileImage != null || request.InstituteMasterRequest.ProfileImage?.Length > 0)
            {
                // Create a unique filename to avoid overwriting existing files
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.InstituteMasterRequest.ProfileImage?.FileName);
                var webRootPath = _webHostEnvironment.WebRootPath;
                // Combine the wwwroot path with the desired file path and filename
                var filePath = Path.Combine(webRootPath, "ProfileImage", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.InstituteMasterRequest.ProfileImage?.CopyToAsync(fileStream);
                }
                request.InstituteMasterRequest.InstituteLogo = fileName;
                HttpContext.Session.SetString("ProfileImage", fileName);
            }
            else
            {
                HttpContext.Session.SetString("ProfileImage", "");
            }
            var response = _instituteService.UpdateInstituteMaster(request.InstituteMasterRequest);
            return Json(new { Status = response });
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
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
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


        #region User Access 

        [Route("Account/ChangeAccess")]
        public async Task<IActionResult> ChangeAccess(string Id, string role)
        {
            var entityUser = _evolvedtaxContext.EntitiesUsers.FirstOrDefault(ue => ue.Id == Convert.ToInt32(Id));

            if (entityUser != null)
            {
                if (role == "Remove")
                {
                    _evolvedtaxContext.EntitiesUsers.Remove(entityUser);
                }
                else
                {
                    entityUser.Role = role;
                    _evolvedtaxContext.EntitiesUsers.Update(entityUser);
                }

                await _evolvedtaxContext.SaveChangesAsync();
            }

            return Json(new { type = ResponseMessageConstants.SuccessStatus, message = ResponseMessageConstants.SuccessRoleChange });
        }
        #endregion


        [HttpGet]
        public IActionResult EmailReminder()
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = _instituteService.GetInstituteDataById(instId);
            return View(_mapper.Map<InstituteMasterRequest>(response));
        }
        [HttpPost]
        public IActionResult EmailReminder(SettingRequest request)
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            request.InstituteMasterRequest.InstId = instId;
            var response = _instituteService.SetEmailReminder(request.InstituteMasterRequest);
            return Json(new { Status = response });
        }

        public async Task<IActionResult> GetAlertsNotification()
        {
            //var user = await _userManager.GetUserAsync(User);
            bool IsuperAdmin = User.IsInRole("SuperAdmin");

            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            List<AlertRequest> alerts = _instituteService.GetAlertsNotification(instId, IsuperAdmin);
            return Json(alerts);
        }

        [HttpPost]
        public IActionResult MarkAlertAsRead(int alertId)
        {
            var response = _instituteService.MarkAlertAsRead(alertId);
            return Json(new { success = response });
        }

        public IActionResult MarkAllAlertsAsRead()
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var response = _instituteService.MarkAllAlertsAsRead(instId);
            return Json(new { success = response });
        }

        public IActionResult GetAnnouncements()
        {

            List<AnnouncementRequest> announcement = _instituteService.GetAnnouncements();
            return Json(announcement);
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
