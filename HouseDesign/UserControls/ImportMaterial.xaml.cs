using HouseDesign.Classes;
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
    /// Interaction logic for ImportMaterial.xaml
    /// </summary>
    public partial class ImportMaterial : UserControl
    {
        public event EventHandler StatusUpdated;

        private Material importedMaterial;
        public bool IsEdited { get; set; }



        public String CurrencyName
        {
            get { return (String)GetValue(CurrencyNameProperty); }
            set { SetValue(CurrencyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrencyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrencyNameProperty =
            DependencyProperty.Register("CurrencyName", typeof(String), typeof(ImportMaterial));

        private Currency currency;

        
        public ImportMaterial(String title, Material currentMaterial, bool isReadOnly, bool isEdited, bool isProjectState)
        {
            InitializeComponent();
            mainGroupBox.Header = title;
            this.IsEdited = isEdited;
            if (isReadOnly)
            {
                textBoxName.IsReadOnly = true;
                textBoxDescription.IsReadOnly = true;
                textBoxPrice.IsReadOnly = true;
                btnCancel.IsEnabled = false;
                btnLoadMaterial.IsEnabled = false;
                btnOK.IsEnabled = false;
                groupBoxPreviewMaterial.Visibility = Visibility.Collapsed;
            }

            if (isProjectState)
            {
                currency = CurrencyHelper.GetProjectCurrency();
            }
            else
            {
                currency = CurrencyHelper.GetCurrentCurrency();
            }

            CurrencyName = currency.Name.ToString();

            if (currentMaterial != null)
            {
                this.importedMaterial = currentMaterial;
                InitializeCurrentMaterial();
            }
            else
            {
                importedMaterial = new Material();
            }

            
            
        }
        public void InitializeCurrentMaterial()
        {
            groupBoxPreviewMaterial.Visibility = Visibility.Visible;
            textBoxName.Text = importedMaterial.Name;
            textBoxDescription.Text = importedMaterial.Description;
            Decimal price = CurrencyHelper.FromCurrencyToCurrency(CurrencyHelper.GetCurrentCurrency(), importedMaterial.Price,
                currency);
            textBoxPrice.Text = Math.Round(price, 2).ToString();
            String imagePath = importedMaterial.FullPath;
            InitializeImage(imagePath);
        }

        public void InitializeImage(String imagePath)
        {
            Image imgMaterial = new Image();
            imgMaterial.Source = new BitmapImage((new Uri(imagePath)));
            imgMaterial.Tag = imagePath;
            imgMaterial.Width = 100;
            imgMaterial.Height = 100;
            Grid grid = new Grid();
            grid.Children.Add(imgMaterial);
            groupBoxPreviewMaterial.Content = grid;
        }

        private void btnLoadMaterial_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Import material";
            
            const string assetsDirectory = "Assets";
            fdlg.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", assetsDirectory));
            fdlg.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == true)
            {
                importedMaterial.FullPath = fdlg.FileName;
                InitializeImage(importedMaterial.FullPath);
                groupBoxPreviewMaterial.Visibility = Visibility.Visible;

            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxName.Text.Length == 0 || textBoxPrice.Text.Length == 0)
            {
                MessageBox.Show("Complete mandatory fields!");
                return;
            }
            
            if(FieldValidation.IsValidDecimalNumericField(textBoxPrice.Text)==false)
            {
                MessageBox.Show("Invalid value typed price! Type another!");
                return;
            }
            else
            {
                importedMaterial.Name = textBoxName.Text;
                importedMaterial.Description = textBoxDescription.Text;
                importedMaterial.Price = Convert.ToDecimal(textBoxPrice.Text);
                ClearAllFields();
                if (this.StatusUpdated != null)
                {
                    this.StatusUpdated(this, new EventArgs());
                }

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
        }

        private void ClearAllFields()
        {
            groupBoxPreviewMaterial.Visibility = Visibility.Collapsed;
            textBoxDescription.Clear();
            textBoxPrice.Clear();
            textBoxName.Clear();
        }

        public Material GetImportedMaterial()
        {
            return importedMaterial;
        }

        public void SetImportedMaterial(Material material)
        {
            this.importedMaterial = material;
        }
    }
}
