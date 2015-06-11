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
    /// Interaction logic for HouseObjectUserControl.xaml
    /// </summary>
    public partial class HouseObjectUserControl : UserControl
    {


        public String Quantity
        {
            get { return (String)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Quantity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register("Quantity", typeof(String), typeof(HouseObjectUserControl));



        public String ObjectName
        {
            get { return (String)GetValue(ObjectNameProperty); }
            set { SetValue(ObjectNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ObjectName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ObjectNameProperty =
            DependencyProperty.Register("ObjectName", typeof(String), typeof(HouseObjectUserControl));


        public String InitialPrice
        {
            get { return (String)GetValue(InitialPriceProperty); }
            set { SetValue(InitialPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitialPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitialPriceProperty =
            DependencyProperty.Register("InitialPrice", typeof(String), typeof(HouseObjectUserControl));


        public String MaterialsPrice
        {
            get { return (String)GetValue(MaterialsPriceProperty); }
            set { SetValue(MaterialsPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaterialsPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaterialsPriceProperty =
            DependencyProperty.Register("MaterialsPrice", typeof(String), typeof(HouseObjectUserControl));



        public String TotalPrice
        {
            get { return (String)GetValue(TotalPriceProperty); }
            set { SetValue(TotalPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalPriceProperty =
            DependencyProperty.Register("TotalPrice", typeof(String), typeof(HouseObjectUserControl));

        
        
        
        public HouseObjectUserControl(int quantity, String objectName, String initialPrice, String materialsPrice, String totalPrice)
        {
            InitializeComponent();
            this.Quantity = quantity.ToString();
            this.ObjectName = objectName;
            this.InitialPrice = initialPrice;
            this.MaterialsPrice = materialsPrice;
            this.TotalPrice = totalPrice;
        }
    }
}
