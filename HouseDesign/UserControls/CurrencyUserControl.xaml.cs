using HouseDesign.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HouseDesign.UserControls
{
    /// <summary>
    /// Interaction logic for CurrencyUserControl.xaml
    /// </summary>
    public partial class CurrencyUserControl : UserControl
    {


        public String CurrencyShortcut
        {
            get { return (String)GetValue(CurrencyShortcutProperty); }
            set { SetValue(CurrencyShortcutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrencyShortcut.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrencyShortcutProperty =
            DependencyProperty.Register("CurrencyShortcut", typeof(String), typeof(CurrencyUserControl));



        public String CurrencyName
        {
            get { return (String)GetValue(CurrencyNameProperty); }
            set { SetValue(CurrencyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrencyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrencyNameProperty =
            DependencyProperty.Register("CurrencyName", typeof(String), typeof(CurrencyUserControl));        

        public String CurrencyValue
        {
            get { return (String)GetValue(CurrencyValueProperty); }
            set { SetValue(CurrencyValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrencyValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrencyValueProperty = 
            DependencyProperty.Register("CurrencyValue", typeof(String), typeof(CurrencyUserControl));



        public String RelativeCurrency
        {
            get { return (String)GetValue(RelativeCurrencyProperty); }
            set { SetValue(RelativeCurrencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RelativeCurrency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RelativeCurrencyProperty =
            DependencyProperty.Register("RelativeCurrency", typeof(String), typeof(CurrencyUserControl));

        



        

        
        public CurrencyUserControl(Currency currentCurrency, Currency relativeCurrency)
        {
            InitializeComponent();
            this.CurrencyShortcut = currentCurrency.Name.ToString();
            int value=(int)currentCurrency.Name;
            this.CurrencyName = Enum.GetName(typeof(Currency.InternationalName), value);
            this.RelativeCurrency = relativeCurrency.Name.ToString();
            this.CurrencyValue = currentCurrency.Value.ToString();
        }

        public CurrencyUserControl(String header1, String header2, String header3, String header4)
        {
            InitializeComponent();
            this.CurrencyShortcut = header1;
            this.CurrencyName = header2;
            this.CurrencyValue = header3;
            this.RelativeCurrency = header4;
        }
    }
}
