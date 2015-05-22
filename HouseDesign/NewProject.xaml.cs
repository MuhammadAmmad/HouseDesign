using HouseDesign.Classes;
using HouseDesign.UserControls;
using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace HouseDesign
{
    /// <summary>
    /// Interaction logic for NewProject.xaml
    /// </summary>
    public partial class NewProject : Window
    {
        public HousePlan currentHousePlan { get; set; }
        private List<HousePlan> housePlans;
        private float height;
        private String housePlansDirectory;
        public NewProject(String title)
        {
            InitializeComponent();
            this.Title = title;
            housePlansDirectory = @"D:\Licenta\HouseDesign\HouseDesign\HousePlans";
            housePlans = new List<HousePlan>();
            InitializeHousePlans();
        }

        public void InitializeHousePlans()
        {
            try
            {
                string[] files = Directory.GetFiles(housePlansDirectory, "*.hpl");
                foreach (string file in files)
                {
                    String[] tokens = file.Split('.');
                    String[] currentDirectoryPath = tokens[0].Split('\\');
                    String fileName = currentDirectoryPath[currentDirectoryPath.GetLength(0)-1];
                    HousePlan housePlan = new HousePlan(fileName);
                    housePlans.Add(housePlan);
                    HousePlanControl housePlanControl = new HousePlanControl(housePlan);
                    
                    listViewHousePlans.Items.Add(housePlanControl);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(textBoxWallsHeight.Text.Length==0)
            {
                MessageBox.Show("Type a value for height!");
                return;
            }
            else
            {
                float h = Convert.ToSingle(textBoxWallsHeight.Text);
                if(comboBoxMeasurementUnits.Text=="m")
                {
                    h *= 1000;
                }
                if(comboBoxMeasurementUnits.Text=="cm")
                {
                    h *= 10;
                }
                this.height = h;
            }

            if(this.currentHousePlan.GetWalls().Count==0)
            {
                MessageBox.Show("Choose house plan to load!");
                return;
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Import house plan";
            fdlg.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\HousePlans";
            fdlg.Filter = "Image files (*.hpl)|*.hpl";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            
            if (fdlg.ShowDialog() == true)
            {
                //string fullPath = System.IO.Path.GetFullPath(fdlg.FileName);
                //try
                //{
                //    using (StreamReader sr = new StreamReader(fullPath))
                //    {
                //        while(!sr.EndOfStream)
                //        {
                //            String line = sr.ReadLine();
                //            String[] points = line.Split(' ');
                //            String[] point1 = points[0].Split(',');
                //            Point3d p1 = new Point3d(Convert.ToSingle(point1[0]), Convert.ToSingle(point1[1]), Convert.ToSingle(point1[2]));
                //            String[] point2 = points[1].Split(',');
                //            Point3d p2 = new Point3d(Convert.ToSingle(point2[0]), Convert.ToSingle(point2[1]), Convert.ToSingle(point2[2]));
                //            String[] point3 = points[2].Split(',');
                //            Point3d p3 = new Point3d(Convert.ToSingle(point3[0]), Convert.ToSingle(point3[1]), Convert.ToSingle(point3[2]));
                //            String[] point4 = points[3].Split(',');
                //            Point3d p4 = new Point3d(Convert.ToSingle(point4[0]), Convert.ToSingle(point4[1]), Convert.ToSingle(point4[2]));
                //            walls.Add(new Wall(p1, p2, p3, p4));
                //        }
                        
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("The file could not be read:");
                //    Console.WriteLine(ex.Message);
                //}

            }

            groupBoxWallsHeight.Visibility = Visibility.Visible;
        }        

        private void btnLoadPlan_Click(object sender, RoutedEventArgs e)
        {           
            HousePlanControl currentHousePlanControl = listViewHousePlans.SelectedItem as HousePlanControl;
            if(currentHousePlanControl==null)
            {
                MessageBox.Show("Select a house plan!");
                return;
            }
            currentHousePlan =currentHousePlanControl.GetCurrentHousePlan();
            groupBoxWallsHeight.Visibility = Visibility.Visible;
        }

        public float GetHeight()
        {
            return this.height;
        }

        private void listViewHousePlans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
