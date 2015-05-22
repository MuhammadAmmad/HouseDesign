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
    /// Interaction logic for ExtendedMenuItem.xaml
    /// </summary>
    public partial class ExtendedMenuItem : UserControl
    {

        public String ImagePath { get; set; }
        public String Title { get; set; }
        public ExtendedMenuItem()
        {

        }
        public ExtendedMenuItem(String imagePath, String name)
        {
            InitializeComponent();
            this.ImagePath = imagePath;
            this.Title = name;
            InitializeImage();
            
        }

        public void InitializeImage()
        {
            this.image.Source = new BitmapImage((new Uri(ImagePath)));
            this.image.Tag = ImagePath;
            this.name.Content = this.Title;

        }
    }
}
