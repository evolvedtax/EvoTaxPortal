//using System.Text;
//using EvolvedTax.Business.Services.CommonService;
//using EvolvedTax.Business.Services.GeneralQuestionareService;
//using EvolvedTax.Business.Services.InstituteService;
//using EvolvedTax.Business.Services.W8BenFormService;
//using EvolvedTax.Business.Services.W8ECIFormService;
//using EvolvedTax.Business.Services.W9FormService;
//using EvolvedTax.Common.Constants;
//using EvolvedTax.Data.EFRepository;
//using EvolvedTax.Data.Models.DTOs.Request;
//using EvolvedTax.Data.Models.Entities;
//using EvolvedTax.Helpers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.DotNet.Scaffolding.Shared.Project;
//using PdfSharpCore;
//using PdfSharpCore.Pdf;
//using PdfSharpCore.Pdf.IO;

//namespace EvolvedTax.Controllers
//{
//    [SessionTimeout]
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;
//        private readonly IWebHostEnvironment _webHostEnvironment;
//        private readonly EvolvedtaxContext _evolvedtaxContext;
//        private readonly IW9FormService _w9FormService;
//        private readonly IGeneralQuestionareService _generalQuestionareService;
//        private readonly IW8BenFormService _w8BenFormService;
//        private readonly IW8ECIFormService _w8ECIFormService;
//        private readonly ICommonService _commonService;
//        private readonly IInstituteService _instituteService;


//        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
//            IW9FormService w9FormService, IGeneralQuestionareService generalQuestionareService, IW8BenFormService w8BenFormService, IW8ECIFormService w8ECIFormService, ICommonService commonService, IInstituteService instituteService)
//        {
//            _w8ECIFormService = w8ECIFormService;
//            _w8BenFormService = w8BenFormService;
//            _generalQuestionareService = generalQuestionareService;
//            _w9FormService = w9FormService;
//            _evolvedtaxContext = evolvedtaxContext;
//            _webHostEnvironment = webHostEnvironment;
//            _logger = logger;
//            _commonService = commonService;
//            _instituteService = instituteService;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }
//        public IActionResult Entity()
//        {
//            return View();
//        }

//        public async Task<IActionResult> GQIndividual()
//        {
//            var items = await _evolvedtaxContext.MstrCountries.ToListAsync();
//            ViewBag.CountriesList = items.OrderBy(item => item.Favorite != "0" ? int.Parse(item.Favorite) : int.MaxValue)
//                                  .ThenBy(item => item.Country).Select(p => new SelectListItem
//                                  {
//                                      Text = p.Country,
//                                      Value = p.Country,
//                                  });

//            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
//            {
//                Text = p.StateId,
//                Value = p.StateId
//            });
//            var formName = HttpContext.Session.GetString("FormName") ?? string.Empty;
//            if (!string.IsNullOrEmpty(formName))
//            {
//                var model = new FormRequest();
//                var clientEmmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//                if (!string.IsNullOrEmpty(clientEmmail))
//                {
//                    if (formName == AppConstants.W9Form)
//                    {
//                        model = _w9FormService.GetDataForIndividualByClientEmailId(clientEmmail);
//                        model.FormType = formName;
//                    }
//                    else
//                    {
//                        model.FormType = AppConstants.W8Form;
//                        if (formName == AppConstants.W8BENForm)
//                        {
//                            model.W8FormType = formName;
//                            model = _w8BenFormService.GetDataByClientEmailId(clientEmmail);
//                        }
//                        else if (formName == AppConstants.W8ECIForm)
//                        {
//                            model.W8FormType = formName;
//                            model = _w8ECIFormService.GetDataByClientEmailId(clientEmmail);
//                        }
//                    }
//                    return View(model);
//                }
//            }
//            return View();
//        }
//        [HttpPost]
//        public IActionResult GQIndividual(FormRequest model)
//        {
//            string FormName = string.Empty;
//            string filePathResponse = string.Empty;

//            model.EmailId = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//            model.UserName = model.EmailId;//HttpContext.Session.GetString("UserName") ?? string.Empty;
//            model.BasePath = _webHostEnvironment.WebRootPath;

//            var scheme = HttpContext.Request.Scheme; // "http" or "https"
//            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
//            var fullUrl = $"{scheme}://{host}";
//            model.Host = fullUrl;

//            FormName = HttpContext.Session.GetString("FormName") ?? string.Empty;
//            if (!string.IsNullOrEmpty(FormName))
//            {
//                #region Edit
//                // Removing old form data
//                if (FormName != model.FormType)
//                {
//                    //if (FormName !) { 

//                    //}
//                }

//                if (model.FormType == AppConstants.W9Form) // For W9 form
//                {
//                    model.USCitizen = "1";
//                    FormName = model.FormType;
//                    model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W9TemplateFileName);
//                    var response = _w9FormService.UpdateForIndividual(model);
//                    //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
//                    filePathResponse = response;
//                }
//                else if (model.FormType == AppConstants.W8Form)
//                {
//                    model.USCitizen = "0";
//                    if (model.W8FormType == AppConstants.W8BENForm)
//                    {
//                        model.US1 = "1";
//                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8BENTemplateFileName);
//                        filePathResponse = _w8BenFormService.Update(model);
//                    }
//                    else if (model.W8FormType == AppConstants.W8ECIForm)
//                    {
//                        model.US1 = "2";
//                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8ECITemplateFileName);
//                        filePathResponse = _w8ECIFormService.Update(model);
//                    }
//                    FormName = model.W8FormType;
//                }
//                var responseGQForm = _generalQuestionareService.Update(model);
//                if (responseGQForm == 0)
//                {
//                    return View(model);
//                }
//                #endregion
//            }
//            else
//            {
//                #region ADD

//                if (model.FormType == AppConstants.W9Form) // For W9 form
//                {
//                    model.USCitizen = "1";
//                    FormName = model.FormType;
//                    model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W9TemplateFileName);
//                    var response = _w9FormService.SaveForIndividual(model);
//                    //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
//                    HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
//                    filePathResponse = response;
//                }
//                else if (model.FormType == AppConstants.W8Form)
//                {
//                    model.USCitizen = "0";
//                    if (model.W8FormType == AppConstants.W8BENForm)
//                    {
//                        model.US1 = "1";
//                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8BENTemplateFileName);
//                        if (model.W8BENOnBehalfName)
//                        {
//                            HttpContext.Session.SetString("ClientName", model.PrintNameOfSigner ?? string.Empty);
//                        }
//                        else
//                        {
//                            HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
//                            model.PrintNameOfSigner = string.Concat(model.GQFirstName, " ", model.GQLastName);
//                        }
//                        filePathResponse = _w8BenFormService.Save(model);
//                    }
//                    else if (model.W8FormType == AppConstants.W8ECIForm)
//                    {
//                        model.US1 = "2";
//                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8ECITemplateFileName);
//                        if (model.W8ECIOnBehalfName)
//                        {
//                            HttpContext.Session.SetString("ClientName", model.PrintNameOfSignerW8ECI ?? string.Empty);
//                        }
//                        else
//                        {
//                            HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
//                            model.PrintNameOfSignerW8ECI = string.Concat(model.GQFirstName, " ", model.GQLastName);
//                        }
//                        filePathResponse = _w8ECIFormService.Save(model);
//                    }
//                    FormName = model.W8FormType;
//                }
//                var responseGQForm = _generalQuestionareService.Save(model);
//                if (responseGQForm == 0)
//                {
//                    return View(model);
//                }
//                #endregion
//            }
//            //byte[] pdfBytes = System.IO.File.ReadAllBytes(filePathResponse);
//            //MemoryStream memoryStream = new MemoryStream(pdfBytes);
//            //return new FileStreamResult(memoryStream, "application/pdf");
//            HttpContext.Session.SetString("PdfdFileName", filePathResponse);
//            HttpContext.Session.SetString("BaseURL", _webHostEnvironment.WebRootPath);
//            HttpContext.Session.SetString("FormName", FormName);
//            return Json(filePathResponse);
//        }
//        public IActionResult Certification()
//        {
//            if (string.IsNullOrEmpty(HttpContext.Session.GetString("FormName")))
//            {
//                var clientEmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//                if (!string.IsNullOrEmpty(clientEmail))
//                {
//                    return RedirectToAction("DownloadForm", "Home", new { clientEmail = clientEmail });
//                }
//                return RedirectToAction("AccessDenied", "Account", new { statusCode = 401 });
//            }
//            var request = new PdfFormDetailsRequest();
//            request.FileName = HttpContext.Session.GetString("PdfdFileName") ?? string.Empty;
//            request.PrintName = HttpContext.Session.GetString("ClientName") ?? string.Empty;
//            var model = new List<PdfFormDetailsRequest> {
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_PalaceScriptMT, FontSize = "36px", Text = request.PrintName},
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_VladimirScript, FontSize = "36px", Text = request.PrintName},
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_FrenchScriptMT, FontSize = "36px", Text = request.PrintName},
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_SegoeScript, FontSize = "36px", Text = request.PrintName},
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_BlackadderITC, FontSize = "36px", Text = request.PrintName},
//            };
//            request.ButtonRequests = model;
//            request.EntryDate = DateTime.Now;
//            request.IsSignaturePasted = false;
//            return View(request);
//        }
//        [HttpPost]
//        public IActionResult Certification(PdfFormDetailsRequest request)
//        {
//            if (string.IsNullOrEmpty(HttpContext.Session.GetString("FormName")))
//            {
//                var clientEmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//                if (!string.IsNullOrEmpty(clientEmail))
//                {
//                    return RedirectToAction("DownloadForm", "Home", new { clientEmail = clientEmail });
//                }
//                return RedirectToAction("AccessDenied", "Account", new { statusCode = 401 });
//            }
//            var buttonRequest = new PdfFormDetailsRequest();
//            if (request.FontFamily.Trim() == AppConstants.F_Family_PalaceScriptMT)
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "19";
//            }
//            else if (request.FontFamily == AppConstants.F_Family_VladimirScript)
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "15";
//            }
//            else if (request.FontFamily == AppConstants.F_Family_FrenchScriptMT)
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "17";
//            }
//            else if (request.FontFamily == AppConstants.F_Family_SegoeScript)
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "12";
//            }
//            else
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "10";
//            }
//            buttonRequest.IsSignaturePasted = true;
//            buttonRequest.BaseUrl = HttpContext.Session.GetString("BaseURL") ?? string.Empty;
//            buttonRequest.FormName = HttpContext.Session.GetString("FormName") ?? string.Empty; ;
//            buttonRequest.EntryDate = request.EntryDate;
//            buttonRequest.FileName = _commonService.AssignSignature(buttonRequest, request.FileName);
//            return View(buttonRequest);
//        }

//        public async Task<IActionResult> Certify(PdfFormDetailsRequest request)
//        {
//            var oldFile = request.FileName;
//            request.FileName = request.FileName.Replace("_temp", "");
//            var clientEmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//            if (!(request.Agreement1 && request.Agreement2))
//            {
//                return Json(new { staus = false });
//            }
//            if (request.FormName == AppConstants.W9Form)
//            {
//                await _w9FormService.UpdateByClientEmailId(clientEmail, request);
//                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
//            }
//            else if (request.FormName == AppConstants.W8BENForm)
//            {
//                await _w8BenFormService.UpdateByClientEmailId(clientEmail, request);
//                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
//            }
//            else if (request.FormName == AppConstants.W8ECIForm)
//            {
//                await _w8ECIFormService.UpdateByClientEmailId(clientEmail, request);
//                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
//            }
//            else
//            {
//                return Json(new { staus = false });
//            }

//            if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, oldFile)))
//            {
//                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, oldFile));
//            }
//            var EntityName = _instituteService.GetEntityDataByClientEmailId(clientEmail).EntityName;
//            var InstituteEmail = _instituteService.GetInstituteDataByClientEmailId(clientEmail).EmailAddress ?? string.Empty;
//            HttpContext.Session.SetString("FormName", string.Empty);
//            return Json(new { fileName = request.FileName, staus = true, entityName = EntityName, printName = request.PrintName, formName = request.FormName, InstituteEmail = InstituteEmail }); ;
//        }
//        public IActionResult DownloadForm(string clientEmail)
//        {
//            var entityName = _instituteService.GetEntityDataByClientEmailId(clientEmail).EntityName;
//            var clientData = _instituteService.GetClientDataByClientEmailId(clientEmail);
//            ViewBag.FileName = clientData.FileName;
//            ViewBag.EntityName = entityName;
//            ViewBag.FormName = clientData?.FormName;
//            ViewBag.InstituteEmail = _instituteService.GetInstituteDataByClientEmailId(clientEmail).EmailAddress ?? string.Empty;
//            if (clientData?.FormName?.Trim() == AppConstants.W9Form)
//            {
//                ViewBag.PrintName = _w9FormService.GetDataForIndividualByClientEmailId(clientEmail).W9PrintName;
//            }
//            else if (clientData?.FormName?.Trim() == AppConstants.W8BENForm)
//            {
//                ViewBag.PrintName = _w8BenFormService.GetDataByClientEmailId(clientEmail).NameOfIndividual;
//            }
//            else if (clientData?.FormName?.Trim() == AppConstants.W8ECIForm)
//            {
//                ViewBag.PrintName = _w8ECIFormService.GetDataByClientEmailId(clientEmail).NameOfIndividual;
//            }
//            return View();
//        }
        
//    }
//}//using System.Text;
//using EvolvedTax.Business.Services.CommonService;
//using EvolvedTax.Business.Services.GeneralQuestionareService;
//using EvolvedTax.Business.Services.InstituteService;
//using EvolvedTax.Business.Services.W8BenFormService;
//using EvolvedTax.Business.Services.W8ECIFormService;
//using EvolvedTax.Business.Services.W9FormService;
//using EvolvedTax.Common.Constants;
//using EvolvedTax.Data.EFRepository;
//using EvolvedTax.Data.Models.DTOs.Request;
//using EvolvedTax.Data.Models.Entities;
//using EvolvedTax.Helpers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.DotNet.Scaffolding.Shared.Project;
//using PdfSharpCore;
//using PdfSharpCore.Pdf;
//using PdfSharpCore.Pdf.IO;

//namespace EvolvedTax.Controllers
//{
//    [SessionTimeout]
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;
//        private readonly IWebHostEnvironment _webHostEnvironment;
//        private readonly EvolvedtaxContext _evolvedtaxContext;
//        private readonly IW9FormService _w9FormService;
//        private readonly IGeneralQuestionareService _generalQuestionareService;
//        private readonly IW8BenFormService _w8BenFormService;
//        private readonly IW8ECIFormService _w8ECIFormService;
//        private readonly ICommonService _commonService;
//        private readonly IInstituteService _instituteService;


//        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
//            IW9FormService w9FormService, IGeneralQuestionareService generalQuestionareService, IW8BenFormService w8BenFormService, IW8ECIFormService w8ECIFormService, ICommonService commonService, IInstituteService instituteService)
//        {
//            _w8ECIFormService = w8ECIFormService;
//            _w8BenFormService = w8BenFormService;
//            _generalQuestionareService = generalQuestionareService;
//            _w9FormService = w9FormService;
//            _evolvedtaxContext = evolvedtaxContext;
//            _webHostEnvironment = webHostEnvironment;
//            _logger = logger;
//            _commonService = commonService;
//            _instituteService = instituteService;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }
//        public IActionResult Entity()
//        {
//            return View();
//        }

//        public async Task<IActionResult> GQIndividual()
//        {
//            var items = await _evolvedtaxContext.MstrCountries.ToListAsync();
//            ViewBag.CountriesList = items.OrderBy(item => item.Favorite != "0" ? int.Parse(item.Favorite) : int.MaxValue)
//                                  .ThenBy(item => item.Country).Select(p => new SelectListItem
//                                  {
//                                      Text = p.Country,
//                                      Value = p.Country,
//                                  });

//            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
//            {
//                Text = p.StateId,
//                Value = p.StateId
//            });
//            var formName = HttpContext.Session.GetString("FormName") ?? string.Empty;
//            if (!string.IsNullOrEmpty(formName))
//            {
//                var model = new FormRequest();
//                var clientEmmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//                if (!string.IsNullOrEmpty(clientEmmail))
//                {
//                    if (formName == AppConstants.W9Form)
//                    {
//                        model = _w9FormService.GetDataForIndividualByClientEmailId(clientEmmail);
//                        model.FormType = formName;
//                    }
//                    else
//                    {
//                        model.FormType = AppConstants.W8Form;
//                        if (formName == AppConstants.W8BENForm)
//                        {
//                            model.W8FormType = formName;
//                            model = _w8BenFormService.GetDataByClientEmailId(clientEmmail);
//                        }
//                        else if (formName == AppConstants.W8ECIForm)
//                        {
//                            model.W8FormType = formName;
//                            model = _w8ECIFormService.GetDataByClientEmailId(clientEmmail);
//                        }
//                    }
//                    return View(model);
//                }
//            }
//            return View();
//        }
//        [HttpPost]
//        public IActionResult GQIndividual(FormRequest model)
//        {
//            string FormName = string.Empty;
//            string filePathResponse = string.Empty;

//            model.EmailId = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//            model.UserName = model.EmailId;//HttpContext.Session.GetString("UserName") ?? string.Empty;
//            model.BasePath = _webHostEnvironment.WebRootPath;

//            var scheme = HttpContext.Request.Scheme; // "http" or "https"
//            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
//            var fullUrl = $"{scheme}://{host}";
//            model.Host = fullUrl;

//            FormName = HttpContext.Session.GetString("FormName") ?? string.Empty;
//            if (!string.IsNullOrEmpty(FormName))
//            {
//                #region Edit
//                // Removing old form data
//                if (FormName != model.FormType)
//                {
//                    //if (FormName !) { 

//                    //}
//                }

//                if (model.FormType == AppConstants.W9Form) // For W9 form
//                {
//                    model.USCitizen = "1";
//                    FormName = model.FormType;
//                    model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W9TemplateFileName);
//                    var response = _w9FormService.UpdateForIndividual(model);
//                    //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
//                    filePathResponse = response;
//                }
//                else if (model.FormType == AppConstants.W8Form)
//                {
//                    model.USCitizen = "0";
//                    if (model.W8FormType == AppConstants.W8BENForm)
//                    {
//                        model.US1 = "1";
//                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8BENTemplateFileName);
//                        filePathResponse = _w8BenFormService.Update(model);
//                    }
//                    else if (model.W8FormType == AppConstants.W8ECIForm)
//                    {
//                        model.US1 = "2";
//                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8ECITemplateFileName);
//                        filePathResponse = _w8ECIFormService.Update(model);
//                    }
//                    FormName = model.W8FormType;
//                }
//                var responseGQForm = _generalQuestionareService.Update(model);
//                if (responseGQForm == 0)
//                {
//                    return View(model);
//                }
//                #endregion
//            }
//            else
//            {
//                #region ADD

//                if (model.FormType == AppConstants.W9Form) // For W9 form
//                {
//                    model.USCitizen = "1";
//                    FormName = model.FormType;
//                    model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W9TemplateFileName);
//                    var response = _w9FormService.SaveForIndividual(model);
//                    //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
//                    HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
//                    filePathResponse = response;
//                }
//                else if (model.FormType == AppConstants.W8Form)
//                {
//                    model.USCitizen = "0";
//                    if (model.W8FormType == AppConstants.W8BENForm)
//                    {
//                        model.US1 = "1";
//                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8BENTemplateFileName);
//                        if (model.W8BENOnBehalfName)
//                        {
//                            HttpContext.Session.SetString("ClientName", model.PrintNameOfSigner ?? string.Empty);
//                        }
//                        else
//                        {
//                            HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
//                            model.PrintNameOfSigner = string.Concat(model.GQFirstName, " ", model.GQLastName);
//                        }
//                        filePathResponse = _w8BenFormService.Save(model);
//                    }
//                    else if (model.W8FormType == AppConstants.W8ECIForm)
//                    {
//                        model.US1 = "2";
//                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8ECITemplateFileName);
//                        if (model.W8ECIOnBehalfName)
//                        {
//                            HttpContext.Session.SetString("ClientName", model.PrintNameOfSignerW8ECI ?? string.Empty);
//                        }
//                        else
//                        {
//                            HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
//                            model.PrintNameOfSignerW8ECI = string.Concat(model.GQFirstName, " ", model.GQLastName);
//                        }
//                        filePathResponse = _w8ECIFormService.Save(model);
//                    }
//                    FormName = model.W8FormType;
//                }
//                var responseGQForm = _generalQuestionareService.Save(model);
//                if (responseGQForm == 0)
//                {
//                    return View(model);
//                }
//                #endregion
//            }
//            //byte[] pdfBytes = System.IO.File.ReadAllBytes(filePathResponse);
//            //MemoryStream memoryStream = new MemoryStream(pdfBytes);
//            //return new FileStreamResult(memoryStream, "application/pdf");
//            HttpContext.Session.SetString("PdfdFileName", filePathResponse);
//            HttpContext.Session.SetString("BaseURL", _webHostEnvironment.WebRootPath);
//            HttpContext.Session.SetString("FormName", FormName);
//            return Json(filePathResponse);
//        }
//        public IActionResult Certification()
//        {
//            if (string.IsNullOrEmpty(HttpContext.Session.GetString("FormName")))
//            {
//                var clientEmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//                if (!string.IsNullOrEmpty(clientEmail))
//                {
//                    return RedirectToAction("DownloadForm", "Home", new { clientEmail = clientEmail });
//                }
//                return RedirectToAction("AccessDenied", "Account", new { statusCode = 401 });
//            }
//            var request = new PdfFormDetailsRequest();
//            request.FileName = HttpContext.Session.GetString("PdfdFileName") ?? string.Empty;
//            request.PrintName = HttpContext.Session.GetString("ClientName") ?? string.Empty;
//            var model = new List<PdfFormDetailsRequest> {
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_PalaceScriptMT, FontSize = "36px", Text = request.PrintName},
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_VladimirScript, FontSize = "36px", Text = request.PrintName},
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_FrenchScriptMT, FontSize = "36px", Text = request.PrintName},
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_SegoeScript, FontSize = "36px", Text = request.PrintName},
//                new PdfFormDetailsRequest { FontFamily = AppConstants.F_Family_BlackadderITC, FontSize = "36px", Text = request.PrintName},
//            };
//            request.ButtonRequests = model;
//            request.EntryDate = DateTime.Now;
//            request.IsSignaturePasted = false;
//            return View(request);
//        }
//        [HttpPost]
//        public IActionResult Certification(PdfFormDetailsRequest request)
//        {
//            if (string.IsNullOrEmpty(HttpContext.Session.GetString("FormName")))
//            {
//                var clientEmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//                if (!string.IsNullOrEmpty(clientEmail))
//                {
//                    return RedirectToAction("DownloadForm", "Home", new { clientEmail = clientEmail });
//                }
//                return RedirectToAction("AccessDenied", "Account", new { statusCode = 401 });
//            }
//            var buttonRequest = new PdfFormDetailsRequest();
//            if (request.FontFamily.Trim() == AppConstants.F_Family_PalaceScriptMT)
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "19";
//            }
//            else if (request.FontFamily == AppConstants.F_Family_VladimirScript)
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "15";
//            }
//            else if (request.FontFamily == AppConstants.F_Family_FrenchScriptMT)
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "17";
//            }
//            else if (request.FontFamily == AppConstants.F_Family_SegoeScript)
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "12";
//            }
//            else
//            {
//                buttonRequest.Text = request.PrintName;
//                buttonRequest.FontFamily = request.FontFamily;
//                buttonRequest.FontSize = "10";
//            }
//            buttonRequest.IsSignaturePasted = true;
//            buttonRequest.BaseUrl = HttpContext.Session.GetString("BaseURL") ?? string.Empty;
//            buttonRequest.FormName = HttpContext.Session.GetString("FormName") ?? string.Empty; ;
//            buttonRequest.EntryDate = request.EntryDate;
//            buttonRequest.FileName = _commonService.AssignSignature(buttonRequest, request.FileName);
//            return View(buttonRequest);
//        }

//        public async Task<IActionResult> Certify(PdfFormDetailsRequest request)
//        {
//            var oldFile = request.FileName;
//            request.FileName = request.FileName.Replace("_temp", "");
//            var clientEmail = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
//            if (!(request.Agreement1 && request.Agreement2))
//            {
//                return Json(new { staus = false });
//            }
//            if (request.FormName == AppConstants.W9Form)
//            {
//                await _w9FormService.UpdateByClientEmailId(clientEmail, request);
//                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
//            }
//            else if (request.FormName == AppConstants.W8BENForm)
//            {
//                await _w8BenFormService.UpdateByClientEmailId(clientEmail, request);
//                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
//            }
//            else if (request.FormName == AppConstants.W8ECIForm)
//            {
//                await _w8ECIFormService.UpdateByClientEmailId(clientEmail, request);
//                await _instituteService.UpdateClientByClientEmailId(clientEmail, request);
//            }
//            else
//            {
//                return Json(new { staus = false });
//            }

//            if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, oldFile)))
//            {
//                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, oldFile));
//            }
//            var EntityName = _instituteService.GetEntityDataByClientEmailId(clientEmail).EntityName;
//            var InstituteEmail = _instituteService.GetInstituteDataByClientEmailId(clientEmail).EmailAddress ?? string.Empty;
//            HttpContext.Session.SetString("FormName", string.Empty);
//            return Json(new { fileName = request.FileName, staus = true, entityName = EntityName, printName = request.PrintName, formName = request.FormName, InstituteEmail = InstituteEmail }); ;
//        }
//        public IActionResult DownloadForm(string clientEmail)
//        {
//            var entityName = _instituteService.GetEntityDataByClientEmailId(clientEmail).EntityName;
//            var clientData = _instituteService.GetClientDataByClientEmailId(clientEmail);
//            ViewBag.FileName = clientData.FileName;
//            ViewBag.EntityName = entityName;
//            ViewBag.FormName = clientData?.FormName;
//            ViewBag.InstituteEmail = _instituteService.GetInstituteDataByClientEmailId(clientEmail).EmailAddress ?? string.Empty;
//            if (clientData?.FormName?.Trim() == AppConstants.W9Form)
//            {
//                ViewBag.PrintName = _w9FormService.GetDataForIndividualByClientEmailId(clientEmail).W9PrintName;
//            }
//            else if (clientData?.FormName?.Trim() == AppConstants.W8BENForm)
//            {
//                ViewBag.PrintName = _w8BenFormService.GetDataByClientEmailId(clientEmail).NameOfIndividual;
//            }
//            else if (clientData?.FormName?.Trim() == AppConstants.W8ECIForm)
//            {
//                ViewBag.PrintName = _w8ECIFormService.GetDataByClientEmailId(clientEmail).NameOfIndividual;
//            }
//            return View();
//        }
        
//    }
//}