using Emgu.CV;
using Emgu.CV.Structure;
using HouseDesign.Classes;
using HouseDesign.ImageProcessing;
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
            currentHousePlan = new HousePlan("Current");
            InitializeComboBoxCurrentCurrency();
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
            fdlg.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\HousePlansImages";
            fdlg.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;

            if (fdlg.ShowDialog() == true)
            {
                Image<Bgr, byte> image = new Image<Bgr, byte>(fdlg.FileName);
                Image<Gray, byte> currentImage = image.Convert<Gray, byte>();
                currentImage = StandardOperation.Binarize(currentImage, 127);
                currentImage = StandardOperation.GetImageWithExtraPixels(currentImage, StandardOperation.BinaryColor.Black, 3);
                currentImage = Dilation.GetDilation(currentImage, 3);
                currentImage = Skeletation.GetProcessedImage(StandardOperation.Binarize(currentImage, 127), 0);
                currentImage = StandardOperation.Binarize(currentImage, 127);
                StandardOperation.Invert(currentImage);

                int halfWidth = currentImage.Width / 2;
                int halfHeight = currentImage.Height / 2;
                SortedList<double, Segment> segments = HoughLines.GetSegments(currentImage, 5);

                for (int i = 0; i < segments.Count - 1; i++)
                {
                    for (int j = i + 1; j < segments.Count; j++)
                    {
                        double value = segments.Values[i].GetDistance(segments.Values[j].BeginPoint);
                        Vector v1 = segments.Values[i].EndPoint - segments.Values[i].BeginPoint;
                        Vector v2 = segments.Values[j].EndPoint - segments.Values[j].BeginPoint;
                        double angle = Math.Acos((v1.X * v2.X + v1.Y * v2.Y) / (v1.Length * v2.Length));
                        if ((Math.Abs(Math.PI - angle) < 0.1 || Math.Abs(angle) < 0.1))
                        {
                            if (value > 5 && value < 12 && StandardOperation.Intersect(segments.Values[i], segments.Values[j]))
                            {
                                List<Point> topPoints = new List<Point>();
                                topPoints.Add(segments.Values[i].BeginPoint);
                                topPoints.Add(segments.Values[i].EndPoint);
                                topPoints.Add(segments.Values[i].GetProjection(segments.Values[j].BeginPoint));
                                topPoints.Add(segments.Values[i].GetProjection(segments.Values[j].EndPoint));

                                List<Point> bottomPoints = new List<Point>();
                                bottomPoints.Add(segments.Values[j].GetProjection(segments.Values[i].BeginPoint));
                                bottomPoints.Add(segments.Values[j].GetProjection(segments.Values[i].EndPoint));
                                bottomPoints.Add(segments.Values[j].BeginPoint);
                                bottomPoints.Add(segments.Values[j].EndPoint);

                                int bestI = -1;
                                int bestJ = -1;
                                double bestLength = 0;

                                for (int k = 0; k < 3; k++)
                                {
                                    for (int p = k + 1; p < 4; p++)
                                    {
                                        double length = Math.Sqrt((topPoints[p].Y - topPoints[k].Y) * (topPoints[p].Y - topPoints[k].Y) + (topPoints[p].X - topPoints[k].X) * (topPoints[p].X - topPoints[k].X));
                                        if (length > bestLength)
                                        {
                                            bestI = k;
                                            bestJ = p;
                                            bestLength = length;
                                        }
                                    }
                                }
                                Point3d p1 = new Point3d(Convert.ToSingle((topPoints[bestI].X - halfWidth) * 10), 0, Convert.ToSingle((topPoints[bestI].Y - halfHeight) * 10));
                                Point3d p2 = new Point3d(Convert.ToSingle((topPoints[bestJ].X - halfWidth) * 10), 0, Convert.ToSingle((topPoints[bestJ].Y - halfHeight) * 10));
                                Point3d p3 = new Point3d(Convert.ToSingle((bottomPoints[bestJ].X - halfWidth) * 10), 0, Convert.ToSingle((bottomPoints[bestJ].Y - halfHeight) * 10));
                                Point3d p4 = new Point3d(Convert.ToSingle((bottomPoints[bestI].X - halfWidth) * 10), 0, Convert.ToSingle((bottomPoints[bestI].Y - halfHeight) * 10));
                                Wall wall = new Wall(p1, p2, p3, p4);
                                currentHousePlan.AddWall(wall);
                                break;

                            }
                        }
                        else
                        {
                            break;
                        }

                    }

                }
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

        private void btnSetCurrency_Click(object sender, RoutedEventArgs e)
        {
            comboBoxCurrencies.Visibility = Visibility.Visible;
        }

        private void comboBoxCurrencies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem currentItem = comboBoxCurrencies.SelectedItem as ComboBoxItem;
            if (currentItem.Content !=null)
            {
                Currency.CurrencyName currencyName = (Currency.CurrencyName)Enum.Parse(typeof(Currency.CurrencyName), currentItem.Content.ToString());
                CurrencyHelper.SetProjectCurrency(CurrencyHelper.GetCurrencyByName(currencyName));
            }            
        }

        private void InitializeComboBoxCurrentCurrency()
        {

            comboBoxCurrencies.SelectedValue = CurrencyHelper.GetCurrentCurrency().Name.ToString();
        }
    }
}
