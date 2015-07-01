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
        private Currency lastCurrency;
        public String CurrencyName
        {
            get { return (String)GetValue(CurrencyNameProperty); }
            set { SetValue(CurrencyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrencyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrencyNameProperty =
            DependencyProperty.Register("CurrencyName", typeof(String), typeof(ProjectPropertiesUserControl));

    
        public ProjectPropertiesUserControl(bool isEdited)
        {
            InitializeComponent();         
            
            if(isEdited)
            {
                textBoxWallsHeight.IsEnabled = false;
                comboBoxMeasurementUnits.IsEnabled = false;
                //groupBoxWallsHeight.Visibility = Visibility.Collapsed;
                btnSetCurrency.Visibility = Visibility.Collapsed;
                comboBoxCurrencies.IsEnabled = false;
                comboBoxCurrencies.Visibility = Visibility.Visible;
            }
            else
            {
                CurrencyHelper.SetProjectCurrency(CurrencyHelper.GetCurrentCurrency());
                lastCurrency = CurrencyHelper.GetProjectCurrency();
            }            
            InitializeComboBoxCurrentCurrency();
        }        

        private void btnSetCurrency_Click(object sender, RoutedEventArgs e)
        {
            comboBoxCurrencies.Visibility = Visibility.Visible;
        }

        private void InitializeBudget()
        {
            if(textBoxBudget.Text.Length>0)
            {
                Decimal budget = CurrencyHelper.FromCurrencyToCurrency(lastCurrency, Convert.ToDecimal(textBoxBudget.Text), CurrencyHelper.GetProjectCurrency());
                textBoxBudget.Text = Math.Round(budget, 2).ToString();
            }            
        }
        private void comboBoxCurrencies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem currentItem = comboBoxCurrencies.SelectedItem as ComboBoxItem;
            if (currentItem.Content != null)
            {
                lastCurrency = CurrencyHelper.GetProjectCurrency();
                Currency.CurrencyName currencyName = (Currency.CurrencyName)Enum.Parse(typeof(Currency.CurrencyName), currentItem.Content.ToString());
                if (currencyName == Currency.CurrencyName.RON)
                {
                    CurrencyHelper.SetProjectCurrency(CurrencyHelper.GetDefaultCurrency());
                }
                else
                {
                    CurrencyHelper.SetProjectCurrency(CurrencyHelper.GetCurrencyByName(currencyName));
                }
                //InitializeBudget();

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

        public bool CheckValidFields()
        {
            if (FieldValidation.IsValidDecimalNumericField(textBoxBudget.Text)==false)
            {
                MessageBox.Show("Invalid value typed for budget! Type another!");
                return false;
            }
            if (FieldValidation.IsValidFloatNumericField(textBoxWallsHeight.Text)==false)
            {
                MessageBox.Show("Invalid value typed for walls height! Type another!");
                return false;
            }
            if (FieldValidation.IsValidLongNumericField(textBoxTelephoneNumber.Text)==false)
            {
                MessageBox.Show("Invalid value typed for telephone number! Type another!");
                return false;
            }
            if (FieldValidation.IsValidEmail(textBoxEmailAddress.Text)==false)
            {
                MessageBox.Show("Invalid value typed for email! Type another!");
                return false;
            }

            return true;

        }
    }
}
