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
using SharpGL.SceneGraph;
using SharpGL;
using HouseDesign.Classes;
using HouseDesign.UserControls;
using System.Windows.Threading;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using System.ComponentModel;
using System.Xml;
using System.Xml.Linq;

namespace HouseDesign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //private WorldObject testObject;

        private WorldObject sceneObject;
        private bool isZoomedIn = false;
        private String configurationFileName;
        private Configuration configuration;
        private WorldObject currentObject;
        private int rotateCount;
        //private Decimal totalPrice;
        private float sceneHeight;
        public event PropertyChangedEventHandler PropertyChanged;
        private String currentProjectName;
        private List<Currency> currencies;
        private Project currentProject;
        public decimal TotalPrice
        {
            get { return (decimal)GetValue(TotalPriceProperty); }
            set { SetValue(TotalPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalPriceProperty =
            DependencyProperty.Register("TotalPrice", typeof(decimal), typeof(MainWindow));


        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            currentProject = new Project(new HouseDesign.Classes.Scene());

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            delta = 1000 / 40;
            timer.Interval = new TimeSpan(0, 0, 0, 0, delta);
            timer.Start();

            configurationFileName = "configuration.config";
            configuration = new Configuration();

            rotateCount = 0;
            TotalPrice = 0;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            isCollision = false;
            CurrencyHelper.InitializeCurrencies();
            CurrencyHelper.SetLastCurrency(CurrencyHelper.GetDefaultCurrency());

            if (IsEmptyConfiguration() == 0)
            {
                DeserializeConfiguration();
                CurrencyHelper.SetCurrentCurrency(configuration.CurrentCurrency);
            }
            else
            {
                MessageBox.Show("Welcome to HouseDesign! Please setup your configuration!");
                CurrencyHelper.SetCurrentCurrency(CurrencyHelper.GetDefaultCurrency());
                SetupConfiguration setupConfiguration = new SetupConfiguration("Setup configuration", configuration);
                setupConfiguration.ShowDialog();
            }

            CurrencyHelper.SetProjectCurrency(CurrencyHelper.GetDefaultCurrency());

        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public int IsEmptyConfiguration()
        {
            if (!File.Exists(configurationFileName))
            {
                MessageBox.Show("The configuration file does not exist!");
                return -1;
            }
            else
            {
                if (new FileInfo(configurationFileName).Length == 0)
                {
                    return 1;

                }

            }

            return 0;
        }

        private int delta;
        private float speed = 200 * 0.05f;

        private void SerializeConfiguration()
        {
            using (Stream fileStream = new FileStream(configurationFileName, FileMode.Create,
                           FileAccess.Write, FileShare.None))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, configuration);
            }
        }

        private void DeserializeConfiguration()
        {
            using (Stream fileStream = new FileStream(configurationFileName, FileMode.Open,
                           FileAccess.Read, FileShare.Read))
            {
                IFormatter formatter = new BinaryFormatter();
                configuration = (Configuration)formatter.Deserialize(fileStream);
            }
        }

        private void SerializeProject(String sceneFileName)
        {
            currentProject.ActualPrice = TotalPrice;
            using (Stream fileStream = new FileStream(sceneFileName, FileMode.Create,
                           FileAccess.Write, FileShare.None))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, currentProject);
            }
        }

        private void DeserializeProject(String sceneFileName)
        {
            using (Stream fileStream = new FileStream(sceneFileName, FileMode.Open,
                           FileAccess.Read, FileShare.Read))
            {
                IFormatter formatter = new BinaryFormatter();
                currentProject = (Project)formatter.Deserialize(fileStream);
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyUp(Key.Up))
            {
                currentProject.Scene.MainCamera.Translate += new Point3d(0, 0, speed);
            }
            if (Keyboard.IsKeyUp(Key.Down))
            {
                currentProject.Scene.MainCamera.Translate += new Point3d(0, 0, -speed);

                if (Keyboard.IsKeyUp(Key.Left))
                {
                    currentProject.Scene.MainCamera.Translate += new Point3d(-speed, 0, 0);
                }
                if (Keyboard.IsKeyUp(Key.Right))
                {
                    currentProject.Scene.MainCamera.Translate += new Point3d(speed, 0, 0);

                }
                //lblPosition.Content = scene.MainCamera.Translate.X + " " + scene.MainCamera.Translate.Y + " " + scene.MainCamera.Translate.Z + scene.MainCamera.Rotate.Y;
                //lblPosition.Content = "R "+scene.MainCamera.Rotate.X + " " + scene.MainCamera.Rotate.Y + " " + scene.MainCamera.Rotate.Z+
                //" T "+ scene.MainCamera.Translate.X+" "+scene.MainCamera.Translate.Y+" "+ scene.MainCamera.Translate.Z;
            }
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL object.
            //OpenGL gl = openGLControl.OpenGL;
            OpenGL gl = args.OpenGL as OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();

            currentProject.Scene.Render(gl);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            //OpenGL gl = openGLControl.OpenGL;
            OpenGL gl = args.OpenGL as OpenGL;

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            //OpenGL gl = openGLControl.OpenGL;
            OpenGL gl = args.OpenGL as OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(60.0f, (double)openGLControl.ActualWidth / (double)openGLControl.ActualHeight, 0.01, 10000.0);

            gl.Viewport(0, 0, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void menuItemDesign_Click(object sender, RoutedEventArgs e)
        {
            if (currentProject.Scene.IsEmpty() == false)
            {
                addExtendedMenuItems("Design");
                groupBoxExtendedMenu.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Create a new project or open an existing one!");
            }

        }

        private void menuItemNewProject_Click(object sender, RoutedEventArgs e)
        {
            NewProject project = new NewProject("New Project", configuration);
            project.ShowDialog();
            currentProject = project.GetCurrentProject();
            //if (currentProject != null)
            //{
            //    groupBoxCurrentProject.Visibility = Visibility.Visible;
            //    sceneHeight = currentProject.WallsHeight;
            //}

            if (currentProject.Scene.IsEmpty()==false)
            {
                groupBoxCurrentProject.Visibility = Visibility.Visible;
                groupBoxCurrentProject.Background = Brushes.Black;
                sceneHeight = currentProject.WallsHeight;
            }

        }

        private void menuItemOpenProject_Click(object sender, RoutedEventArgs e)
        {
            const string directory = "SavedProjects";
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Open project";
            fdlg.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", directory));
            fdlg.Filter = "HouseDesign Projects (.hds)|*.hds";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == true)
            {
                currentProjectName = fdlg.FileName;
                DeserializeProject(currentProjectName);
                if (currentProject.GetConfiguration().Equals(configuration))
                {
                    TotalPrice = currentProject.ActualPrice;
                    sceneHeight = currentProject.WallsHeight;
                    groupBoxCurrentProject.Visibility = Visibility.Visible;
                    groupBoxCurrentProject.Background = Brushes.Black;
                }
                else
                {
                    MessageBox.Show("The project can't be opened!");
                }

            }

        }

        private void menuItemSaveProject_Click(object sender, RoutedEventArgs e)
        {
            const string directory = "SavedProjects";
            if (currentProject.IsEmpty)
            {
                MessageBox.Show("There is no project to save!");
                return;
            }

            if (currentProjectName == null)
            {
                SaveFileDialog saveProject = new SaveFileDialog();
                saveProject.DefaultExt = ".hds";
                saveProject.Filter = "HouseDesign Projects (.hds)|*.hds";
                saveProject.Title = "Save Project";
                saveProject.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", directory));
                if (saveProject.ShowDialog() == true)
                {
                    SerializeProject(saveProject.FileName);
                }
            }
            else
            {
                SerializeProject(currentProjectName);
            }

        }
        private void menuItemSaveProjectAs_Click(object sender, RoutedEventArgs e)
        {
            const string directory = "SavedProjects";
            if (currentProject.IsEmpty)
            {
                MessageBox.Show("There is no project to save!");
                return;
            }

            SaveFileDialog saveProject = new SaveFileDialog();
            saveProject.DefaultExt = ".hds";
            saveProject.Filter = "HouseDesign Projects (.hds)|*.hds";
            saveProject.Title = "Save Project";
            saveProject.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", directory));
            if (saveProject.ShowDialog() == true)
            {
                SerializeProject(saveProject.FileName);
                currentProjectName = saveProject.FileName;
            }
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuItemResetHousePlan_Click(object sender, RoutedEventArgs e)
        {
            currentProject.Scene.ClearWalls();
            currentProject.Scene.ClearHouseObjects();
        }
        private void menuItemResetObjects_Click(object sender, RoutedEventArgs e)
        {
            currentProject.Scene.ClearHouseObjects();
        }

        private void addExtendedMenuItems(String menuType)
        {
            switch (menuType)
            {
                case "File":
                    {
                        break;
                    }
                case "Project":
                    {
                        break;
                    }
                case "Design":
                    {
                        menuExtended.Items.Clear();

                        for (int i = 0; i < configuration.Categories.Count; i++)
                        {
                            Category<FurnitureObject> currentCategory = configuration.Categories[i];
                            addExtendedMenuItem(currentCategory);
                        }
                        break;
                    }

            }

        }

        private void addExtendedMenuItem(Category<FurnitureObject> category)
        {
            ExtendedMenuItem item = new ExtendedMenuItem(category.Path, category.Name);
            item.Tag = category;
            menuExtended.Items.Add(item);
            item.MouseLeftButtonDown += menuItemDesign_MouseLeftButtonDown;
        }

        void menuItemDesign_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Category<FurnitureObject> currentCategory = ((ExtendedMenuItem)sender).Tag as Category<FurnitureObject>;
            GenericCategory wndDesign = new GenericCategory(currentCategory, configuration.Materials, sceneHeight, TotalPrice, currentProject.Budget, currentProject.MeasurementUnit);
            wndDesign.ShowDialog();
            if (currentProject.Scene.IsEmpty() == false)
            {
                groupBoxCurrentProject.Visibility = Visibility.Visible;
                groupBoxCurrentProject.Background = Brushes.Black;
                sceneObject = wndDesign.SelectedObject;
            }
            wndDesign.Dispose();

        }

        private void openGLControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        Point3d displace;
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point3d direction = GetClickDirection(e.GetPosition(openGLControl));

            currentObject = currentProject.Scene.GetCollisionObject(currentProject.Scene.MainCamera.Translate, direction);
            if (currentObject != null)
            {
                Point3d floorPoint = GetFloorClickPoint(e.GetPosition(openGLControl));
                displace = currentObject.Translate - floorPoint;
                if (isCollision == false)
                {
                    lastValidObjectPosition = currentObject.Translate - displace;
                }
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point3d direction = GetClickDirection(e.GetPosition(openGLControl));

            currentObject = currentProject.Scene.GetCollisionObject(currentProject.Scene.MainCamera.Translate, direction);
            if (currentObject != null)
            {
                Decimal lastPrice = currentObject.Price;
                EditObject wndEditObject = new EditObject(currentObject, configuration.Materials, currentProject.WallsHeight,
                    TotalPrice, currentProject.Budget, currentProject.MeasurementUnit);
                wndEditObject.ShowDialog();                
                currentObject = wndEditObject.GetCurrentObject();
                if (currentObject.Price != lastPrice)
                {
                    TotalPrice -= lastPrice;
                    TotalPrice += currentObject.Price;
                }
                return;
            }

            if (isZoomedIn == false)
            {
                //Point3d direction = GetClickDirection(e.GetPosition(openGLControl));
                float alpha = -(currentProject.Scene.MainCamera.Translate.Y) / direction.Y;
                Point3d destination = new Point3d(currentProject.Scene.MainCamera.Translate.X + direction.X * alpha, 0,
                    currentProject.Scene.MainCamera.Translate.Z + direction.Z * alpha);
                currentProject.Scene.MainCamera.Translate = destination;
                currentProject.Scene.MainCamera.Rotate = new Point3d(0, 180, 0);
                isZoomedIn = true;
            }
            else
            {
                currentProject.Scene.MainCamera.Translate = new Point3d(80, 540, 560);
                currentProject.Scene.MainCamera.Rotate = new Point3d(-40, 180, 0);
                isZoomedIn = false;
            }

        }
        private Point3d GetClickDirection(Point clickPoint)
        {
            double height = openGLControl.ActualHeight;
            double width = openGLControl.ActualWidth;
            float d = Convert.ToSingle(height * Math.Sqrt(3) / 2);
            Point3d position = new Point3d(Convert.ToSingle(clickPoint.X), Convert.ToSingle(clickPoint.Y), 0);
            Point3d v = new Point3d(Convert.ToSingle(clickPoint.X - width / 2), Convert.ToSingle(clickPoint.Y - height / 2), 0);
            Point3d dif = currentProject.Scene.MainCamera.Right * v.X + currentProject.Scene.MainCamera.Top * v.Y;
            Point3d direction = currentProject.Scene.MainCamera.Forward * d - dif;
            return direction;
        }

        private Point3d GetFloorClickPoint(Point clickPoint)
        {
            Point3d direction = GetClickDirection(clickPoint);
            float alpha = -(currentProject.Scene.MainCamera.Translate.Y) / direction.Y;
            Point3d destination = new Point3d(currentProject.Scene.MainCamera.Translate.X + direction.X * alpha, 0,
                currentProject.Scene.MainCamera.Translate.Z + direction.Z * alpha);


            return destination;
        }
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point3d destination = GetFloorClickPoint(e.GetPosition(openGLControl));
            if (sceneObject != null)
            {
                if (sceneObject.Translate.Y != 0)
                {
                    destination.Y = sceneObject.Translate.Y;
                }
                sceneObject.Translate = destination;
                sceneObject.Rotate = new Point3d(0, 180, 0);
                sceneObject.Scale = new Point3d(20, 20, 20);
                currentProject.Scene.AddHouseObject(sceneObject);
                TotalPrice += Math.Round(sceneObject.Price, 2);
                AdjustCollisionPositionOnPlace(sceneObject);
                //lblTotalPrice.Content = "Total price is: " + TotalPrice + " " + currency.ToString();
                sceneObject = null;
            }
        }

        private void AdjustCollisionPositionOnPlace(WorldObject sceneObject)
        {
            double step = Math.PI/120;
            float radius = 800;
            List<Point3d> directions = new List<Point3d>();
            for (double i = 0; i < Math.PI*2; i += step)
            {
                directions.Add(new Point3d((float)(radius * Math.Cos(i)), 0.0f, (float)(radius*Math.Sin(i))));
            }

            float td;
            float maxTd = float.MinValue;
            int maxIndex = -1;

            for (int i = 0; i < directions.Count; ++i)
            {
                isCollision = currentProject.Scene.CheckCurrentObjectCollisions(sceneObject, directions[i], out td);
                if (isCollision)
                {
                    if (td > maxTd)
                    {
                        maxTd = td;
                        maxIndex = i;
                    }
                }
            }

            if (isCollision)
            {
                sceneObject.Translate = sceneObject.Translate - directions[maxIndex] * (1 - maxTd);
            }
        }

        private void menutItemConfiguration_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemEditConfiguration_Click(object sender, RoutedEventArgs e)
        {
            SetupConfiguration setupConfiguration = new SetupConfiguration("Edit Configuration", configuration);
            setupConfiguration.ShowDialog();
            if (setupConfiguration.IsSavedConfiguration == true)
            {
                this.configuration = setupConfiguration.GetConfiguration();
                this.configuration.CurrentCurrency = CurrencyHelper.GetCurrentCurrency();
                SerializeConfiguration();
                addExtendedMenuItems("Design");
            }
        }

        private void menuItemResetConfiguration_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to reset the configuration?", "Reset Configuration", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                configuration.Reset();
                CurrencyHelper.SetCurrentCurrency(CurrencyHelper.GetDefaultCurrency());
                this.configuration.CurrentCurrency = CurrencyHelper.GetCurrentCurrency();
                SerializeConfiguration();
                addExtendedMenuItems("Design");
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentObject = null;
            isCollision = false;
        }

        Point oldMousePosition;
        Point3d rotateAroudPosition;
        Point3d lastValidObjectPosition;
        bool isCollision;

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentMousePosition = e.GetPosition(openGLControl);
            Point difference = new Point(currentMousePosition.X - oldMousePosition.X, currentMousePosition.Y - oldMousePosition.Y);
            float angle = Convert.ToSingle(Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y)) * 0.01f;
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (currentObject != null)
                {
                    rotateCount += 10;
                    if (rotateCount % 100 == 0)
                    {
                        currentObject.Rotate = new Point3d(currentObject.Rotate.X, currentObject.Rotate.Y + 90, currentObject.Rotate.Z);
                    }
                }
                else
                {
                    if (difference.X > 0)
                        angle = -angle;
                    currentProject.Scene.MainCamera.RotateAroundPoint(new Point3d(0, 0, 0), angle);
                }

            }
            else
            {
                if (currentObject != null)
                {
                    Point3d destination = GetFloorClickPoint(e.GetPosition(openGLControl));
                    lastValidObjectPosition = currentObject.Translate;
                    currentObject.Translate = destination + displace;

                    Point3d d = currentObject.Translate - lastValidObjectPosition;
                    float td;
                    isCollision = currentProject.Scene.CheckCurrentObjectCollisions(currentObject, d, out td);

                    if (isCollision)
                    {
                        //td *= 0.8f;
                        currentObject.Translate = lastValidObjectPosition + d * td;

                        isCollision = currentProject.Scene.CheckCurrentObjectCollisions(currentObject, d, out td);
                        if (isCollision)
                        {
                            currentObject.Translate = lastValidObjectPosition;
                        }
                    }
                }


                if (difference.X > 0)
                {
                    angle = -angle;
                }
            }


            oldMousePosition = currentMousePosition;
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            float zoom;
            Point3d center = GetFloorClickPoint(e.GetPosition(openGLControl));

            if (e.Delta > 0)
            {
                zoom = 0.8f;
            }
            else
            {
                zoom = 1.2f;
            }
            currentProject.Scene.MainCamera.Translate = (currentProject.Scene.MainCamera.Translate - center) * zoom + center;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            rotateAroudPosition = GetFloorClickPoint(e.GetPosition(openGLControl));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (currentObject == null)
                {
                    MessageBox.Show("Select an object to delete!");
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Do you really want to delete this object?", "Delete object", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        TotalPrice -= currentObject.Price;
                        currentProject.Scene.DeleteHouseObject(currentObject);
                        //lblTotalPrice.Content = "Total price is: " + totalPrice + " " + currency.ToString();
                        currentObject = null;

                    }
                    e.Handled = true;
                }

            }
        }



        private void menuItemScreenshot_Click(object sender, RoutedEventArgs e)
        {
            GenerateScreenshot();
        }

        private void GenerateScreenshot()
        {
            const string directory = "Screenshots";
            if (currentProject.Scene.IsEmpty() == false)
            {
                int width = (int)openGLControl.RenderSize.Width;
                int height = (int)openGLControl.RenderSize.Height;
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(openGLControl);
                PngBitmapEncoder pngImage = new PngBitmapEncoder();
                pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                MessageBoxResult result = MessageBox.Show("Do you want to take a screenshot of the current scene?", "Screenshot", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.DefaultExt = ".png";
                    saveFileDialog.Filter = "HouseDesign Scenes (.png)|*.png";
                    saveFileDialog.Title = "Screenshot";
                    saveFileDialog.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", directory));
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        using (Stream fileStream = File.Create(saveFileDialog.FileName))
                        {
                            pngImage.Save(fileStream);
                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("Screenshot not available! Create a new project or open an existing one!");
            }
        }

        private void menuItemUpView_Click(object sender, RoutedEventArgs e)
        {
            currentProject.Scene.MainCamera.Translate = new Point3d(0, 500, 0);
            currentProject.Scene.MainCamera.Rotate = new Point3d(-90, 180, 0);
        }

        private void menuItemReceipt_Click(object sender, RoutedEventArgs e)
        {
            Receipt receipt = new Receipt(currentProject.Scene.GetSortedHouseObjects(), configuration, currentProject);
            receipt.ShowDialog();
        }

        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            About_Box about = new About_Box();
            about.Show();
        }

        private void menuItemEditProperties_Click(object sender, RoutedEventArgs e)
        {
            EditProperties editProperties = new EditProperties(currentProject);
            editProperties.ShowDialog();
        }

        private void window_Closed(object sender, EventArgs e)
        {
            if (currentProject.Scene.IsEmpty() == false)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save the current project?", "Saving current project", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    if (currentProjectName != null)
                    {
                        SerializeProject(currentProjectName);
                    }
                    else
                    {
                        menuItemSaveProjectAs_Click(sender, new RoutedEventArgs());
                    }
                }
            }

        }





    }
}
