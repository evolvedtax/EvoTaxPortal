

using EvolvedTax.Business.Services.GeneralQuestionareService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;

namespace EvolvedTax.Controllers
{
    [SessionTimeout]
    public class HomeController : Controller
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
            ViewBag.FileName = Path.Combine("Forms", "83b.pdf");
            return View();
        }
        public IActionResult TaxPayerDetails()
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