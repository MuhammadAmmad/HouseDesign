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
        public ImportMaterial(String title, Material currentMaterial, bool isReadOnly, bool isEdited)
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
            textBoxPrice.Text = importedMaterial.Price.ToString();
            String imagePath = importedMaterial.FullPath;
            InitializeImage(imagePath);
        }

        public void InitializeImage(String imagePath)
        {
            Image imgMaterial = new Image();
            imgMaterial.Source = new BitmapImage((new Uri(imagePath)));
            imgMaterial.Tag = imagePath;
            imgMaterial.Width = 75;
            imgMaterial.Height = 75;
            Grid grid = new Grid();
            grid.Children.Add(imgMaterial);
            groupBoxPreviewMaterial.Content = grid;
        }

        private void btnLoadMaterial_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Import material";
            fdlg.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\Assets";
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
    }
}
