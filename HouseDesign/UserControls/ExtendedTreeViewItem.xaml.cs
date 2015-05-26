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
    /// Interaction logic for ExtendedTreeViewItem.xaml
    /// </summary>
    public partial class ExtendedTreeViewItem : UserControl
    {        
        public String IconPath { get; set; }
        public String HeaderName { get; set; }
        public String FullPath { get; set; }
        public ExtendedTreeViewItem(String iconPath, String name, String fullPath)
        {
            InitializeComponent();
            this.IconPath = iconPath;
            this.HeaderName = name;
            this.FullPath = fullPath;
            icon.Source = new BitmapImage(new Uri(this.IconPath));
            lblName.Content = HeaderName;
            
        }
    }
}
