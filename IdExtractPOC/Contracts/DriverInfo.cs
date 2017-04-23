using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdExtractPOC.Contracts
{
    public class DriverInfo
    {
        public string[] PotentialNames { get; set; }
        public Address[] PotentialAddresses { get; set; }
        public DateTime[] PotentialDatesOfBirth { get; set; }
        public DriverInfo()
        {
            PotentialAddresses = new Address[0];
            PotentialNames = new string[0];
            PotentialDatesOfBirth = new DateTime[0];
        }
    }

    public class Address
    {
        public bool IsVerified { get; set; }
        public string FullAddress { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}
