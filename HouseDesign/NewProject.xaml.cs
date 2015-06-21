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
        private String housePlansDirectory;
        private Project currentProject;
        private Configuration configuration;
        private String currentHousePlanName;
        public NewProject(String title, Configuration configuration)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Title = title;
            const string directory = "HousePlans";
            housePlansDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", directory));
            housePlans = new List<HousePlan>();
            InitializeHousePlans();
            this.configuration = configuration;
            currentProject = new Project(new Scene());

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
                    String fileName = currentDirectoryPath[currentDirectoryPath.GetLength(0) - 1];
                    HousePlan housePlan = new HousePlan(fileName);
                    housePlans.Add(housePlan);
                    HousePlanControl housePlanControl = new HousePlanControl(housePlan);
                    housePlanControl.MouseLeftButtonDown += housePlanControl_MouseLeftButtonDown;

                    listViewHousePlans.Items.Add(housePlanControl);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        void housePlanControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentHousePlan = (sender as HousePlanControl).GetCurrentHousePlan();
            String imagePath = (sender as HousePlanControl).GetHousePlanImagePath();
            PreviewPlan previewPlan = new PreviewPlan(imagePath);
            previewPlan.ShowDialog();
            btnCreateProject.IsEnabled = true;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Import house plan";

            const string directory = "HousePlansImages";
            const string housePlansDirectory = "HousePlans";
            fdlg.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", directory));
            fdlg.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;

            if (fdlg.ShowDialog() == true)
            {
                currentHousePlanName = System.IO.Path.GetFileNameWithoutExtension(fdlg.FileName);
                currentHousePlan = new HousePlan(currentHousePlanName);
                String currentHousePlanFileName = System.IO.Path.GetFullPath(string.Format(@"{0}\{1}.hpl",
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", housePlansDirectory), currentHousePlanName));
                Image<Bgr, byte> image = new Image<Bgr, byte>(fdlg.FileName);
                Image<Gray, byte> currentImage = image.Convert<Gray, byte>();
                List<Wall2D> walls = WallDetector.GetWalls(currentImage);
                int width = currentImage.Width;
                int height = currentImage.Height;
                double factor = 20000 / width;

                using (StreamWriter wr = new StreamWriter(currentHousePlanFileName))
                {
                    for (int i = 0; i < walls.Count; i++)
                    {
                        Point3d p1 = new Point3d(Convert.ToSingle((walls[i].Corners[0].X - width / 2) * factor), 0, Convert.ToSingle((walls[i].Corners[0].Y - height / 2) * factor));
                        Point3d p2 = new Point3d(Convert.ToSingle((walls[i].Corners[1].X - width / 2) * factor), 0, Convert.ToSingle((walls[i].Corners[1].Y - height / 2) * factor));
                        Point3d p3 = new Point3d(Convert.ToSingle((walls[i].Corners[2].X - width / 2) * factor), 0, Convert.ToSingle((walls[i].Corners[2].Y - height / 2) * factor));
                        Point3d p4 = new Point3d(Convert.ToSingle((walls[i].Corners[3].X - width / 2) * factor), 0, Convert.ToSingle((walls[i].Corners[3].Y - height / 2) * factor));
                        Wall wall = new Wall(p1, p2, p3, p4);
                        currentHousePlan.AddWall(wall);
                        wr.WriteLine((int)p1.X + "," + (int)p1.Y + "," + (int)p1.Z + " " + (int)p2.X + "," + (int)p2.Y + "," + (int)p2.Z + " " +
                            (int)p3.X + "," + (int)p3.Y + "," + (int)p3.Z + " " + (int)p4.X + "," + (int)p4.Y + "," + (int)p4.Z + " ");
                    }

                }

                btnCreateProject.IsEnabled = true;

            }
        }


        private void btnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            if (currentHousePlan != null)
            {
                if (currentHousePlan.GetWalls().Count == 0)
                {
                    HousePlanControl currentHousePlanControl = listViewHousePlans.SelectedItem as HousePlanControl;
                    if (currentHousePlanControl == null)
                    {
                        MessageBox.Show("Select a house plan!");
                        return;
                    }

                    currentHousePlan = currentHousePlanControl.GetCurrentHousePlan();
                }

                if (projectProperties.CheckEmptyFields() == true)
                {
                    MessageBox.Show("Complete mandatory fields!");
                    return;
                }

                List<Wall> walls = currentHousePlan.GetWalls();
                Project.UnitOfMeasurement measurementUnit = Project.UnitOfMeasurement.mm;
                float wallsHeight = Convert.ToSingle(projectProperties.textBoxWallsHeight.Text);

                if (projectProperties.comboBoxMeasurementUnits.Text == Project.UnitOfMeasurement.m.ToString())
                {
                    wallsHeight *= 1000;
                    measurementUnit = Project.UnitOfMeasurement.m;
                }
                if (projectProperties.comboBoxMeasurementUnits.Text == Project.UnitOfMeasurement.cm.ToString())
                {
                    wallsHeight *= 10;
                    measurementUnit = Project.UnitOfMeasurement.cm;
                }
                Client client = new Client(projectProperties.textBoxClientName.Text, Convert.ToInt64(projectProperties.textBoxTelephoneNumber.Text),
                    projectProperties.textBoxEmailAddress.Text);
                Decimal budget = Convert.ToDecimal(projectProperties.textBoxBudget.Text);
                String notes = projectProperties.textBoxNotes.Text;
                Scene scene = new Scene();
                scene.MainCamera.Translate = new Point3d(0, 500, 0);
                scene.MainCamera.Rotate = new Point3d(-90, 180, 0);
                for (int i = 0; i < walls.Count; i++)
                {
                    WallObject wall = new WallObject(walls[i], wallsHeight);
                    scene.AddWall(wall);
                }
                currentProject = new Project(client, scene, configuration, CurrencyHelper.GetProjectCurrency(), wallsHeight, budget,
                    notes, measurementUnit);

                this.Close();
            }

        }

        public Project GetCurrentProject()
        {
            return this.currentProject;
        }

        private void listViewHousePlans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
