using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.Form1042Services;
using EvolvedTax.Common.Constants;
using EvolvedTax.Web.Controllers;

namespace EvolvedTax_Institute.Areas._1042.Controllers
{
    [Area("1042")]
    public class CommonController : BaseController
    {
        private readonly ICommonService _commonService;
        private readonly IForm1042_S_Service _form1042_S_Service;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CommonController(IWebHostEnvironment webHostEnvironment, ICommonService commonService,
            IForm1042_S_Service form1042_S_Service)
        {
            _webHostEnvironment = webHostEnvironment;
            _commonService = commonService;
            _form1042_S_Service = form1042_S_Service;
        }
        [HttpGet]
        public IActionResult DownloadExcel(string fileType)
        {
            string fileName;
            string filePath;

            switch (fileType)
            {
                case AppConstants.Form1042S:
                    fileName = AppConstants.Form1042SExcelTemplate;
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
        [Route("common/SendEmailstoRecipients")]
        [HttpPost]
        public async Task<IActionResult> SendLinkToRecipients(int[] selectedValues, string form)
        {
            var instId = HttpContext.Session.GetInt32("InstId") ?? 0;
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = string.Empty;
            if (_webHostEnvironment.IsDevelopment())
            {
                //host = HttpContext.Request.Host.Value;
                host = "localhost:7163";
            }
            else
            {
                host = URLConstants.ClientUrl; // Hostname (e.g., example.com)
            }
            //host = HttpContext.Request.Host.Value; // Comment this line if the project is other than institute
            var fullUrl = $"{scheme}://{host}";
            string URL = string.Concat(fullUrl, "/AuthRecVerify/Account", "/OTP");
            switch (form)
            {
                case AppConstants.Form1042S:
                    await _form1042_S_Service.SendEmailToRecipients(selectedValues, URL, AppConstants.Form1042S, instId);
                    break;
            }

            return Json(new { type = ResponseMessageConstants.SuccessStatus, message = ResponseMessageConstants.SuccessEmailSend });
        }
    }
}
