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
    /// Interaction logic for CustomizeHeader.xaml
    /// </summary>
    public partial class CustomizeHeader : UserControl
    {
        public CustomizeHeader(String firstColumn, String secondColumn, String thridColumn, String fourthColumn, String fifthColumn)
        {
            InitializeComponent();
            lblFirstColumn.Content = firstColumn;
            lblSecondColumn.Content = secondColumn;
            lblThirdColumn.Content = thridColumn;
            lblFourthColumn.Content = fourthColumn;
            lblFifthColumn.Content = fifthColumn;
        }
    }
}
