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
    /// Interaction logic for About_Box.xaml
    /// </summary>
    public partial class About_Box : Window
    {
        private String description;
        public About_Box()
        {
            InitializeComponent();
            description = "House Design is a 3D software used for Interior Design. Its main purpose is to improve the quality of a furnishing process.";
            textBlockDescription.Text = description;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
