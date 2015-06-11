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
    /// Interaction logic for MaterialUserControl.xaml
    /// </summary>
    public partial class MaterialUserControl : UserControl
    {


        public String MaterialName
        {
            get { return (String)GetValue(MaterialNameProperty); }
            set { SetValue(MaterialNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaterialName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaterialNameProperty =
            DependencyProperty.Register("MaterialName", typeof(String), typeof(MaterialUserControl));


        public String Price
        {
            get { return (String)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Price.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PriceProperty =
            DependencyProperty.Register("Price", typeof(String), typeof(MaterialUserControl));


        public String SurfaceNeeded
        {
            get { return (String)GetValue(SurfaceNeededProperty); }
            set { SetValue(SurfaceNeededProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SurfaceNeeded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SurfaceNeededProperty =
            DependencyProperty.Register("SurfaceNeeded", typeof(String), typeof(MaterialUserControl));


        public String TotalPrice
        {
            get { return (String)GetValue(TotalPriceProperty); }
            set { SetValue(TotalPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalPriceProperty =
            DependencyProperty.Register("TotalPrice", typeof(String), typeof(MaterialUserControl));

        
        
        
        
        public MaterialUserControl(String imagePath, String materialName, String price, String surfaceNeeded, String totalPrice)
        {
            InitializeComponent();
            if(imagePath.Length>0)
            {
                BitmapImage img = new BitmapImage(new Uri(imagePath));
                imgMaterial.Source = img;
                
            }

            this.MaterialName = materialName;
            this.Price = price;
            this.SurfaceNeeded = surfaceNeeded;
            this.TotalPrice = totalPrice;
        }
    }
}
