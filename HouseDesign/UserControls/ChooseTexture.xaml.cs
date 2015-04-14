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
    /// Interaction logic for ChooseTexture.xaml
    /// </summary>
    public partial class ChooseTexture : UserControl
    {
        public int Index { get; set; }
        public ChooseTexture(int index, String iconPath)
        {
            InitializeComponent();
            this.Index = index;
            lblTextureName.Content = "Texture" + index;
            iconTexture.Source = new BitmapImage(new Uri(iconPath));
        }
    }
}
