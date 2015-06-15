using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Client
    {
        public String Name { get; set; }
        public long TelephoneNumber { get; set; }
        public String EmailAddress { get; set; }

        public Client(String name, long telephoneNumber, String emailAddress)
        {
            this.Name = name;
            this.TelephoneNumber = telephoneNumber;
            this.EmailAddress = emailAddress;
        }
    }
}
