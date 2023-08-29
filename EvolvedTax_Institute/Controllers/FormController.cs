using System.Text;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.FormReport;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using Newtonsoft.Json.Converters;

namespace EvolvedTax_Institute.Controllers
{
    public class FormController : Controller
    {

        readonly private IInstituteService _instituteService;
        readonly private IFormReportService _formReportService;
        readonly private IWebHostEnvironment _webHostEnvironment;
        readonly private IMailService _emailService;
        readonly ICommonService _commonService;
        readonly EvolvedtaxContext _evolvedtaxContext;
        public FormController(IInstituteService instituteService, IMailService emailService,
            IWebHostEnvironment webHostEnvironment, ICommonService commonService,
            EvolvedtaxContext evolvedtaxContext, IFormReportService formReportService)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _commonService = commonService;
            _instituteService = instituteService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _formReportService = formReportService;
        }

        public IActionResult Report()
        {

            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;

            ViewBag.EntitiesList = _evolvedtaxContext.InstituteEntities.Where(p =>p.IsActive== RecordStatusEnum.Active).Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString()
            });
            ViewBag.FormTypes = new List<string>
            {
                AppConstants.W9Form,
                AppConstants.W8BENForm,
                AppConstants.W8ECIForm,
                AppConstants.W8BENEForm,
                AppConstants.W8IMYForm,
                AppConstants.W8EXPForm
            };
            return View(_formReportService.GetClientByInstituteId(InstId));

        }
        [HttpPost]
        public IActionResult Report(string formType,string Entities,string Status)
        {

           
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            ViewBag.FormTypes = new List<string>
        {
            AppConstants.W9Form,
            AppConstants.W8BENForm,
            AppConstants.W8ECIForm,
            AppConstants.W8BENEForm,
            AppConstants.W8IMYForm,
            AppConstants.W8EXPForm
        };
            ViewBag.EntitiesList = _evolvedtaxContext.InstituteEntities.Where(p => p.IsActive == RecordStatusEnum.Active).Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString()
            });
            // Load filtered data on POST request (after the form is submitted)
            var filteredData = _formReportService.GetClientByInstituteId(InstId, formType, Entities,Status);
            TempData["LastSelectedFormType"] = formType;
            TempData["LastSelectedEntities"] = Entities;
            TempData["LastSelectedStatus"] = Status;
            return View(filteredData);
        }
    }
}
