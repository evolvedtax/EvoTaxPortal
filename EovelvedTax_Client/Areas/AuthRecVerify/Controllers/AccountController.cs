using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.Form1099Services;
using EvolvedTax.Common.Constants;
using EvolvedTax.Common.ExtensionMehtods;
using EvolvedTax.Common.ExtensionMethods;
using EvolvedTax.Data.Models.DTOs;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Data.Models.Entities._1099;
using EvolvedTax.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using static EvolvedTax.Data.Models.Entities.VerifyModel;

namespace EvolvedTax1099_Recipient.Controllers
{
    [Area("AuthRecVerify")]
    public class AccountController : Controller
    {
        #region Fields
        readonly ITrailAudit1099Service _trailAudit1099Service;
        private readonly IWebHostEnvironment _webHostEnvironment;
        readonly IMailService _mailService;
        #endregion

        #region Ctor
        public AccountController(ITrailAudit1099Service trailAudit1099Service, IMailService mailService, IWebHostEnvironment webHostEnvironment)
        {
            _trailAudit1099Service = trailAudit1099Service;
            _mailService = mailService;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OTP(string? s = "", string e = "", string f = "", string i = "")
        {
            if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(s))
            {
                s = EncryptionHelper.Decrypt(s.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
                e = EncryptionHelper.Decrypt(e.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
                f = EncryptionHelper.Decrypt(f.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
                i = EncryptionHelper.Decrypt(i.Replace(' ', '+').Replace('-', '+').Replace('_', '/'));
                if (await _trailAudit1099Service.CheckIfRecipientRecordExist(s, e))
                {
                    return RedirectToAction("AccessDenied", new { statusCode = 400 });
                }
                HttpContext.Session.SetString("RecipientEmail", s);
                HttpContext.Session.SetString("InstituteId", i);
                var bytes = Base32Encoding.ToBytes("JBSWY3DPEHPK3PXP");
                var totp = new Totp(bytes);
                var otp = totp.ComputeTotp();
                var res = GetClientIP();
                var request = new AuditTrail1099
                {
                    OTP = otp,
                    RecipientEmail = s,
                    Timestamp = DateTime.Now,
                    Description = res,
                    OTPExpiryTime = DateTime.Now.AddMinutes(60),
                    Token = e,
                    FormName = f
                };
                await _trailAudit1099Service.AddUpdateRecipientAuditDetails(request);
                await _mailService.SendOTPToRecipientAsync(otp, s, "Action Required: Your One Time Password (OTP) with EvoTax Portal", "User",Convert.ToInt32(i));
                ViewBag.RecipientEmail = s;
                ViewBag.FormName = f;
                HttpContext.Session.SetString("OTPRecipientEmail", s);
                HttpContext.Session.SetString("OTPFormName", f);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> OTP(IFormCollection formVals)
        {
            string RecipientEmail;
            if (!string.IsNullOrEmpty(formVals["RecipientEmail"]))
            {
                RecipientEmail = formVals["RecipientEmail"];
            }
            else
            {

                RecipientEmail = HttpContext.Session.GetString("OTPRecipientEmail");
            }
            var formName = HttpContext.Session.GetString("OTPFormName");
            var response = _trailAudit1099Service.GetRecipientDataByEmailId(RecipientEmail);
            string Otp = string.Concat(
                formVals["Otp1"].ToString(),
                formVals["Otp2"].ToString(),
                formVals["Otp3"].ToString(),
                formVals["Otp4"].ToString(),
                formVals["Otp5"].ToString(),
                formVals["Otp6"].ToString());
            if (response.OTP.Trim() == string.Empty || response.OTPExpiryTime < DateTime.Now)
            {
                TempData["Type"] = ResponseMessageConstants.ErrorStatus;
                TempData["Message"] = "OTP has expired";
                return View(nameof(OTP));
            }
            if (Otp.Trim() == response?.OTP.Trim())
            {
                var request = new AuditTrail1099 { RecipientEmail = RecipientEmail, FormName = formName, OTPExpiryTime = DateTime.Now, OTP = string.Empty };
                await _trailAudit1099Service.UpdateOTPStatus(request);
                HttpContext.Session.SetString("RecipientEmail", RecipientEmail);
                return RedirectToAction("Verify", "Account");
                //return RedirectToAction("Entities", "Institute");
            }
            TempData["Type"] = ResponseMessageConstants.ErrorStatus;
            TempData["Message"] = "Please enter correct OTP";
            return View(nameof(OTP));
        }
        [RecipientSession]
        [HttpGet]
        public IActionResult Verify()
        {
            var RecipientEmail = HttpContext.Session.GetString("RecipientEmail");
            var result = new VerifyModel
            {
                Items = _trailAudit1099Service.GetRecipientStatusListByEmailId(RecipientEmail).Select(p => new CheckboxItem
                {
                    FormName = p.FormName,
                    IsSelected = false,
                    Rcp_Email = RecipientEmail
                }).ToList()
            };
            return View(result);
        }

        [RecipientSession]
        public async Task<IActionResult> Verify(VerifyModel model)
        {
            var formName = HttpContext.Session.GetString("OTPFormName");
            var rcpEmail = HttpContext.Session.GetString("RecipientEmail");
            var InstituteId = HttpContext.Session.GetString("InstituteId");
            //var request = model.Items.Select(p => new RcpElecAcptnceStatus
            //{
            //    Rcp_Email = rcpEmail,
            //    FormName = p.FormName,
            //    Status = p.IsSelected ? 1 : 2
            //}).ToList();

            var request = model.Items.Select(p => new RcpElecAcptnceStatus
            {
                Rcp_Email = rcpEmail,
                FormName = p.FormName,
                Status = p.Action == "Accept" ? 1 : 2 // Set status to 1 for "Accept" and 2 for "Reject"
            }).ToList();

            var response = await _trailAudit1099Service.UpdateRcpElecAcptnceStatusStatus(request);
            string jsonString = response.Description;
            IpInfo? ipInfo = JsonConvert.DeserializeObject<IpInfo>(jsonString);

            var pdfContent = await _mailService.SendConfirmationEmailToRecipient(ipInfo, response.RecipientEmail, "Electronic Acceptance Confirmation", model);
            var path = string.Concat(_webHostEnvironment.WebRootPath, "/ElecAccepEmailsInPdf", "/", response.RecipientEmail, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".pdf");
            AppCommonMethods.GeneratePdfFromHtml(path, pdfContent); // creating pdf from html
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(ResponseMessage));
        }

        public IActionResult ResponseMessage()
        {
            return View();
        }

        #endregion

        #region Utilities
        public static string GetClientIP()
        {
            string result = "";

            // Create the URI for the IP lookup service
            Uri uri_val = new Uri("http://ip-api.com/json/?fields=61439");

            // Create a web request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri_val);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
            request.Method = WebRequestMethods.Http.Get;

            // Get the response from the server
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Read the response data
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
        #endregion
    }
}
