using AutoMapper;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Business.Services.W8BEN_E_FormService;
using EvolvedTax.Business.Services.W8ECIFormService;
using EvolvedTax.Business.Services.W8EXPFormService;
using EvolvedTax.Business.Services.W8IMYFormService;
using EvolvedTax.Business.Services.W9FormService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
        private readonly IW8BEN_E_FormService _w8BENEFormService;
        private readonly IW8EXPFormService _w8EXPFormService;
        private readonly IW8IMYFormService _W8IMYFormService;
        private readonly IMapper _mapper;
        public EntityController(IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
            IW9FormService w9FormService, ICommonService commonService, IInstituteService instituteService,
                            IGeneralQuestionareEntityService generalQuestionareEntityService,
                            IW8EXPFormService W8EXPFormService, IW8ECIFormService w8ECIFormService, IW8IMYFormService W8IMYFormService,IW8BEN_E_FormService w8BENEFormService, IMapper mapper)
        {
            _w9FormService = w9FormService;
            _evolvedtaxContext = evolvedtaxContext;
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _instituteService = instituteService;
            _generalQuestionareEntityService = generalQuestionareEntityService;
            _w8EXPFormService = W8EXPFormService;
            _w8ECIFormService = w8ECIFormService;
            _w8BENEFormService = w8BENEFormService;
            _mapper = mapper;
            _W8IMYFormService = W8IMYFormService;
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
            ViewBag.EntitiesList = _evolvedtaxContext.MasterEntityTypes.Where(p => p.IsActive == true).Select(p => new SelectListItem
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

            if(GQEntitiesResponse != null)
            {

                if (GQEntitiesResponse.W8FormType == AppConstants.W8EXPForm)
                {
                    GQEntitiesResponse = _w8EXPFormService.GetDataByClientEmail(clientEmail);
                }
                else
                {
                    _w9FormService.ActivateRecord(clientEmail);
                    GQEntitiesResponse = _w9FormService.GetDataForEntityByClientEmailId(clientEmail);
                }
            }


            /*
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
                        GQEntitiesResponse = _w8ECIFormService.GetEntityDataByClientEmailId(clientEmail);
                        //GQEntitiesResponse.FormType = formName;
                    }
                    else if (formName == AppConstants.W8EXPForm)
                    {
                        GQEntitiesResponse = _w8EXPFormService.GetDataByClientEmail(clientEmail);
                        //GQEntitiesResponse.FormType = formName;
                    }
                    else if (formName == AppConstants.W8IMYForm)
                    {
                        GQEntitiesResponse = _W8IMYFormService.GetDataByClientEmail(clientEmail);
                        //GQEntitiesResponse.FormType = formName;
                    }
                    else
                    {

                    }
                    
                    return View(GQEntitiesResponse);
                }
            }
            */
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

            //#region W8EXPForm save logic
            //if (model.FormType == AppConstants.W8FormTypes && model.W8FormType == AppConstants.W8EXPForm)
            //{
            //    bool isPartialSave = model.IsPartialSave;
            //    FormName = model.W8FormType;

            //    if (isPartialSave)
            //    {

            //        var responsePartialForm = _w8EXPFormService.SavePartial(model);
            //        var responseGQPartialForm = _generalQuestionareEntityService.Save(model);
            //        if (responseGQPartialForm == 0 || responsePartialForm == 0)
            //        {
            //            return View(model);
            //        }
            //        // Return a response indicating successful partial save
            //        return Json(new { success = true, message = AppConstants.FormPartiallySave });
            //    }

            //    model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8EXPTemplateFileName);

            //    var responsew8EXPForm = _w8EXPFormService.Save(model);
            //    var clientData = _instituteService.GetClientDataByClientEmailId(model.EmailId);
            //    HttpContext.Session.SetString("ClientName", model.AuthSignatoryName);
            //    filePathResponse = responsew8EXPForm;

            //}
            //#endregion
            #region ADD
            if (model.FormType == AppConstants.W9Form) // For W9 form
            {
                model.USCitizen = "1";
                FormName = model.FormType;
                model.W8FormType = "";
                model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W9TemplateFileName);
                var response = _w9FormService.SaveForEntity(model);
                //filePathResponse = Path.Combine(_webHostEnvironment.WebRootPath, response);
                var clientData = _instituteService.GetClientDataByClientEmailId(model.EmailId);
                HttpContext.Session.SetString("ClientName", model.AuthSignatoryName ?? "");
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
                    if (model.W8ECIOnBehalfName ?? false)
                    {
                        HttpContext.Session.SetString("ClientName", model.AuthSignatoryName ?? string.Empty);
                    }
                    else
                    {
                        HttpContext.Session.SetString("ClientName", string.Concat(model.AuthSignatoryName ?? string.Empty));
                        model.PrintNameOfSignerW8ECI = string.Concat(model.AuthSignatoryName ?? string.Empty);
                    }
                    //filePathResponse = _w8ECIFormService.SaveForEntity(model);
                }
                else if (model.W8FormType == AppConstants.W8IMYForm)
                {
                    model.US1 = "2";
                    model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8IMYTemplateFileName);
                    if (model.W8ECIOnBehalfName ?? false)
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
            if (filePathResponse == AppConstants.FormPartiallySave)
            {
                return Json(new { success = true, message = AppConstants.FormPartiallySave });
            }
            #endregion
            HttpContext.Session.SetString("PdfdFileName", filePathResponse);
            HttpContext.Session.SetString("BaseURL", _webHostEnvironment.WebRootPath);
            HttpContext.Session.SetString("FormName", FormName);
            HttpContext.Session.SetString("EntityStatus", AppConstants.EntityStatus);
            return Json(filePathResponse);
        }


        #region W8EXP
        public async Task<IActionResult> W8EXP()
        {
            string clientEmail = HttpContext.Session.GetString("ClientEmail") ?? "";
            _w8EXPFormService.ActivateRecord(clientEmail);

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
            var entitiesListW8 = _evolvedtaxContext.W8expentities.Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString()
            });
            ViewBag.entitiesListW8 = entitiesListW8;

            var W8EXPFATCAList = _evolvedtaxContext.W8expfatcas.Select(p => new SelectListItem
            {
                Text = p.Fatca,
                Value = p.FatcaId.ToString()
            });

            ViewBag.W8EXPFATCAList = W8EXPFATCAList;

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId
            });


            var formName = HttpContext.Session.GetString("FormName") ?? string.Empty;
            if (!string.IsNullOrEmpty(formName))
            {
                if (!string.IsNullOrEmpty(clientEmail))
                {
                    if (formName == AppConstants.W8EXPForm)
                    {

                        GQEntitiesResponse = _w8EXPFormService.GetDataByClientEmail(clientEmail);
                        if (GQEntitiesResponse != null)
                        {
                            GQEntitiesResponse.activeTabIndex = "0";

                        }
                    }

                    return View(GQEntitiesResponse);
                }
            }
            else
            {
                GQEntitiesResponse = _w8EXPFormService.GetDataByClientEmail(clientEmail);
            }

       


            /*
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
                        GQEntitiesResponse = _w8ECIFormService.GetEntityDataByClientEmailId(clientEmail);
                        //GQEntitiesResponse.FormType = formName;
                    }
                    else if (formName == AppConstants.W8EXPForm)
                    {
                        GQEntitiesResponse = _w8EXPFormService.GetDataByClientEmail(clientEmail);
                        //GQEntitiesResponse.FormType = formName;
                    }
                    else if (formName == AppConstants.W8IMYForm)
                    {
                        GQEntitiesResponse = _W8IMYFormService.GetDataByClientEmail(clientEmail);
                        //GQEntitiesResponse.FormType = formName;
                    }
                    else
                    {

                    }
                    
                    return View(GQEntitiesResponse);
                }
            }
            */
            return View(GQEntitiesResponse);
        }
        [HttpPost]
        public IActionResult W8EXP(FormRequest model)
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

           // FormName = HttpContext.Session.GetString("FormName") ?? string.Empty;
            model.FormType = AppConstants.W8FormTypes.ToString();
            model.W8FormType = AppConstants.W8EXPForm.ToString();
            FormName = model.W8FormType;
            #region W8EXPForm save logic

            bool isPartialSave = model.IsPartialSave;
                FormName = model.W8FormType;

                if (isPartialSave)
                {

                    var responsePartialForm = _w8EXPFormService.SavePartial(model);
                    var responseGQPartialForm = _generalQuestionareEntityService.Save(model);
                    if (responseGQPartialForm == 0 || responsePartialForm == 0)
                    {
                        return View(model);
                    }
                    // Return a response indicating successful partial save
                    return Json(new { success = true, message = AppConstants.FormPartiallySave });
                }
        
            model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8EXPTemplateFileName);

                var responsew8EXPForm = _w8EXPFormService.Save(model);
                var clientData = _instituteService.GetClientDataByClientEmailId(model.EmailId);
                HttpContext.Session.SetString("ClientName", model.AuthSignatoryName);
                filePathResponse = responsew8EXPForm;

            
            #endregion
            var responseGQForm = _generalQuestionareEntityService.Save(model);
            if (responseGQForm == 0)
            {
                return View(model);
            }
            if (filePathResponse == AppConstants.FormPartiallySave)
            {
                return Json(new { success = true, message = AppConstants.FormPartiallySave });
            }
          
            HttpContext.Session.SetString("PdfdFileName", filePathResponse);
            HttpContext.Session.SetString("BaseURL", _webHostEnvironment.WebRootPath);
            HttpContext.Session.SetString("FormName", FormName);
            HttpContext.Session.SetString("EntityStatus", AppConstants.EntityStatus);
            return Json(filePathResponse);
        }

        #endregion

        #region W8ECI
        public async Task<IActionResult> W8ECI()
        {
            string clientEmail = HttpContext.Session.GetString("ClientEmail") ?? "";
            _w8ECIFormService.ActivateRecord(clientEmail);
            if (_instituteService.GetClientDataByClientEmailId(clientEmail)?.ClientStatus == AppConstants.ClientStatusFormSubmitted)
            {
                return RedirectToAction("DownloadForm", "Certification", new { clientEmail = clientEmail });
            }

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

            ViewBag.TypeOfEntityW8ECI = _evolvedtaxContext.W8eciEntityTypes.Select(p => new SelectListItem
            {
                Text = p.EntityType,
                Value = p.EntityId.ToString()
            });

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId
            });

            var GQEntitiesResponse = _w8ECIFormService.GetEntityDataByClientEmailId(clientEmail);
            //GQEntitiesResponse.FormType = formName;
            return View(GQEntitiesResponse ??  new FormRequest());
        }
        [HttpPost]
        public IActionResult W8ECI(FormRequest model)
        {
            string filePathResponse = string.Empty;
            model.IndividualOrEntityStatus = AppConstants.EntityStatus;
            model.EmailId = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
            model.UserName = model.EmailId;//HttpContext.Session.GetString("UserName") ?? string.Empty;
            model.BasePath = _webHostEnvironment.WebRootPath;

            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
            var fullUrl = $"{scheme}://{host}";
            model.Host = fullUrl;

            model.US1 = "2";
            model.FormType = AppConstants.W8FormTypes;
            model.W8FormType = AppConstants.W8ECIForm;
            model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8ECITemplateFileName);
            HttpContext.Session.SetString("ClientName", model.PrintNameOfSignerW8ECI ?? string.Empty);

            filePathResponse = _w8ECIFormService.SaveForEntity(model);
            var responseGQForm = _generalQuestionareEntityService.Save(model);
            if (responseGQForm == 0)
            {
                return View(model);
            }
            if (filePathResponse == AppConstants.FormPartiallySave)
            {
                return Json(new { success = true, message = AppConstants.FormPartiallySave });
            }
            HttpContext.Session.SetString("PdfdFileName", filePathResponse);
            HttpContext.Session.SetString("BaseURL", _webHostEnvironment.WebRootPath);
            HttpContext.Session.SetString("FormName", AppConstants.W8ECIForm);
            HttpContext.Session.SetString("EntityStatus", AppConstants.EntityStatus);
            return Json(filePathResponse);
        }
        #endregion

        #region W8BENE
        public async Task<IActionResult> W8BENE()
        {
            string clientEmail = HttpContext.Session.GetString("ClientEmail") ?? "";
            _w8BENEFormService.ActivateRecord(clientEmail);
            if (_instituteService.GetClientDataByClientEmailId(clientEmail)?.ClientStatus == AppConstants.ClientStatusFormSubmitted)
            {
                return RedirectToAction("DownloadForm", "Certification", new { clientEmail = clientEmail });
            }

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

            ViewBag.TypeOfEntityW8BENE = _evolvedtaxContext.W8beneEntityTypes.Select(p => new SelectListItem
            {
                Text = p.EntityType,
                Value = p.EntityId.ToString()
            });

            ViewBag.FATCAStatusW8BENE = _evolvedtaxContext.W8benefatcas.Select(p => new SelectListItem
            {
                Text = p.Fatca,
                Value = p.FatcaId.ToString()
            });
            ViewBag.W8benefatcaDE = _evolvedtaxContext.W8benefatcades.Select(p => new SelectListItem
            {
                Text = p.Fatca,
                Value = p.FatcaId.ToString()
            });

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId
            });
            //var GQEntitiesResponse = new W8BENERequest();
            var GQEntitiesResponse = _w8BENEFormService.GetEntityDataByClientEmailId(clientEmail);
            //GQEntitiesResponse.FormType = formName;
            return View(GQEntitiesResponse ?? new W8BENERequest());
        }
        [HttpPost]
        public IActionResult W8BENE(W8BENERequest model)
        {
            string filePathResponse = string.Empty;
            model.IndividualOrEntityStatus = AppConstants.EntityStatus;
            model.EmailId = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
            //model.UserName = model.EmailId;//HttpContext.Session.GetString("UserName") ?? string.Empty;
            model.BasePath = _webHostEnvironment.WebRootPath;

            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
            var fullUrl = $"{scheme}://{host}";
            model.Host = fullUrl;


            model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8BENETemplateFileName);
            HttpContext.Session.SetString("ClientName", model.PrintNameOfSigner ?? string.Empty);

            model.GQOrgName = model.NameOfOrganization ?? string.Empty;
            model.EntityType = model.TypeOfEntity ?? "";
            model.DE = model.DECheckBox;
            model.DEOwnerName = model.NameOfDiregardedEntity ?? string.Empty;
            model.FormType = AppConstants.W8FormTypes;
            model.W8FormType = AppConstants.W8BENEForm;

            filePathResponse = _w8BENEFormService.SaveForEntity(model);

            var responseGQForm = _generalQuestionareEntityService.Save(_mapper.Map<FormRequest>(model));
            if (responseGQForm == 0)
            {
                return View(model);
            }
            if (filePathResponse == AppConstants.FormPartiallySave)
            {
                return Json(new { success = true, message = AppConstants.FormPartiallySave });
            }
            HttpContext.Session.SetString("PdfdFileName", filePathResponse);
            HttpContext.Session.SetString("BaseURL", _webHostEnvironment.WebRootPath);
            HttpContext.Session.SetString("FormName", AppConstants.W8BENEForm);
            HttpContext.Session.SetString("EntityStatus", AppConstants.EntityStatus);
            return Json(filePathResponse);
        }
        #endregion

        #region W8IMY Code

        public async Task<IActionResult> W8IMY()
        {
            string clientEmail = HttpContext.Session.GetString("ClientEmail") ?? "";
            _W8IMYFormService.ActivateRecord(clientEmail);

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
            ViewBag.EntitiesList = _evolvedtaxContext.W8imyEntityTypes.Select(p => new SelectListItem
            {
                Text = p.EntityType,
                Value = p.EntityId.ToString()
            });

            ViewBag.FATCAList = _evolvedtaxContext.W8imyFatcas.Select(p => new SelectListItem
            {
                Text = p.Fatca,
                Value = p.FatcaId.ToString()
            });

            ViewBag.FATCAListForCH4 = _evolvedtaxContext.W8imyfatcades.Select(p => new SelectListItem
            {
                Text = p.Fatca,
                Value = p.FatcaId.ToString()
            });





            var formName = HttpContext.Session.GetString("FormName") ?? string.Empty;
            if (!string.IsNullOrEmpty(formName))
            {
                if (!string.IsNullOrEmpty(clientEmail))
                {
                    if (formName == AppConstants.W8IMYForm)
                    {

                        GQEntitiesResponse = _W8IMYFormService.GetDataByClientEmail(clientEmail);
                        if (GQEntitiesResponse != null)
                        {
                            GQEntitiesResponse.activeTabIndex = "0";
                            
                        }
                    }

                    return View(GQEntitiesResponse);
                }
            }
            else
            {
                GQEntitiesResponse = _W8IMYFormService.GetDataByClientEmail(clientEmail);
            }

            return View(GQEntitiesResponse);
        }

        [HttpPost]
        public IActionResult W8IMY(FormRequest model)
        {

            bool isPartialSave = model.IsPartialSave;

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
            model.FormType = AppConstants.W8FormTypes.ToString();
            model.W8FormType = AppConstants.W8IMYForm.ToString();


            if (isPartialSave)
            {
                // string activeTabIndex = model.activeTabIndex;
                //string activeTabIndex = HttpContext.Request.Form["activeTabIndex"];
                var responsePartialForm = _W8IMYFormService.SavePartial(model);
                var responseGQPartialForm = _generalQuestionareEntityService.Save(model);
                if (responseGQPartialForm == 0 || responsePartialForm == 0)
                {
                    return View(model);
                }
                // Return a response indicating successful partial save
                return Json(new { success = true, message = AppConstants.FormPartiallySave });
            }
            #region W8EXPForm save logic


            FormName = model.W8FormType;
            model.TemplateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Forms", AppConstants.W8IMYTemplateFileName);

            var responsew8EXPForm = _W8IMYFormService.Save(model);
            //var clientData = _instituteService.GetClientDataByClientEmailId(model.EmailId);
            HttpContext.Session.SetString("ClientName", model.PrintNameOfSignerW8IMY);
            filePathResponse = responsew8EXPForm;

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

        #endregion

        #region P.O BOX validation
        [HttpGet]
        public IActionResult ValidateAddress(string MAddress1)
        {
            bool isValid = CheckAddressAgainstForbiddenTerms(MAddress1);

            if (isValid)
            {
                return Json(true); // Address is valid
            }

            return Json(false); // Address is not valid
        }
        [HttpGet]
        public IActionResult ValidatePAddress(string PAddress1)
        {
            bool isValid = CheckAddressAgainstForbiddenTerms(PAddress1);

            if (isValid)
            {
                return Json(true); // Address is valid
            }

            return Json(false); // Address is not valid
        }
        [HttpGet]
        public IActionResult Validate_Mailing_address(string _12Mailing_address)
        {
            bool isValid = CheckAddressAgainstForbiddenTerms(_12Mailing_address);

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
            //return true;
        }
        #endregion

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
            var entitiesList = _evolvedtaxContext.MasterEntityTypes.Where(p => p.IsActive == true).Select(p => new SelectListItem
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

        #region Check Existing Record if the user want to delete it 

        [HttpPost]
        public IActionResult CheckAndDelete()
        {
            string emailAddress = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
            bool recordExistsInW8EBENEForm = _evolvedtaxContext.TblW8ebeneforms.Any(e => e.W8beneemailAddress == emailAddress && e.IsActive == true);
            bool recordExistsInW8IMYForm = _evolvedtaxContext.TblW8imyforms.Any(e => e.EmailAddress == emailAddress && e.IsActive == true);
            bool recordExistsInW8EXPForm = _evolvedtaxContext.TblW8expforms.Any(e => e.EmailAddress == emailAddress && e.IsActive == true);
            bool recordExistsInW8ECIForm = _evolvedtaxContext.TblW8eciforms.Any(e => e.W8eciemailAddress == emailAddress && e.IsActive == true);
            bool recordExistsInW9Form = _evolvedtaxContext.TblW9forms.Any(e => e.W9emailAddress == emailAddress && e.IsActive == true);

            bool recordExists = recordExistsInW8EBENEForm || recordExistsInW8IMYForm || recordExistsInW8EXPForm || recordExistsInW8ECIForm || recordExistsInW9Form;

            return Json(new
            {
                exists = recordExists,
                recordExistsInW8EBENEForm = recordExistsInW8EBENEForm,
                recordExistsInW8IMYForm = recordExistsInW8IMYForm,
                recordExistsInW8EXPForm = recordExistsInW8EXPForm,
                recordExistsInW8ECIForm = recordExistsInW8ECIForm,
                recordExistsInW9Form = recordExistsInW9Form
            });
        }

        [HttpPost]
        public IActionResult DeleteRecord()
        {
            string emailAddress = HttpContext.Session.GetString("ClientEmail") ?? string.Empty;
            bool recordExistsInW8EBENEForm = _evolvedtaxContext.TblW8ebeneforms.Any(e => e.W8beneemailAddress == emailAddress);
            bool recordExistsInW8IMYForm = _evolvedtaxContext.TblW8imyforms.Any(e => e.EmailAddress == emailAddress && e.IsActive == true);
            bool recordExistsInW8EXPForm = _evolvedtaxContext.TblW8expforms.Any(e => e.EmailAddress == emailAddress && e.IsActive == true);
            bool recordExistsInW8ECIForm = _evolvedtaxContext.TblW8eciforms.Any(e => e.W8eciemailAddress == emailAddress && e.IsActive == true);
            bool recordExistsInW9Form = _evolvedtaxContext.TblW9forms.Any(e => e.W9emailAddress == emailAddress && e.IsActive == true);
            bool recordExists = recordExistsInW8EBENEForm || recordExistsInW8IMYForm || recordExistsInW8EXPForm || recordExistsInW8ECIForm || recordExistsInW9Form;
        
            var recordGQ = _evolvedtaxContext.GeneralQuestionEntities.FirstOrDefault(p => p.UserName == emailAddress);
            if (recordExists)
            {
                if (recordGQ != null)
                {
                    recordGQ.W8formType = string.Empty;
                    recordGQ.FormType = string.Empty;
                    recordGQ.W8expId = 0;

                }
                if (recordExistsInW9Form)
                {
                    var record = _evolvedtaxContext.TblW9forms.FirstOrDefault(e => e.W9emailAddress == emailAddress && e.IsActive == true);
                    if (record != null)
                    {
                        record.IsActive = false;
                    }

                }

                if (recordExistsInW8EBENEForm)
                {
                    var record = _evolvedtaxContext.TblW8ebeneforms.FirstOrDefault(e => e.W8beneemailAddress == emailAddress && e.IsActive == true);
                    if (record != null)
                    {
                        record.IsActive = false;
                    }

                }
                if (recordExistsInW8IMYForm)
                {
                    var record = _evolvedtaxContext.TblW8imyforms.FirstOrDefault(e => e.EmailAddress == emailAddress && e.IsActive == true);
                    if (record != null)
                    {
                        record.IsActive = false;
                    }

                }
                if (recordExistsInW8EXPForm)
                {
                    var record = _evolvedtaxContext.TblW8expforms.FirstOrDefault(e => e.EmailAddress == emailAddress && e.IsActive == true);
                    if (record != null)
                    {
                        record.IsActive = false;
                    }

                }
                if (recordExistsInW8ECIForm)
                {
                    var record = _evolvedtaxContext.TblW8eciforms.FirstOrDefault(e => e.W8eciemailAddress == emailAddress && e.IsActive == true);
                    if (record != null)
                    {
                        record.IsActive = false;
                    }

                }

                _evolvedtaxContext.SaveChanges();
                return Ok();
            }

            return BadRequest("Record not found.");
        }
        #endregion

        [HttpGet]
        public IActionResult IsSsnitneinValid(string Ssnitnein)
        {
            // Perform your custom validation here.
            bool isValid = !string.IsNullOrEmpty(Ssnitnein) && (Ssnitnein.Length ==10 || Ssnitnein.Length == 11) ;

            return Json(isValid);
        }
        [HttpGet]
        public IActionResult IsUSTinValid(string US_TIN)
        {
            // Perform your custom validation here.
            bool isValid = !string.IsNullOrEmpty(US_TIN) && (US_TIN.Length == 10 || US_TIN.Length == 11);

            return Json(isValid);
        }
        [HttpGet]
        public IActionResult IsValueValid(string propertyName, string value)
        {
            bool isValid = false;

            if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(value))
            {
                // Perform custom validation based on propertyName and value
                if (propertyName == "US_TIN")
                {
                    isValid = value.Length >= 11;
                }
                // Add more cases for other property names if needed
            }

            return Json(isValid);
        }

    }
}