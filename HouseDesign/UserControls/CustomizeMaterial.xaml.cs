﻿using HouseDesign.Classes;
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
    /// Interaction logic for CustomizeMaterial.xaml
    /// </summary>
    public partial class CustomizeMaterial : UserControl
    {
        public String MaterialName
        {
            get { return (String)GetValue(MaterialNameProperty); }
            set { SetValue(MaterialNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaterialName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaterialNameProperty =
            DependencyProperty.Register("MaterialName", typeof(String), typeof(CustomizeMaterial));


        public Decimal InitialPrice
        {
            get { return (Decimal)GetValue(InitialPriceProperty); }
            set { SetValue(InitialPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitialPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitialPriceProperty =
            DependencyProperty.Register("InitialPrice", typeof(Decimal), typeof(CustomizeMaterial));



        public String ImagePath
        {
            get { return (String)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImagePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(String), typeof(CustomizeMaterial));

        

        public Double SurfaceNeeded
        {
            get { return (Double)GetValue(SurfaceNeededProperty); }
            set { SetValue(SurfaceNeededProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SurfaceNeeded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SurfaceNeededProperty =
            DependencyProperty.Register("SurfaceNeeded", typeof(Double), typeof(CustomizeMaterial));



        public Decimal TotalPrice
        {
            get { return (Decimal)(Convert.ToDecimal(SurfaceNeeded)*InitialPrice); }
            set { SetValue(TotalPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalPriceProperty =
            DependencyProperty.Register("TotalPrice", typeof(Decimal), typeof(CustomizeMaterial));

        public int Index { get; set; }

        private Material currentMaterial;

        public event EventHandler StatusUpdated;

        private Currency currentCurrency;
        
        public CustomizeMaterial(int index, Material material, double surfaceNeeded, bool canBeImported, bool isConfigurationView)
        {
            InitializeComponent();
            this.Index = index;
            //this.MaterialName = name;
            this.ImagePath = material.FullPath;
            //this.InitialPrice = initialPrice;
            //this.SurfaceNeeded = surfaceNeeded;
            if(isConfigurationView)
            {
                currentCurrency = CurrencyHelper.GetCurrentCurrency();
            }
            else
            {
                currentCurrency = CurrencyHelper.GetProjectCurrency();
            }
            if(material!=null)
            {
                currentMaterial = material;
                InitializeCurrentMaterial(currentMaterial, surfaceNeeded);
            }
            if (canBeImported)
            {
                btnImport.Visibility = Visibility.Visible;
            }
        }

        public void InitializeCurrentMaterial(Material currentMaterial, double surfaceNeeded)
        {
            lblMaterialName.Content = currentMaterial.Name;
            //Currency projectCurrency = CurrencyHelper.GetProjectCurrency();
            Decimal actualPrice=CurrencyHelper.FromCurrencyToCurrency(CurrencyHelper.GetCurrentCurrency(), currentMaterial.Price, currentCurrency);
            textBlockInitialPrice.Text = string.Format("{0:0.000}", actualPrice);
            textBlockSurfaceNeeded.Text = string.Format("{0:0.000}", surfaceNeeded);
            Decimal actualTotalPrice=CurrencyHelper.FromCurrencyToCurrency(CurrencyHelper.GetCurrentCurrency(), (currentMaterial.Price) * Convert.ToDecimal(surfaceNeeded), currentCurrency);
            textBlockTotalPrice.Text = string.Format("{0:0.000}", actualTotalPrice);
            imgMaterial.Source = new BitmapImage(new Uri(currentMaterial.FullPath));
        }
        public Material GetCurrentMaterial()
        {
            return currentMaterial;
        }

        public void SetCurrentMaterial(Material material)
        {
            currentMaterial = material;
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (this.StatusUpdated != null)
            {
                this.StatusUpdated(this, new EventArgs());
            }
        }
    }
}
