using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdExtractPOC
{
    public class AppSettings
    {
        public AppSettings() { }
        public string MicrosoftCognitiveServicesToken { get; set; }
        public string SmartyStreetAuthId { get; set; }
        public string SmartyStreetAuthToken { get; set; }
    }
}
