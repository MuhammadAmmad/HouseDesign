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
    /// Interaction logic for AddCategory.xaml
    /// </summary>
    public partial class AddCategory : UserControl
    {
        private static readonly List<string> imageExtensions = new List<string> {
                                                                                   ".jpg",
                                                                                   ".jpe",
                                                                                   ".bmp",
                                                                                   ".gif",
                                                                                   ".png"
                                                                                };
        private List<Image> icons;
        public Category<FurnitureObject> currentCategory { get; set; }

        public event EventHandler StatusUpdated;

        private Image defaultIcon;

        private Image customIcon;
        public bool IsEdited { get; set; }
        public AddCategory(String title, Category<FurnitureObject> category, bool isReadOnly, bool isEdited, String iconsDirectoryPath)
        {
            InitializeComponent();
            mainGroupBox.Header = title;            
            InitializeIcons(iconsDirectoryPath);
            InitializeIcon(ref defaultIcon, @"D:\Licenta\HouseDesign\HouseDesign\Images\defaultIcon.png");
            IsEdited = isEdited;
            if (category != null)
            {
                InitializeCategory(category);
            }

            if(isReadOnly)
            {
                textBoxName.IsReadOnly = true;
                textBoxDescription.IsReadOnly = true;
                textBoxTradeAllowance.IsReadOnly = true;
                btnLoadIcon.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnOK.IsEnabled = false;
                listViewIcons.Visibility = Visibility.Collapsed;
            }
        }

        private void InitializeIcon(ref Image icon, String path)
        {
            icon = new Image();
            icon.Source = new BitmapImage((new Uri(path)));
            icon.Tag = path;
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(textBoxName.Text.Length==0 || textBoxTradeAllowance.Text.Length==0)
            {
                MessageBox.Show("Complete mandatory fields!");
                return;
            }
            Image icon;
            
            if (listViewIcons.SelectedItem != null)
            {
                icon = listViewIcons.SelectedItem as Image;
            }
            else
            {
                if(customIcon!=null)
                {
                    icon = customIcon;
                }
                else
                {
                    icon = defaultIcon;
                }
                
            }

            currentCategory = new Category<FurnitureObject>(textBoxName.Text, icon.Tag.ToString(), textBoxDescription.Text, Convert.ToDouble(textBoxTradeAllowance.Text));
            ClearAllFields();
            if (this.StatusUpdated != null)
            {
                this.StatusUpdated(this, new EventArgs());
            }                
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
        }

        public void ClearAllFields()
        {
            textBoxName.Text = "";
            textBoxDescription.Text = "";
            textBoxTradeAllowance.Text = "";
            listViewIcons.SelectedItem = null;
            customIcon = null;
        }

        private void InitializeIcons(String iconsDirectoryPath)
        {
            icons = new List<Image>();
            string[] images = System.IO.Directory.GetFiles(iconsDirectoryPath);
            images = images.Where(F => imageExtensions.Contains(System.IO.Path.GetExtension(F))).ToArray();

            foreach (string currentImage in images)
            {
                AddImageToListViewIcons(currentImage);
            }
            listViewIcons.ItemsSource = icons;

        }

        private void AddImageToListViewIcons(String image)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(image));
            img.Width = 32;
            img.Height = 32;
            img.Tag = image;
            icons.Add(img);
        }

        private void listViewIcons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void InitializeCategory(Category<FurnitureObject> category)
        {
            textBoxName.Text = category.Name;
            textBoxDescription.Text = category.Description;
            textBoxTradeAllowance.Text=category.TradeAllowance.ToString();
            InitializeIcon(ref customIcon, category.Path);
        }

        private void btnLoadIcon_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Load icon";
            dlg.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\Icons";
            dlg.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            dlg.FilterIndex = 2;
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == true)
            {
                string fullPath = System.IO.Path.GetFullPath(dlg.FileName);
                InitializeIcon(ref customIcon, fullPath);

            }
        }
    }
}
