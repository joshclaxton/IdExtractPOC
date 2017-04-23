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
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.Extensions.Options;

namespace IdExtractPOC.Logic
{
    public class LicenseLogic
    {
        private AppSettings AppSettings { get; set; }
        public LicenseLogic(AppSettings appSettings)
        {
            AppSettings = appSettings;
        }
        public Address[] ExtractAddresses(SmartyStreetsAddressExtractResponse smartyAddressResponse)
        {
            if (smartyAddressResponse == null || smartyAddressResponse.meta.address_count == 0)
                return new Address[0];


            var potentialAddresses = new List<Address>();

            foreach (var smartyAddress in smartyAddressResponse.addresses)
            {
                Address address;
                address = new Address
                {
                    IsVerified = smartyAddress.verified,
                    FullAddress = smartyAddress.text
                };
                if (smartyAddress.verified)
                {
                    address.AddressLine = smartyAddress.api_output[0].delivery_line_1;
                    address.City = smartyAddress.api_output[0].components.city_name;
                    address.State = smartyAddress.api_output[0].components.state_abbreviation;
                    address.ZipCode = string.IsNullOrEmpty(smartyAddress.api_output[0].components.plus4_code) ? smartyAddress.api_output[0].components.zipcode : $"{smartyAddress.api_output[0].components.zipcode}-{smartyAddress.api_output[0].components.plus4_code}";
                }
                potentialAddresses.Add(address);

            };
            return potentialAddresses.ToArray();
        }

        public async Task<OCRResponse> ExtractTextFromImage(IFormFile file)
        {
            using (var client = new HttpClient())
            {
                //https://azure.microsoft.com/en-us/services/cognitive-services/
                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AppSettings.MicrosoftCognitiveServicesToken);

                // Request parameters
                var queryString = "language=unk&detectOrientation=true";
                var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + queryString;

                HttpResponseMessage response;

                var reader = new BinaryReader(file.OpenReadStream());
                byte[] byteData = reader.ReadBytes((int)file.Length);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);
                }

                var jsonText = await response.Content.ReadAsStringAsync();
                var ocrResponse = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<OCRResponse>(jsonText));

                return ocrResponse;
            }
        }

        public async Task<SmartyStreetsAddressExtractResponse> ExtractAddressFromText(string text)
        {
            using (var client = new HttpClient())
            {
                // Request parameters
                var queryString = $"auth-id={AppSettings.SmartyStreetAuthId}&auth-token={AppSettings.SmartyStreetAuthToken}";
                var uri = "https://us-extract.api.smartystreets.com/?" + queryString;

                HttpResponseMessage response;

                using (var content = new StringContent(text))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    response = await client.PostAsync(uri, content);
                }

                var jsonText = await response.Content.ReadAsStringAsync();
                var smartyStreetsExtractResponse = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<SmartyStreetsAddressExtractResponse>(jsonText));

                return smartyStreetsExtractResponse;
            }
        }

        public DateTime[] ExtractDates(IEnumerable<string> linesOfText)
        {
            List<DateTime> dates = new List<DateTime>();
            foreach (var lineOfText in linesOfText)
            {
                var match = Regex.Match(lineOfText, "[0-3]?[0-9][/\\.-][0-3]?[0-9][/\\.-](?:[0-9]{2})?[0-9]{2}");

                while (match.Success)
                {
                    dates.Add(DateTime.Parse(match.Value).Date);
                    match = match.NextMatch();
                }
            }
            return dates.ToArray();
        }

        public string[] ExtractNames(IEnumerable<string> linesOfText)
        {
            //most sophisticated name search ever made. 
            List<string> potentialNames = new List<string>();
            foreach (var line in linesOfText)
            {
                if (!line.Any(c => char.IsDigit(c)))
                    potentialNames.Add(line);
            }
            return potentialNames.ToArray();
        }
    }
}
