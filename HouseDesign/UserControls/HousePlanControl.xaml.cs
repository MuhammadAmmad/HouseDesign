using HouseDesign.Classes;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for HousePlanControl.xaml
    /// </summary>
    public partial class HousePlanControl : UserControl
    {
        private HousePlan currentHousePlan;
        private String imageHousePlanDirectory;
        public HousePlanControl(HousePlan housePlan)
        {
            InitializeComponent();
            this.currentHousePlan = housePlan;
            imageHousePlanDirectory=@"D:\Licenta\HouseDesign\HouseDesign\HousePlansImages";
            String iconPath = GetHousePlanImage();
            if(iconPath!=null)
            {
                imgHousePlan.Source = new BitmapImage(new Uri(iconPath));
            }
            else
            {
                //MessageBox.Show("Couldn't find the corresponding image for the current house plan!");
            }
        }

        public String GetHousePlanImage()
        {
            try
            {
                string[] files = Directory.GetFiles(imageHousePlanDirectory, "*.jpg");
                foreach (string file in files)
                {
                    String[] tokens = file.Split('.');
                    String [] currentPathDirectory = tokens[0].Split('\\');
                    String fileName = currentPathDirectory[currentPathDirectory.GetLength(0) - 1];
                    if(fileName==currentHousePlan.Name)
                    {
                        return file;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            return null;
        }

       
        public HousePlan GetCurrentHousePlan()
        {
            return currentHousePlan;
        }

    }
}
