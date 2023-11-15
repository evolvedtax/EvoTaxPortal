using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Business.Services.SignupService;
using EvolvedTax.Business.Services.W8BenFormService;
using EvolvedTax.Business.Services.W8ECIFormService;
using EvolvedTax.Business.Services.W9FormService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;


namespace EvolvedTax_Admin.Controllers
{
    //[SessionTimeout]
    public class TaxInformation : Controller
    {
        readonly IWebHostEnvironment _webHostEnvironment;
        readonly EvolvedtaxContext _evolvedtaxContext;
        readonly ISignupQuestionareService _signupQuestionareService;
        readonly ICommonService _commonService;
        readonly private IMailService _emailService;

        public TaxInformation(IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
        ISignupQuestionareService signupQuestionareService, ICommonService commonService, IMailService emailService)
        {

            _signupQuestionareService = signupQuestionareService;

            _evolvedtaxContext = evolvedtaxContext;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _emailService = emailService;
        }


        public IActionResult Index()
        {


            return View();
        }



        #region TaxInformation Signup
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


            ViewBag.EntityType = _evolvedtaxContext.MasterEntityTypes.Select(p => new SelectListItem
            {
                Text = p.EntityType,
                Value = p.EntityType
            });



            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId.ToString()
            });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(InstituteSignUpFormRequest model)
        {
            string FormName = string.Empty;

            var responseForm = _signupQuestionareService.Save(model);
            if (responseForm == 0)
            {
                return View(model);
            }
            else
            {
                var scheme = HttpContext.Request.Scheme; // "http" or "https"
                var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
                string fullnaame = model.SUFirstName + " " + model.SULastName;
                string email = model.SUEmailAddress;

                var fullUrl = $"{scheme}://{host}";
                //string URL = Path.Combine(fullUrl, "Account", "login");
                string URL = string.Concat(fullUrl, "/Account", "/Verify?s=");
                //string URL = Path.Combine(fullUrl, "Account", "Verify?s=");

                await _emailService.SendEmailToInstituteAsync(fullnaame, email, "Action Required: Verify Your Registration with EvoForms", "", URL);


                //TempData["Type"] = ResponseMessageConstants.SuccessSaved;
                //TempData["MessageSignup"] = "Information has been saved successfully.";

                return View("Index");



            }


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

        //[HttpPost]
        //public ActionResult ValidateEmailDomain(string email)
        //{


        //    try
        //    {
        //        string domainEmail = email.Split('@')[1];
        //        var emails = _evolvedtaxContext.EmailDomains.Where(e => e.EmailDomain1.ToLower().Contains(domainEmail.ToLower())).ToList();
        //        if (emails.Any())
        //        {
        //            return Json(new { isValid = false });
        //        }
        //        // IPHostEntry entry = Dns.GetHostEntry(domain);
        //        return Json(new { isValid = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { isValid = false });
        //    }
        //}

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
                bool emailExists = _evolvedtaxContext.InstituteMasters.Any(i => i.EmailAddress == SUEmailAddress);

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
