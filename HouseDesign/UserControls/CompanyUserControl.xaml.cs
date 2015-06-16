using Microsoft.Win32;
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
    /// Interaction logic for CompanyUserControl.xaml
    /// </summary>
    public partial class CompanyUserControl : UserControl
    {
        public bool isEdited { get; set; }
        public CompanyUserControl()
        {
            InitializeComponent();
            isEdited = false;
        }

        public void SetReadOnlyFields()
        {
            textBoxCompanyName.IsReadOnly = true;
            textBoxAddress.IsReadOnly = true;
            textBoxTelephoneNumber.IsReadOnly = true;
            textBoxEmailAddress.IsReadOnly = true;
            textBoxWebsite.IsReadOnly = true;
            btnLoadLogo.IsEnabled = false;
        }

        public void UnsetReadOnlyFields()
        {
            textBoxCompanyName.IsReadOnly = false;
            textBoxAddress.IsReadOnly = false;
            textBoxTelephoneNumber.IsReadOnly = false;
            textBoxEmailAddress.IsReadOnly = false;
            textBoxWebsite.IsReadOnly = false;
            btnLoadLogo.IsEnabled = true;
        }

        private void btnLoadLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Load logo";
            dlg.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\Icons";
            dlg.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            dlg.FilterIndex = 2;
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == true)
            {
                groupBoxLogo.Visibility = Visibility.Visible;
                string fullPath = System.IO.Path.GetFullPath(dlg.FileName);
                imgLogo.Source = new BitmapImage(new Uri(fullPath));
                imgLogo.Tag = fullPath;

            }
        }
    }
}
