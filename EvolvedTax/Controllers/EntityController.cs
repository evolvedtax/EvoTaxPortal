using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.W8BenFormService;
using EvolvedTax.Business.Services.W8ECIFormService;
using EvolvedTax.Business.Services.W9FormService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax.Controllers
{
    public class EntityController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EvolvedtaxContext _evolvedtaxContext;
        private readonly IW9FormService _w9FormService;
        private readonly ICommonService _commonService;
        private readonly IInstituteService _instituteService;
        private readonly IGeneralQuestionareEntityService _generalQuestionareEntityService;
        public EntityController(IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
            IW9FormService w9FormService, ICommonService commonService, IInstituteService instituteService, IGeneralQuestionareEntityService generalQuestionareEntityService)
        {
            _w9FormService = w9FormService;
            _evolvedtaxContext = evolvedtaxContext;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
            _generalQuestionareEntityService = generalQuestionareEntityService;
        }
        public async Task<IActionResult> GQEntity()
        {
            string clientEmail = HttpContext.Session.GetString("ClientEmail") ?? "";
            if (_instituteService.GetClientDataByClientEmailId(clientEmail)?.ClientStatus == AppConstants.ClientStatusFormSubmitted)
            {
                return RedirectToAction("DownloadForm", "Certification", new { clientEmail = clientEmail });
            }
            var GQEntitiesResponse = _generalQuestionareEntityService.GetDataByClientEmail(clientEmail);

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
            ViewBag.EntitiesList = _evolvedtaxContext.MasterEntityTypes.Select(p => new SelectListItem
            {
                Text = p.EntityType,
                Value = p.EntityId.ToString()
            });
            ViewBag.PayeeCodeList = _evolvedtaxContext.ExemptPayeeCodes.Select(p => new SelectListItem
            {
                Text = p.ExemptCode,
                Value = p.ExemptValue
            });
            ViewBag.FATCACodeList = _evolvedtaxContext.FatcaCodes.Select(p => new SelectListItem
            {
                Text = p.FatcaCode1,
                Value = p.FatcaValue
            });

            var formName = HttpContext.Session.GetString("FormName") ?? string.Empty;
            if (!string.IsNullOrEmpty(formName))
            {
                if (!string.IsNullOrEmpty(clientEmail))
                {
                    if (formName == AppConstants.W9Form)
                    {
                        GQEntitiesResponse = _w9FormService.GetDataForEntityByClientEmailId(clientEmail);
                        GQEntitiesResponse.FormType = formName;
                    }
                    else
                    {
                        
                    }
                    return View(GQEntitiesResponse);
                }
            }
            return View(GQEntitiesResponse);
        }
        [HttpPost]
        public IActionResult GQEntity(FormRequest model)
        {
            string FormName = string.Empty;
            string filePathResponse = string.Empty;

            model.IndividualOrEntityStatus = AppConstants.EntityStatus;
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
                    var response = _w9FormService.UpdateForEntity(model);
                    //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
                    filePathResponse = response;
                }
                else if (model.FormType == AppConstants.W8Form)
                {
                    
                }
                var responseGQForm = _generalQuestionareEntityService.Update(model);
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
                    var response = _w9FormService.SaveForEntity(model);
                    //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
                    HttpContext.Session.SetString("ClientName", model.GQOrgName);
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
                        }
                        else
                        {
                            HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
                            model.PrintNameOfSigner = string.Concat(model.GQFirstName, " ", model.GQLastName);
                        }
                        //filePathResponse = _w8BenFormService.Save(model);
                    }
                    else if (model.W8FormType == AppConstants.W8ECIForm)
                    {
                        model.US1 = "2";
                        model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8ECITemplateFileName);
                        if (model.W8ECIOnBehalfName)
                        {
                            HttpContext.Session.SetString("ClientName", model.PrintNameOfSignerW8ECI ?? string.Empty);
                        }
                        else
                        {
                            HttpContext.Session.SetString("ClientName", string.Concat(model.GQFirstName, " ", model.GQLastName));
                            model.PrintNameOfSignerW8ECI = string.Concat(model.GQFirstName, " ", model.GQLastName);
                        }
                        //filePathResponse = _w8ECIFormService.Save(model);
                    }
                    FormName = model.W8FormType;
                }
                var responseGQForm = _generalQuestionareEntityService.Save(model);
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
            return Json(filePathResponse);
        }
        
    }
}
