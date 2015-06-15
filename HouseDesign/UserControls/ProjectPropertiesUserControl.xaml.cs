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
    /// Interaction logic for ProjectPropertiesUserControl.xaml
    /// </summary>
    public partial class ProjectPropertiesUserControl : UserControl
    {

        private Project currentProject;
        public String CurrencyName
        {
            get { return (String)GetValue(CurrencyNameProperty); }
            set { SetValue(CurrencyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrencyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrencyNameProperty =
            DependencyProperty.Register("CurrencyName", typeof(String), typeof(ProjectPropertiesUserControl));

    
        public ProjectPropertiesUserControl()
        {
            InitializeComponent();         
            
            CurrencyHelper.SetProjectCurrency(CurrencyHelper.GetCurrentCurrency());
            InitializeComboBoxCurrentCurrency();
        }        

        private void btnSetCurrency_Click(object sender, RoutedEventArgs e)
        {
            comboBoxCurrencies.Visibility = Visibility.Visible;
        }

        private void comboBoxCurrencies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem currentItem = comboBoxCurrencies.SelectedItem as ComboBoxItem;
            if (currentItem.Content != null)
            {
                Currency.CurrencyName currencyName = (Currency.CurrencyName)Enum.Parse(typeof(Currency.CurrencyName), currentItem.Content.ToString());
                if (currencyName == Currency.CurrencyName.RON)
                {
                    CurrencyHelper.SetProjectCurrency(CurrencyHelper.GetDefaultCurrency());
                }
                else
                {
                    CurrencyHelper.SetProjectCurrency(CurrencyHelper.GetCurrencyByName(currencyName));
                }

            }            
        }
        private void InitializeComboBoxCurrentCurrency()
        {
            comboBoxCurrencies.SelectedValue = CurrencyHelper.GetCurrentCurrency().Name.ToString();
        }

        public bool CheckEmptyFields()
        {
            if(textBoxClientName.Text.Length==0 || textBoxBudget.Text.Length==0 || textBoxEmailAddress.Text.Length==0 || 
                textBoxTelephoneNumber.Text.Length==0 || textBoxWallsHeight.Text.Length==0 )
            {
                return true;
            }

            return false;
        }
    }
}
