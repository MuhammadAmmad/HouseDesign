using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;

namespace HouseDesign.Classes
{
    public static class CurrencyHelper
    {
        private static String URLString = "http://www.bnr.ro/nbrfxrates.xml";
        private static String currenciesFileName = "currencies.crr";
        private static List<Currency> currencies;
        private static CurrencyInformation currencyInformation;
        private static Currency currentCurrency;
        private static Currency lastCurrency;
        private static Currency defaultCurrency = new Currency(Currency.CurrencyName.RON, 1, 1);
        private static Currency projectCurrency;

        public static List<Currency> GetCurrencies()
        {
            currencies = new List<Currency>();
            currencyInformation = new CurrencyInformation();

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(URLString);
                XmlNodeList elemList = doc.GetElementsByTagName("Rate");
                for (int i = 0; i < elemList.Count; i++)
                {
                    String s = elemList[i].Attributes["currency"].Value.ToString();
                    String v = elemList[i].InnerXml;
                    v = v.Replace('.', ',');
                    Decimal value = Convert.ToDecimal(v);
                    Currency.CurrencyName currencyName = (Currency.CurrencyName)Enum.Parse(typeof(Currency.CurrencyName), s);

                    Decimal multiplier = 1;
                    if (elemList[i].Attributes["multiplier"] != null)
                    {
                        multiplier = Convert.ToDecimal(elemList[i].Attributes["multiplier"].Value.ToString());
                    }
                    currencies.Add(new Currency(currencyName, value, multiplier));
                }
                currencyInformation.SetCurrencies(currencies);
                SerializeCurrencyInformation();
            }
            catch
            {
                DeserializeCurrencyInformation();
                currencies = currencyInformation.GetCurrencies();
            }   
            return currencies;
        }

        public static Decimal FromCurrencyToCurrency(Currency initialCurrency, Decimal initialValue, Currency actualCurrency)
        {
            Decimal result;
            Decimal defaultCurrencyValue = (initialValue * initialCurrency.Value) / initialCurrency.Multiplier;
            result = (actualCurrency.Multiplier * defaultCurrencyValue) / actualCurrency.Value;

            return result;

        }
        public static Currency GetCurrencyByName(Currency.CurrencyName name)
        {
            for(int i=0;i<currencies.Count;i++)
            {
                if(currencies[i].Name==name)
                {
                    return currencies[i];
                }
            }

            return null;
        }

        private static void SerializeCurrencyInformation()
        {
            using (Stream fileStream = new FileStream(currenciesFileName, FileMode.Create,
                           FileAccess.Write, FileShare.None))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, currencyInformation);
            }
        }

        private static void DeserializeCurrencyInformation()
        {
            using (Stream fileStream = new FileStream(currenciesFileName, FileMode.Open,
                           FileAccess.Read, FileShare.Read))
            {
                IFormatter formatter = new BinaryFormatter();
                currencyInformation = (CurrencyInformation)formatter.Deserialize(fileStream);
            }
        }

        public static void SetCurrentCurrency(Currency currency)
        {
            currentCurrency = currency;
        }

        public static Currency GetCurrentCurrency()
        {
            return currentCurrency;
        }

        public static void SetDefaultCurrency(Currency currency)
        {
            defaultCurrency = currency;
        }

        public static Currency GetDefaultCurrency()
        {
            return defaultCurrency;
        }

        public static void SetLastCurrency(Currency currency)
        {
            lastCurrency = currency;
        }

        public static Currency GetLastCurrency()
        {
            return lastCurrency;
        }

        public static void SetProjectCurrency(Currency currency)
        {
            projectCurrency = currency;
        }

        public static Currency GetProjectCurrency()
        {
            return projectCurrency;
        }
    }
}
