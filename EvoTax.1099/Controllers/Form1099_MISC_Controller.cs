using EvolvedTax.Business.Services.AnnouncementService;
using EvolvedTax.Business.Services.W9FormService;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EvolvedTax_1099.Controllers
{
    public class Form1099_MISC_Controller : BaseController
    {
        private readonly IForm1099_MISC_Service _form1099_MISC_Service;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Form1099_MISC_Controller(IForm1099_MISC_Service form1099_MISC_Service, IWebHostEnvironment webHostEnvironment)
        {
            _form1099_MISC_Service = form1099_MISC_Service;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View(_form1099_MISC_Service.GetForm1099MISCList());
        }
        [Route("Form1099_MISC_/uploadClients")]
        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file, int EntityId, string entityName)
        {
            if (file == null)
            {
                return Json(false);
            }
            var response = await _form1099_MISC_Service.Upload1099_MISC_Data(file, SessionUser.InstituteId, SessionUser.UserId);
            return Json(response);
        }

        [Route("Form1099_MISC_/downlodPdf")]
        [HttpGet]
        public IActionResult DownlodPdf(string Id)
        {
            var response = _form1099_MISC_Service.GeneratePdf(Id, _webHostEnvironment.WebRootPath);
            return Json(response);
        }

    }
}
