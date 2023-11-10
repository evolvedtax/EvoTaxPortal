using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.DTOs.ViewModels;
using EvolvedTax.Data.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;

namespace EvolvedTax_Institute.Controllers
{
    public class W8Controller : Controller
    {
        readonly private IInstituteService _instituteService;
        readonly private IWebHostEnvironment _webHostEnvironment;
        readonly private IMailService _emailService;
        readonly ICommonService _commonService;
        readonly RoleManager<IdentityRole> _identityRoles;
        readonly UserManager<User> _userManager;
        readonly EvolvedtaxContext _evolvedtaxContext;

        public W8Controller(IInstituteService instituteService, IMailService emailService, IWebHostEnvironment webHostEnvironment, ICommonService commonService, EvolvedtaxContext evolvedtaxContext, RoleManager<IdentityRole> identityRoles, UserManager<User> userManager)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _commonService = commonService;
            _instituteService = instituteService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _identityRoles = identityRoles;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Entities", "Institute", new { area = "" });
        }

        public IActionResult ChangeEntity(int entityId)
        {
            HttpContext.Session.SetInt32("EntityId", entityId);
            return Json(new { Data = "true" }); ;
        }

        public IActionResult W8IMY()
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var entities = _instituteService.GetEntitiesByInstId(InstId);
            ViewBag.EntitiesList = entities.Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString()
            });
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            if (EntityId == 0)
            {
                var firstEntity = entities.FirstOrDefault();
                EntityId =Convert.ToInt32( firstEntity.EntityId.ToString());
            }

            var model = new InstituteClientViewModel { InstituteClientsResponse = _instituteService.GetClientByEntityIdAndFormName(InstId, AppConstants.W8IMYForm, EntityId) };
            HttpContext.Session.SetInt32("EntityId", 0);
            return View(model);
        }
        public IActionResult W8EXP()
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var entities = _instituteService.GetEntitiesByInstId(InstId);
            ViewBag.EntitiesList = entities.Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString()
            });
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            if (EntityId == 0)
            {
                var firstEntity = entities.FirstOrDefault();
                EntityId = Convert.ToInt32(firstEntity.EntityId.ToString());
            }

            var model = new InstituteClientViewModel { InstituteClientsResponse = _instituteService.GetClientByEntityIdAndFormName(InstId, AppConstants.W8EXPForm, EntityId) };
            HttpContext.Session.SetInt32("EntityId", 0);
            return View(model);
        }
        public IActionResult W8ECI()
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var entities = _instituteService.GetEntitiesByInstId(InstId);
            ViewBag.EntitiesList = entities.Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString()
            });
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            if (EntityId == 0)
            {
                var firstEntity = entities.FirstOrDefault();
                EntityId = Convert.ToInt32(firstEntity.EntityId.ToString());
            }
            var model = new InstituteClientViewModel { InstituteClientsResponse = _instituteService.GetClientByEntityIdAndFormName(InstId, AppConstants.W8ECIForm, EntityId) };
            HttpContext.Session.SetInt32("EntityId", 0);
            return View(model);
        }
        public IActionResult W9()
        {
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var entities = _instituteService.GetEntitiesByInstId(InstId);
            ViewBag.EntitiesList = entities.Select(p => new SelectListItem
            {
                Text = p.EntityName,
                Value = p.EntityId.ToString()
            });
            var EntityId = HttpContext.Session.GetInt32("EntityId") ?? 0;
            if (EntityId == 0)
            {
                var firstEntity = entities.FirstOrDefault();
                EntityId = Convert.ToInt32(firstEntity.EntityId.ToString());
            }

            var model = new InstituteClientViewModel { InstituteClientsResponse = _instituteService.GetClientByEntityIdAndFormName(InstId, AppConstants.W9Form, EntityId) };
            HttpContext.Session.SetInt32("EntityId", 0);
            return View(model);
        }
        public IActionResult DownloadExcel(string fileType)
        {
            string fileName;
            string filePath;

            switch (fileType)
            {
                case AppConstants.Entity:
                    fileName = AppConstants.InstituteEntityTemplate;
                    break;
                case AppConstants.Client:
                    fileName = AppConstants.InstituteClientTemplate;
                    break;
                default:
                    return NotFound();
            }

            filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Templates", fileName);

            if (System.IO.File.Exists(filePath))
            {
                var memoryStream = _commonService.DownloadFile(filePath);
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else
            {
                return NotFound();
            }

        }
    }
}
