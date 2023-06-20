using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.W8BenFormService;
using EvolvedTax.Business.Services.W8ECIFormService;
using EvolvedTax.Business.Services.W8EXPFormService;
using EvolvedTax.Business.Services.W9FormService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax.Controllers
{
    [UserSession]
    public class CertificationController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EvolvedtaxContext _evolvedtaxContext;
        private readonly IW9FormService _w9FormService;
        private readonly IW8EXPFormService _w8ExpFormService;
        private readonly ICommonService _commonService;
        private readonly IInstituteService _instituteService;
        private readonly IW8BenFormService _w8BenFormService;
        private readonly IW8ECIFormService _w8ECIFormService;

        public CertificationController(IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
            IW9FormService w9FormService, ICommonService commonService, IInstituteService instituteService, 
            IW8BenFormService w8BenFormService, IW8ECIFormService w8ECIFormService, IW8EXPFormService w8ExpFormService)
        {
            _w9FormService = w9FormService;
            _w8ExpFormService = w8ExpFormService;
            _evolvedtaxContext = evolvedtaxContext;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
            _w8BenFormService = w8BenFormService;
            _w8ECIFormService = w8ECIFormService;
        }
        public IActionResult Index()
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
            request.FileName = HttpContext.Session.GetString("PdfdFileName") ?? string.Empty;
            request.PrintName = HttpContext.Session.GetString("ClientName") ?? string.Empty;
            var model = new List<PdfFormDetailsRequest> {
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_PalaceScriptMT, FontSize = "36px", Text = request.PrintName},
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_VladimirScript, FontSize = "36px", Text = request.PrintName},
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_FrenchScriptMT, FontSize = "36px", Text = request.PrintName},
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_SegoeScript, FontSize = "36px", Text = request.PrintName},
                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_BlackadderITC, FontSize = "36px", Text = request.PrintName},
            };
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
            if (request.FormName == AppConstants.W8EXPForm)
            {
                await _w8ExpFormService.UpdateByClientEmailId(clientEmail, request);
            }

            else if (request.FormName == AppConstants.W8BENForm)
            {
                // await _w8BenFormService.UpdateByClientEmailId(clientEmail, request);
                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
            }
            else if (request.FormName == AppConstants.W8ECIForm)
            {
                //await _w8ECIFormService.UpdateByClientEmailId(clientEmail, request);
                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
            }
            else
            {
                return Json(new { staus = false });
            }

            if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, oldFile)))
            {
                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, oldFile));
            }
           // _commonService.RemoveAnnotations(request,request.FileName);
            var EntityName = _instituteService.GetEntityDataByClientEmailId(clientEmail).EntityName;
            var InstituteEmail = _instituteService.GetInstituteDataByClientEmailId(clientEmail).EmailAddress ?? string.Empty;
            HttpContext.Session.SetString("FormName", string.Empty);
            var sd = HttpContext.Session.GetString("ClientNameSig");
            if (string.IsNullOrEmpty(sd))
            {

            }
            else
            {
                request.PrintName = sd;
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
                ViewBag.PrintName = _w8ECIFormService.GetDataByClientEmailId(clientEmail).NameOfIndividualW8ECI;
            }
            return View();
        }
    }
}
