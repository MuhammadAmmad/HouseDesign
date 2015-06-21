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
using System.Windows.Shapes;

namespace HouseDesign
{
    /// <summary>
    /// Interaction logic for EditProperties.xaml
    /// </summary>
    public partial class EditProperties : Window
    {
        private Project currentProject;
        public EditProperties(Project currentProject)
        {
            InitializeComponent();
            this.currentProject = currentProject;
            if (currentProject != null)
            {
                InitializeCurrentProject();
            }
        }

        private void InitializeCurrentProject()
        {
            if(currentProject.IsEmpty==false)
            {
                projectProperties.textBoxClientName.Text = currentProject.Client.Name;
                projectProperties.textBoxEmailAddress.Text = currentProject.Client.EmailAddress;
                projectProperties.textBoxTelephoneNumber.Text = currentProject.Client.TelephoneNumber.ToString();
                projectProperties.textBoxBudget.Text = currentProject.Budget.ToString();
                projectProperties.textBoxNotes.Text = currentProject.Notes;
                projectProperties.comboBoxCurrencies.SelectedValue = currentProject.Currency.Name.ToString();
                projectProperties.comboBoxMeasurementUnits.SelectedValue = currentProject.MeasurementUnit.ToString();
                float wallsHeight = currentProject.WallsHeight;
                if (currentProject.MeasurementUnit == Project.UnitOfMeasurement.cm)
                {
                    wallsHeight /= 10;
                }
                else
                {
                    if (currentProject.MeasurementUnit == Project.UnitOfMeasurement.m)
                    {
                        wallsHeight /= 1000;
                    }
                }
                projectProperties.textBoxWallsHeight.Text = wallsHeight.ToString();
            }
            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            currentProject.Budget = Convert.ToDecimal(projectProperties.textBoxBudget.Text);
            currentProject.Client.Name = projectProperties.textBoxClientName.Text;
            currentProject.Client.EmailAddress = projectProperties.textBoxEmailAddress.Text;
            currentProject.Client.TelephoneNumber = Convert.ToInt64(projectProperties.textBoxTelephoneNumber.Text);
            currentProject.Currency = CurrencyHelper.GetProjectCurrency();
            currentProject.Notes = projectProperties.textBoxNotes.Text;
            
            currentProject.MeasurementUnit = (Project.UnitOfMeasurement)projectProperties.comboBoxMeasurementUnits.SelectedIndex ;
            float wallsHeight = Convert.ToSingle(projectProperties.textBoxWallsHeight.Text);
            if (currentProject.MeasurementUnit == Project.UnitOfMeasurement.cm)
            {
                wallsHeight *= 10;
            }
            else
            {
                if (currentProject.MeasurementUnit == Project.UnitOfMeasurement.m)
                {
                    wallsHeight *= 1000;
                }
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
