using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Company
    {
        public String CompanyName { get; set; }
        public String Address { get; set; }
        public long TelephoneNumber { get; set; }
        public String EmailAddress { get; set; }
        public String Website { get; set; }
        public String LogoPath { get; set; }
        public Company()
        {

        }
        public Company(String companyName, String address, long telephoneNumber, String emailAddress, String website, String logoPath)
        {
            this.CompanyName = companyName;
            this.Address = address;
            this.TelephoneNumber = telephoneNumber;
            this.EmailAddress = emailAddress;
            this.Website = website;
            this.LogoPath = logoPath;
        }
    }
}
