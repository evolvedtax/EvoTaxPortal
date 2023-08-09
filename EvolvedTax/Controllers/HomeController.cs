

using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;

namespace EvolvedTax.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EvolvedtaxContext _evolvedtaxContext;
        private readonly IGeneralQuestionareService _generalQuestionareService;



        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, EvolvedtaxContext evolvedtaxContext,
             IGeneralQuestionareService generalQuestionareService)
        {

            _generalQuestionareService = generalQuestionareService;
            _evolvedtaxContext = evolvedtaxContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult OpenForm83()
        {
            return View();
        }
        [HttpPost]
        public IActionResult OpenForm83(string NumberOfShare, string PerShareRate)
        {
            var scheme = HttpContext.Request.Scheme; // "http" or "https"
            var host = HttpContext.Request.Host.Value; // Hostname (e.g., example.com)
            var fullUrl = $"{scheme}://{host}";
            ViewBag.FileName = string.Concat(fullUrl,"/Forms", "/83b.pdf");
            return View();
        }
        public IActionResult TaxPayerDetails()
        {
            return View();
        }
        public IActionResult EmailFrequency()
        {
            return View();
        }
        [HttpPost]
        public IActionResult TaxPayerDetails(string emailId)
        {
            return View(_generalQuestionareService.GetTaxpayerInfoByEmailId(emailId));
        }
    }
}