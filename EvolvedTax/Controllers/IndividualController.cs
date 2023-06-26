using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.W8BenFormService;
using EvolvedTax.Business.Services.W8ECIFormService;
using EvolvedTax.Business.Services.W9FormService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax.Controllers
{
    [UserSession]
    public class IndividualController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EvolvedtaxContext _evolvedtaxContext;
        private readonly IW9FormService _w9FormService;
        private readonly IGeneralQuestionareService _generalQuestionareService;
        private readonly IW8BenFormService _w8BenFormService;
        private readonly IW8ECIFormService _w8ECIFormService;
        private readonly ICommonService _commonService;
        private readonly IInstituteService _instituteService;


        public IndividualController(IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
            IW9FormService w9FormService, IGeneralQuestionareService generalQuestionareService, IW8BenFormService w8BenFormService, IW8ECIFormService w8ECIFormService, ICommonService commonService, IInstituteService instituteService)
        {
            _w8ECIFormService = w8ECIFormService;
            _w8BenFormService = w8BenFormService;
            _generalQuestionareService = generalQuestionareService;
            _w9FormService = w9FormService;
            _evolvedtaxContext = evolvedtaxContext;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GQIndividual()
        {
            string clientEmail = HttpContext.Session.GetString("ClientEmail") ?? "";
            if (_instituteService.GetClientDataByClientEmailId(clientEmail)?.ClientStatus == AppConstants.ClientStatusFormSubmitted)
            {
                return RedirectToAction("DownloadForm", "Certification", new { clientEmail = clientEmail });
            }
            var GQIndividulResponse = _generalQuestionareService.GetDataByClientEmail(clientEmail);
            if (GQIndividulResponse == null)
            {
                var clientData = _instituteService.GetClientDataByClientEmailId(clientEmail);
                if (clientData != null)
                {
                    GQIndividulResponse = new FormRequest();

                    GQIndividulResponse.GQFirstName = clientData?.PartnerName1 ?? "";
                    GQIndividulResponse.GQLastName = clientData?.PartnerName2 ?? "";
                    GQIndividulResponse.MAddress1 = clientData?.Address1 ?? "";
                    GQIndividulResponse.MAddress2 = clientData?.Address2 ?? "";
                    GQIndividulResponse.MCountry = clientData?.Country ?? "";
                    GQIndividulResponse.MCity = clientData?.City ?? "";
                    GQIndividulResponse.MState = clientData?.State ?? "";
                    GQIndividulResponse.MProvince = clientData?.Province ?? "";
                    GQIndividulResponse.MZipCode = clientData?.Zip ?? "";
                }
            }
            var items = await _evolvedtaxContext.MstrCountries.ToListAsync();
            ViewBag.CountriesList = items.OrderBy(item => item.Favorite != "0" ? int.Parse(item.Favorite) : int.MaxValue)
                                  .ThenBy(item => item.Country).Select(p => new SelectListItem
                                  {
                                      Text = p.Country,
                                      Value = p.Country,
                                  });

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId
            });
            var formName = HttpContext.Session.GetString("FormName") ?? string.Empty;
            if (!string.IsNullOrEmpty(formName))
            {
                var clientEmmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
                if (!string.IsNullOrEmpty(clientEmmail))
                {
                    if (formName == AppConstants.W9Form)
                    {
                        GQIndividulResponse = _w9FormService.GetDataForIndividualByClientEmailId(clientEmmail);
                        GQIndividulResponse.FormType = formName;
                    }
                    else
                    {
                        GQIndividulResponse.FormType = AppConstants.W8Form;
                        if (formName == AppConstants.W8BENForm)
                        {
                            GQIndividulResponse.W8FormType = formName;
                            GQIndividulResponse = _w8BenFormService.GetDataByClientEmailId(clientEmmail);
                        }
                        else if (formName == AppConstants.W8ECIForm)
                        {
                            GQIndividulResponse.W8FormType = formName;
                            GQIndividulResponse = _w8ECIFormService.GetDataByClientEmailId(clientEmmail);
                        }
                    }
                    return View(GQIndividulResponse);
                }
            }
            return View(GQIndividulResponse);
        }
        [HttpPost]
        public IActionResult GQIndividual(FormRequest model)
        {
            string FormName = string.Empty;
            string filePathResponse = string.Empty;

            model.IndividualOrEntityStatus = AppConstants.IndividualStatus;
            model.EmailId = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
            model.UserName = model.EmailId;//HttpContext.Session.GetString("UserName") ?? string.Empty;
            model.BasePath = _webHostEnvironment.WebRootPath;

            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
            var fullUrl = $"{scheme}://{host}";
            model.Host = fullUrl;

            FormName = HttpContext.Session.GetString("FormName") ?? string.Empty;
            if (!string.IsNullOrEmpty(FormName))
            {
                #region Edit
                // Removing old form data
                if (FormName != model.FormType)
                {
                    //if (FormName !) { 

                    //}
                }

                if (model.FormType == AppConstants.W9Form) // For W9 form
                {
                    model.USCitizen = "1";
                    FormName = model.FormType;
                    model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W9TemplateFileName);
                    var response = _w9FormService.UpdateForIndividual(model);
                    //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
                    filePathResponse = response;
                }
                else if (model.FormType == AppConstants.W8Form)
                {
                    model.USCitizen = "0";
                    if (model.W8FormType == AppConstants.W8BENForm)
                    {
                        model.US1 = "1";
                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8BENTemplateFileName);
                        filePathResponse = _w8BenFormService.Update(model);
                    }
                    else if (model.W8FormType == AppConstants.W8ECIForm)
                    {
                        model.US1 = "2";
                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8ECITemplateFileName);
                        filePathResponse = _w8ECIFormService.UpdateForIndividual(model);
                    }
                    FormName = model.W8FormType;
                }
                var responseGQForm = _generalQuestionareService.Update(model);
                if (responseGQForm == 0)
                {
                    return View(model);
                }
                #endregion
            }
            else
            {
                #region ADD

                if (model.FormType == AppConstants.W9Form) // For W9 form
                {
                    model.USCitizen = "1";
                    FormName = model.FormType;
                    model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W9TemplateFileName);
                    var response = _w9FormService.SaveForIndividual(model);
                    //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
                    HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
                    filePathResponse = response;
                }
                else if (model.FormType == AppConstants.W8Form)
                {
                    model.USCitizen = "0";
                    if (model.W8FormType == AppConstants.W8BENForm)
                    {
                        model.US1 = "1";
                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8BENTemplateFileName);
                        if (model.W8BENOnBehalfName)
                        {
                            HttpContext.Session.SetString("ClientName", model.PrintNameOfSigner ?? string.Empty);
                            HttpContext.Session.SetString("ClientNameSig", string.Concat(model.GQFirstName, " ", model.GQLastName));

                            //HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
                        }
                        else
                        {
                            HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
                            model.PrintNameOfSigner = string.Concat(model.GQFirstName, " ", model.GQLastName);
                        }
                        filePathResponse = _w8BenFormService.Save(model);
                    }
                    else if (model.W8FormType == AppConstants.W8ECIForm)
                    {
                        model.US1 = "2";
                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8ECITemplateFileName);
                        if (model.W8ECIOnBehalfName)
                        {
                            HttpContext.Session.SetString("ClientName", model.PrintNameOfSignerW8ECI ?? string.Empty);
                            HttpContext.Session.SetString("ClientNameSig", string.Concat(model.GQFirstName, " ", model.GQLastName));
                            //HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
                        }
                        else
                        {
                            HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
                            model.PrintNameOfSignerW8ECI = string.Concat(model.GQFirstName, " ", model.GQLastName);
                        }
                        filePathResponse = _w8ECIFormService.SaveForIndividual(model);
                    }
                    FormName = model.W8FormType;
                }
                var responseGQForm = _generalQuestionareService.Save(model);
                if (responseGQForm == 0)
                {
                    return View(model);
                }
                #endregion
            }
            //byte[] pdfBytes = System.IO.File.ReadAllBytes(filePathResponse);
            //MemoryStream memoryStream = new MemoryStream(pdfBytes);
            //return new FileStreamResult(memoryStream, "application/pdf");
            HttpContext.Session.SetString("PdfdFileName", filePathResponse);
            HttpContext.Session.SetString("BaseURL", _webHostEnvironment.WebRootPath);
            HttpContext.Session.SetString("FormName", FormName);
            HttpContext.Session.SetString("EntityStatus", AppConstants.IndividualStatus);
            return Json(filePathResponse);
        }
    }
}