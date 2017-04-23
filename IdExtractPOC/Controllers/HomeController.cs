using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using IdExtractPOC.Contracts;
using Newtonsoft.Json;
using System.Linq;
using IdExtractPOC.Logic;
using Microsoft.Extensions.Options;

namespace IdExtractPOC.Controllers
{
    public class HomeController : Controller
    {
        private AppSettings AppSettings { get; set; }
        public HomeController(IOptions<AppSettings> appSettings)
        {
            AppSettings = appSettings.Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            var licenseLogic = new LicenseLogic(AppSettings);

            //Read image
            var ocrResponse = await licenseLogic.ExtractTextFromImage(file);

            //useful variables
            var driversLicenseLines = ocrResponse.regions.SelectMany(r => r.lines.Select(l => l.words.Select(w => w.text).Aggregate((runningResult, next) => $"{runningResult} {next}"))).ToList();

            //Extract Address Logic

            var plainTextDriversLicense = driversLicenseLines.Aggregate((runningResult, next) => $"{runningResult} {next}");
            var smartyAddresses = await licenseLogic.ExtractAddressFromText(plainTextDriversLicense);
            var potentialAddresses = licenseLogic.ExtractAddresses(smartyAddresses);

            //Extract Dates
            var potentialDatesOfBirth = licenseLogic.ExtractDates(driversLicenseLines);
            //remove any dates that are outside of youngest learner permit range
            potentialDatesOfBirth = potentialDatesOfBirth.Where(d => d <= DateTime.UtcNow.Date.AddYears(-14)).ToArray();

            //extract name
            var potentialNames = licenseLogic.ExtractNames(driversLicenseLines);

            var driverInfo = new DriverInfo()
            {
                PotentialDatesOfBirth = potentialDatesOfBirth,
                PotentialNames = potentialNames,
                PotentialAddresses = potentialAddresses
            };

            return View(driverInfo);
        }
    }
}
