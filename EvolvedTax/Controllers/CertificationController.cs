using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.W8BEN_E_FormService;
using EvolvedTax.Business.Services.W8BenFormService;
using EvolvedTax.Business.Services.W8ECIFormService;
using EvolvedTax.Business.Services.W8EXPFormService;
using EvolvedTax.Business.Services.W8IMYFormService;
using EvolvedTax.Business.Services.W9FormService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static NPOI.SS.Format.CellNumberFormatter;

namespace EvolvedTax.Controllers
{
    [UserSession]
    public class CertificationController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EvolvedtaxContext _evolvedtaxContext;
        private readonly IW9FormService _w9FormService;
        private readonly IW8EXPFormService _w8ExpFormService;
        private readonly IW8IMYFormService _w8IMYFormService;
        private readonly ICommonService _commonService;
        private readonly IInstituteService _instituteService;
        private readonly IW8BenFormService _w8BenFormService;
        private readonly IW8BEN_E_FormService _w8BeneFormService;
        private readonly IW8ECIFormService _w8ECIFormService;

        public CertificationController(IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
            IW9FormService w9FormService, ICommonService commonService, IInstituteService instituteService,
            IW8BenFormService w8BenFormService, IW8ECIFormService w8ECIFormService, IW8EXPFormService w8ExpFormService,
            IW8IMYFormService w8IMYFormService, IW8BEN_E_FormService w8BeneFormService)
        {
            _w9FormService = w9FormService;
            _w8ExpFormService = w8ExpFormService;
            _evolvedtaxContext = evolvedtaxContext;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
            _w8BenFormService = w8BenFormService;
            _w8ECIFormService = w8ECIFormService;
            _w8IMYFormService = w8IMYFormService;
            _w8BeneFormService = w8BeneFormService;
        }
        public IActionResult Index(bool? IsDate)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("FormName")))
            {
                var clientEmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
                if (!string.IsNullOrEmpty(clientEmail))
                {
                    return RedirectToAction("DownloadForm", "Certification", new { clientEmail = clientEmail });
                }
                return RedirectToAction("AccessDenied", "Account", new { statusCode = 401 });
            }
        

            var request = new PdfFormDetailsRequest();
            if (IsDate.HasValue)
            {
                request.IsDate = IsDate.Value;
               
            }
            else
            {
                request.IsDate = false;
            }
            request.FileName = HttpContext.Session.GetString("PdfdFileName") ?? string.Empty;
            request.PrintName = HttpContext.Session.GetString("ClientName") ?? string.Empty;
            string fontFamilyValue = HttpContext.Session.GetString("fontFamilyValue") ?? string.Empty;
            ViewBag.FontFamilyValue = fontFamilyValue;
            string settingSignature = HttpContext.Session.GetString("SettingSignature") ?? string.Empty;
            var model = new List<PdfFormDetailsRequest> {
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_DancingScript_Bold, FontSize = "36px", Text = request.PrintName},
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_Yellowtail_Regular, FontSize = "36px", Text = request.PrintName},
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_VLADIMIR, FontSize = "36px", Text = request.PrintName},
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_SegoeScript, FontSize = "36px", Text = request.PrintName},
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_Sugar_Garden, FontSize = "36px", Text = request.PrintName},
            };
            //if (!string.IsNullOrEmpty(fontFamilyValue) && settingSignature == "True")
            //{
            //    model = model.Where(item => item.FontFamily == fontFamilyValue).ToList();
            //    HttpContext.Session.SetString("SettingSignature", "false");
            //}
            request.ButtonRequests = model;
            request.EntryDate = DateTime.Now;
            request.IsSignaturePasted = false;
            return View(request);
        }

        [HttpPost]
        public IActionResult Index(PdfFormDetailsRequest request)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("FormName")))
            {
                var clientEmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
                if (!string.IsNullOrEmpty(clientEmail))
                {
                    return RedirectToAction("DownloadForm", "Certification", new { clientEmail = clientEmail });
                }
                return RedirectToAction("AccessDenied", "Account", new { statusCode = 401 });
            }
            var buttonRequest = new PdfFormDetailsRequest();


            if (request.FontFamily == null)
            {
                request.FontFamily = AppConstants.F_Family_SegoeScript;
            }
            if (request.FontFamily.Trim() == AppConstants.F_Family_DancingScript_Bold)
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "15";
            }
            else if (request.FontFamily == AppConstants.F_Family_Yellowtail_Regular)
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "15";
            }
            else if (request.FontFamily == AppConstants.F_Family_VLADIMIR)
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "15";
            }
            else if (request.FontFamily == AppConstants.F_Family_SegoeScript)
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "15";
            }
            else
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "15";
            }
            #region Old Fonts
            /*
            if (request.FontFamily == null)
            {
                request.FontFamily = AppConstants.F_Family_PalaceScriptMT;
            }
            if (request.FontFamily.Trim() == AppConstants.F_Family_PalaceScriptMT)
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "19";
            }
            else if (request.FontFamily == AppConstants.F_Family_VladimirScript)
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "15";
            }
            else if (request.FontFamily == AppConstants.F_Family_FrenchScriptMT)
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "17";
            }
            else if (request.FontFamily == AppConstants.F_Family_SegoeScript)
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "12";
            }
            else
            {
                buttonRequest.Text = request.PrintName;
                buttonRequest.FontFamily = request.FontFamily;
                buttonRequest.FontSize = "10";
            } 
            */
            #endregion
            buttonRequest.IsSignaturePasted = true;
            buttonRequest.BaseUrl = HttpContext.Session.GetString("BaseURL") ?? string.Empty;
            buttonRequest.FormName = HttpContext.Session.GetString("FormName") ?? string.Empty;
            buttonRequest.EntityStatus = HttpContext.Session.GetString("EntityStatus") ?? string.Empty;
            buttonRequest.EntryDate = request.EntryDate;
            buttonRequest.FileName = _commonService.AssignSignature(buttonRequest, request.FileName);
            return View(buttonRequest);
        }

        public async Task<IActionResult> Certify(PdfFormDetailsRequest request)
        {
            var oldFile = request.FileName;

            request.FileName = request.FileName.Replace("_temp", "");
            var copyFile = request.FileName;
            var clientEmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
            if (!(request.Agreement1 && request.Agreement2))
            {
                return Json(new { staus = false });
            }
            if (request.FormName == AppConstants.W9Form)
            {
                await _w9FormService.UpdateByClientEmailId(clientEmail, request);
                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
            }
            else if (request.FormName == AppConstants.W8EXPForm)
            {
                await _w8ExpFormService.UpdateByClientEmailId(clientEmail, request);
                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
            }
            else if (request.FormName == AppConstants.W8IMYForm)
            {
                await _w8IMYFormService.UpdateByClientEmailId(clientEmail, request);
                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
            }
            else if (request.FormName == AppConstants.W8BENForm)
            {
                // await _w8BenFormService.UpdateByClientEmailId(clientEmail, request);
                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
            }
            else if (request.FormName == AppConstants.W8ECIForm)
            {
                await _w8ECIFormService.UpdateByClientEmailId(clientEmail, request);
                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
            }
            else if (request.FormName == AppConstants.W8BENEForm)
            {
                await _w8BeneFormService.UpdateByClientEmailId(clientEmail, request);
                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
            }
            else
            {
                return Json(new { staus = false });
            }

            request.FileName = _commonService.RemoveAnnotations(request.FileName);

            var OldFileName = Path.Combine(_webHostEnvironment.WebRootPath, request.FileName);
            var newFileName = Path.Combine(_webHostEnvironment.WebRootPath, request.FileName.Replace("_new", ""));
            request.FileName = request.FileName.Replace("_new", "");
            // Remove "_new" from the file name

            //request.FileName = "output.pdf";
            var EntityName = _instituteService.GetEntityDataByClientEmailId(clientEmail).EntityName;
            var InstituteEmail = _instituteService.GetInstituteDataByClientEmailId(clientEmail).SupportEmail ?? string.Empty;
            HttpContext.Session.SetString("FormName", string.Empty);
            var AuthSigName = HttpContext.Session.GetString("ClientNameSig");
            if (!string.IsNullOrEmpty(AuthSigName))
            {
                request.PrintName = AuthSigName;
            }

            if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, oldFile)))
            {
                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, oldFile));
            }

            if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, copyFile)))
            {
                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, copyFile));
            }
           
            // Rename the file in the folder
            if (System.IO.File.Exists(OldFileName))
            {
                System.IO.File.Move(OldFileName, newFileName);
            }
            return Json(new { fileName = request.FileName, staus = true, entityName = EntityName, printName = request.PrintName, formName = request.FormName, InstituteEmail = InstituteEmail }); ;
        }
        public IActionResult DownloadForm(string clientEmail)
        {
            var entityName = _instituteService.GetEntityDataByClientEmailId(clientEmail).EntityName;
            var clientData = _instituteService.GetClientDataByClientEmailId(clientEmail);
            ViewBag.FileName = clientData.FileName;
            ViewBag.EntityName = entityName;
            ViewBag.FormName = clientData?.FormName;
            ViewBag.InstituteEmail = _instituteService.GetInstituteDataByClientEmailId(clientEmail).EmailAddress ?? string.Empty;
            if (clientData?.FormName?.Trim() == AppConstants.W9Form)
            {
                ViewBag.PrintName = _w9FormService.GetPrintNameByClientEmailId(clientEmail);
            }
            else if (clientData?.FormName?.Trim() == AppConstants.W8BENForm)
            {
                ViewBag.PrintName = _w8BenFormService.GetDataByClientEmailId(clientEmail).NameOfIndividual;
            }
            else if (clientData?.FormName?.Trim() == AppConstants.W8ECIForm)
            {
                ViewBag.PrintName = _w8ECIFormService.GetIndividualDataByClientEmailId(clientEmail).NameOfIndividualW8ECI;
            }
            return View();
        }

        [HttpPost]
        public IActionResult SetFontFamilySession(string fontFamilyValue, bool fromSettingSignature = false)
        {
            HttpContext.Session.SetString("fontFamilyValue", fontFamilyValue);
            return Ok(); 
        }
        public IActionResult GetFontFamilyValue()
        {
            string fontFamilyValue = HttpContext.Session.GetString("fontFamilyValue") ?? string.Empty;
            return Json(new { fontFamilyValue });
        }
    }
}
