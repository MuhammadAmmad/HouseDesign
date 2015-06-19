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
    /// Interaction logic for PreviewPlan.xaml
    /// </summary>
    public partial class PreviewPlan : Window
    {
        public PreviewPlan(String imagePath)
        {
            InitializeComponent();
            imgCurrentHousePlan.Source = new BitmapImage(new Uri(imagePath));
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
