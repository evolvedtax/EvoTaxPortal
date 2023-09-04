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
        public Form1099_MISC_Controller(IForm1099_MISC_Service form1099_MISC_Service)
        {
            _form1099_MISC_Service = form1099_MISC_Service;
        }
        public IActionResult Index()
        {
            return View();
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
    }
}
