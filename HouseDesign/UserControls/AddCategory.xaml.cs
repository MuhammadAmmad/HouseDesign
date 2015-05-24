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
        public Category currentCategory { get; set; }

        public event EventHandler StatusUpdated;

        private Image defaultIcon;
        public AddCategory(String title, Category category)
        {
            InitializeComponent();
            mainGroupBox.Header = title;            
            InitializeIcons();
            InitializeDefaultIcon(@"D:\Licenta\HouseDesign\HouseDesign\Images\defaultIcon.png");
            if (category != null)
            {
                InitializeCategory(category);
            }
        }

        private void InitializeDefaultIcon(String path)
        {
            defaultIcon = new Image();
            defaultIcon.Source = new BitmapImage((new Uri(path)));
            defaultIcon.Tag = path;
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(textBoxName.Text.Length==0 || textBoxTradeAllowance.Text.Length==0)
            {
                MessageBox.Show("Complete mandatory fields!");
                return;
            }
            Image icon;
            if(listViewIcons.SelectedItem!=null)
            {
                icon = listViewIcons.SelectedItem as Image;
            }
            else
            {
                icon = defaultIcon;
            }
            
            currentCategory = new Category(textBoxName.Text, icon.Tag.ToString(), textBoxDescription.Text, Convert.ToDouble(textBoxTradeAllowance.Text));
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
        }

        private void InitializeIcons()
        {
            icons = new List<Image>();
            string iconsDirectory = @"D:\Licenta\HouseDesign\HouseDesign\Icons";
            string[] images = System.IO.Directory.GetFiles(iconsDirectory);
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

        private void InitializeCategory(Category category)
        {
            textBoxName.Text = category.Name;
            textBoxDescription.Text = category.Description;
            textBoxTradeAllowance.Text=category.TradeAllowance.ToString();
            String iconPath = category.Path;

            //foreach (Image icon in icons)
            //{
            //    if (icon.Tag.ToString() == iconPath)
            //    {
            //        listViewIcons.SelectedItem = icon;
            //        break;
            //    }
            //}
        }
    }
}
