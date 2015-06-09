using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Currency
    {
        public CurrencyName Name { get; set; }
        public Decimal Value { get; set; }
        public Decimal Multiplier { get; set; }

        public Currency(CurrencyName name, Decimal value, Decimal multiplier)
        {
            this.Name = name;
            this.Value = value;
            this.Multiplier = multiplier;
        }

        public enum CurrencyName
        {
            AED,
            AUD,
            BGN,
            BRL,
            CAD,
            CHF,
            CNY,
            CZK,
            DKK,
            EGP,
            EUR,
            GBP,
            HUF,
            INR,
            JPY,
            KRW,
            MDL,
            MXN,
            NOK,
            NZD,
            PLN,
            RON,
            RSD,
            RUB,
            SEK,
            TRY,
            UAH,
            USD,
            XAU,
            XDR,
            ZAR
        };

        public enum InternationalName
        {
            United_Arab_Emirates_Dirham,
            Australian_Dolar,
            Bulgarian_Lev,
            Brazilian_Real,
            Canadian_Dolar,
            Swiss_Franc,
            Chinese_Yuan,
            Czech_Koruna,
            Danish_Krone,
            Egyptian_Pound,
            Euro,
            British_Pound,
            Hungarian_Forint,
            Indian_Rupee,
            Japanese_Yen,
            South_Korean_Won,
            Moldovan_Leu,
            Mexican_Peso,
            Norwegian_Krone,
            New_Zealand_Dollar,
            Polish_Złoty,
            Romanian_Leu,
            Serbian_Dinar,
            Russian_Ruble,
            Swedish_Krona,
            Turkish_Lira,
            Ukrainian_Hryvnia,
            United_States_Dollar,
            Gold_Ounce,
            DST,
            South_African_Rand
        };
    }
}
