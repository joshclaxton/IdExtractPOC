﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdExtractPOC.Contracts
{
    public class Meta
    {
        public int lines { get; set; }
        public bool unicode { get; set; }
        public int address_count { get; set; }
        public int verified_count { get; set; }
        public int bytes { get; set; }
        public int character_count { get; set; }
    }

    public class Components
    {
        public string primary_number { get; set; }
        public string street_predirection { get; set; }
        public string street_name { get; set; }
        public string street_suffix { get; set; }
        public string secondary_number { get; set; }
        public string secondary_designator { get; set; }
        public string city_name { get; set; }
        public string state_abbreviation { get; set; }
        public string zipcode { get; set; }
        public string plus4_code { get; set; }
        public string delivery_point { get; set; }
        public string delivery_point_check_digit { get; set; }
    }

    public class Metadata
    {
        public string record_type { get; set; }
        public string zip_type { get; set; }
        public string county_fips { get; set; }
        public string county_name { get; set; }
        public string carrier_route { get; set; }
        public string congressional_district { get; set; }
        public string rdi { get; set; }
        public string elot_sequence { get; set; }
        public string elot_sort { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string precision { get; set; }
        public string time_zone { get; set; }
        public int utc_offset { get; set; }
    }

    public class Analysis
    {
        public string dpv_match_code { get; set; }
        public string dpv_footnotes { get; set; }
        public string dpv_cmra { get; set; }
        public string dpv_vacant { get; set; }
        public string active { get; set; }
    }

    public class ApiOutput
    {
        public int input_index { get; set; }
        public int candidate_index { get; set; }
        public string delivery_line_1 { get; set; }
        public string last_line { get; set; }
        public string delivery_point_barcode { get; set; }
        public Components components { get; set; }
        public Metadata metadata { get; set; }
        public Analysis analysis { get; set; }
    }

    public class SmartyAddress
    {
        public string text { get; set; }
        public int line { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public bool verified { get; set; }
        public List<ApiOutput> api_output { get; set; }
    }

    public class SmartyStreetsAddressExtractResponse
    {
        public Meta meta { get; set; }
        public List<SmartyAddress> addresses { get; set; }
    }
}
