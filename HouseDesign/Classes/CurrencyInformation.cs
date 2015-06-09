using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class CurrencyInformation
    {
        private List<Currency> currencies;

        public CurrencyInformation()
        {
            this.currencies = new List<Currency>();
        }
        public void SetCurrencies(List<Currency> currencies)
        {
            this.currencies = currencies;
        }

        public List<Currency> GetCurrencies()
        {
            return this.currencies;
        }
    }
}
