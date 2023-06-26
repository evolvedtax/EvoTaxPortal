using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using EvolvedTax.Business.Services.GeneralQuestionareService;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax.Controllers
{
    [UserSession]
    public class EntityController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EvolvedtaxContext _evolvedtaxContext;
        private readonly IW9FormService _w9FormService;
        private readonly ICommonService _commonService;
        private readonly IInstituteService _instituteService;
        private readonly IGeneralQuestionareEntityService _generalQuestionareEntityService;
        private readonly IW8ECIFormService _w8ECIFormService;
        private readonly IW8EXPFormService _w8EXPFormService;
        public EntityController(IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
            IW9FormService w9FormService, ICommonService commonService, IInstituteService instituteService,
                            IGeneralQuestionareEntityService generalQuestionareEntityService, IW8EXPFormService W8EXPFormService, IW8ECIFormService w8ECIFormService)
        {
            _w9FormService = w9FormService;
            _evolvedtaxContext = evolvedtaxContext;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
            _generalQuestionareEntityService = generalQuestionareEntityService;
            _w8EXPFormService = W8EXPFormService;
            _w8ECIFormService = w8ECIFormService;
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

            ViewBag.CountriesListW8 = items.Where(item => item.Favorite != "5")
                                 .OrderBy(item => item.Favorite != "0" ? int.Parse(item.Favorite) : int.MaxValue)
                                 .ThenBy(item => item.Country)
                                 .Select(p => new SelectListItem
                                 {
                                     Text = p.Country,
                                     Value = p.Country,
                                 });

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId
            });
            ViewBag.EntitiesList = _evolvedtaxContext.MasterEntityTypes.Where(p=>p.IsActive == true).Select(p => new SelectListItem
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
                        //GQEntitiesResponse.FormType = formName;
                    }
                    else if (formName == AppConstants.W8ECIForm)
                    {
                        GQEntitiesResponse = _w8ECIFormService.GetDataByClientEmailId(clientEmail);
                        //GQEntitiesResponse.FormType = formName;
                    }
                    else if (formName == AppConstants.W8EXPForm)
                    {
                        GQEntitiesResponse = _w8EXPFormService.GetDataByClientEmail(clientEmail);
                        //GQEntitiesResponse.FormType = formName;
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

            #region W8EXPForm save logic
            if (model.FormType == AppConstants.W8FormTypes && model.W8FormType == AppConstants.W8EXPForm)
            {

                FormName = model.W8FormType;
                model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8EXPTemplateFileName);

                var responsew8EXPForm = _w8EXPFormService.Save(model);
               // int SaveID = Convert.ToInt32(responsew8EXPForm[0].ToString());
               // string FilePath = responsew8EXPForm[1].ToString();
                //model.W8ExpId = SaveID;
                var clientData = _instituteService.GetClientDataByClientEmailId(model.EmailId);
                HttpContext.Session.SetString("ClientName", model.AuthSignatoryName);
                filePathResponse = responsew8EXPForm;

            }
            #endregion
            #region ADD
            if (model.FormType == AppConstants.W9Form) // For W9 form
            {
                model.USCitizen = "1";
                FormName = model.FormType;
                model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W9TemplateFileName);
                var response = _w9FormService.SaveForEntity(model);
                //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
                var clientData = _instituteService.GetClientDataByClientEmailId(model.EmailId);
                HttpContext.Session.SetString("ClientName", model.AuthSignatoryName);
                filePathResponse = response;
            }
            else if (model.FormType == AppConstants.W8FormTypes)
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
                        HttpContext.Session.SetString("ClientName", model.AuthSignatoryName ?? string.Empty);
                    }
                    else
                    {
                        HttpContext.Session.SetString("ClientName", string.Concat(model.AuthSignatoryName ?? string.Empty));
                        model.PrintNameOfSignerW8ECI = string.Concat(model.AuthSignatoryName ?? string.Empty);
                    }
                    filePathResponse = _w8ECIFormService.SaveForEntity(model);
                }
                else if (model.W8FormType == AppConstants.W8IMYForm)
                {
                    model.US1 = "2";
                    model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8IMYTemplateFileName);
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
            HttpContext.Session.SetString("PdfdFileName", filePathResponse);
            HttpContext.Session.SetString("BaseURL", _webHostEnvironment.WebRootPath);
            HttpContext.Session.SetString("FormName", FormName);
            HttpContext.Session.SetString("EntityStatus", AppConstants.EntityStatus);
            return Json(filePathResponse);
        }


        #region Entity Dropdown Dynamic Binding on the basis of selection
        [HttpPost]
        public ActionResult BindEntityForW8EXPForm()
        {
            var entitiesListW8 = _evolvedtaxContext.W8expentities.Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString()
            });

            ViewBag.entitiesListW8 = entitiesListW8;

            return Json(new { success = true, entitiesListW8 });
        }

        [HttpPost]
        public ActionResult BindEntityForW8ECIGQForm()
        {
            var entitiesList = _evolvedtaxContext.MasterEntityTypes.Where(p=>p.IsActive == true).Select(p => new SelectListItem
            {
                Text = p.EntityType,
                Value = p.EntityId.ToString()
            });
            return Json(new { success = true, entitiesList });
        }
        [HttpPost]
        public ActionResult BindEntityForW8ECIForm()
        {
            var entitiesList = _evolvedtaxContext.W8eciEntityTypes.Select(p => new SelectListItem
            {
                Text = p.EntityType,
                Value = p.EntityId.ToString()
            });
            return Json(new { success = true, entitiesList });
        }
        [HttpPost]
        public ActionResult BindEntityForW9Form()
        {
            var entitiesList = _evolvedtaxContext.MasterEntityTypes.Select(p => new SelectListItem
            {
                Text = p.EntityType,
                Value = p.EntityId.ToString()
            });

            ViewBag.EntitiesList = entitiesList;

            return Json(new { success = true, entitiesList });
        }

        [HttpPost]
        public ActionResult BindEW8EXPFATCA()
        {
            var W8EXPFATCAList = _evolvedtaxContext.W8expfatcas.Select(p => new SelectListItem
            {
                Text = p.Fatca,
                Value = p.FatcaId.ToString()
            });

            ViewBag.W8EXPFATCAList = W8EXPFATCAList;

            return Json(new { success = true, W8EXPFATCAList });
        }
        #endregion

    }
}
