﻿using System;
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

namespace HouseDesign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //private WorldObject testObject;

        private WorldObject sceneObject;
        private HouseDesign.Classes.Scene scene;
        private bool isZoomedIn = false;
        private String configurationFileName;
        private Configuration configuration;
        private WorldObject currentObject;
        private int rotateCount;
        //private Decimal totalPrice;
        private float sceneHeight;
        public event PropertyChangedEventHandler PropertyChanged;
        private Currency currency;
        //public Decimal TotalPrice
        //{
        //    get
        //    {
        //        return totalPrice;
        //    }
        //    set
        //    {
        //        totalPrice = value;
        //        OnPropertyChanged("TotalPrice");
        //    }
        //}



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
            //Collision collision = new Collision(new Point3d(2, 0, 3), new Point3d(3, -1, 2), new Point3d(2, 0, 0), new Point3d(0, 2, 0), new Point3d(0, 0, 2));
            //float rez=collision.GetMaxTD(new Point3d(2, 0, 2));
            //MessageBox.Show(rez + "********");


            Importer importer = new Importer();
            scene = new HouseDesign.Classes.Scene();
            scene.MainCamera.Translate = new Point3d(0, 500, 0);
            scene.MainCamera.Rotate = new Point3d(-90, 180, 0);
            //scene.MainCamera.Translate = new Point3d(80, 540, 560);
            //scene.MainCamera.Rotate = new Point3d(-40, 180, 0);


            //testObject = importer.Import(@"D:\Licenta\HouseDesign\HouseDesign\Exports\bed.fbx");
            //scene.AddHouseObject(testObject);

            //testObject.Scale = new Point3d(20, 20, 20);
            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            delta = 1000 / 40;
            timer.Interval = new TimeSpan(0, 0, 0, 0, delta);
            timer.Start();

            //openGLControl.AddHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler(), true);

            configurationFileName = "configuration.config";
            configuration = new Configuration();
            if(IsEmptyConfiguration()==0)
            {
                DeserializeConfiguration();
            }

            rotateCount = 0;
            TotalPrice = 0;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            isCollision = false;
            currency = Currency.Lei;
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

        private void SerializeScene(String sceneFileName)
        {
            using (Stream fileStream = new FileStream(sceneFileName, FileMode.Create,
                           FileAccess.Write, FileShare.None))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, scene);
            }
        }

        private void DeserializeScene(String sceneFileName)
        {
            using (Stream fileStream = new FileStream(sceneFileName, FileMode.Open,
                           FileAccess.Read, FileShare.Read))
            {
                IFormatter formatter = new BinaryFormatter();
                scene = (HouseDesign.Classes.Scene)formatter.Deserialize(fileStream);
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyUp(Key.Up))
            {
                scene.MainCamera.Translate += new Point3d(0, 0, speed);
            }
            if (Keyboard.IsKeyUp(Key.Down))
            {
                scene.MainCamera.Translate += new Point3d(0, 0, -speed);

                if (Keyboard.IsKeyUp(Key.Left))
                {
                    scene.MainCamera.Translate += new Point3d(-speed, 0, 0);
                }
                if (Keyboard.IsKeyUp(Key.Right))
                {
                    scene.MainCamera.Translate += new Point3d(speed, 0, 0);
                    
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

            scene.Render(gl);
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

        private void menuItemFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemView_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemScene_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemDesign_Click(object sender, RoutedEventArgs e)
        {
            addExtendedMenuItems("Design");
            groupBoxExtendedMenu.Visibility = Visibility.Visible;

        }

        private void menuItemNewProject_Click(object sender, RoutedEventArgs e)
        {            
            NewProject project = new NewProject("New Project");
            project.ShowDialog();
            sceneHeight = project.GetHeight();
            List<Wall> walls = project.currentHousePlan.GetWalls();

            for(int i=0;i<walls.Count;i++)
            {
                WallObject wall = new WallObject(walls[i], sceneHeight);
               // wall.InitializeTextures(openGLControl.OpenGL);
                scene.AddWall(wall);
            }

            groupBoxCurrentProject.Visibility = Visibility.Visible;
        }

        private void menuItemOpenProject_Click(object sender, RoutedEventArgs e)
        {
            groupBoxCurrentProject.Visibility = Visibility.Visible;
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Open project";
            fdlg.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\SavedProjects";
            fdlg.Filter = "HouseDesign Projects (.hds)|*.hds";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == true)
            {
                DeserializeScene(fdlg.FileName);
            }
        }

        private void menuItemSaveProject_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveProject = new SaveFileDialog();            
            saveProject.DefaultExt = ".hds";
            saveProject.Filter = "HouseDesign Projects (.hds)|*.hds";
            saveProject.Title = "Save Project";
            saveProject.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\SavedProjects";
            if (saveProject.ShowDialog() == true)
            {
                String[] tokens = saveProject.FileName.Split('\\');
                String auxName = tokens[tokens.Count() - 1];
                SerializeScene(auxName);
            }
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuItemResetHousePlan_Click(object sender, RoutedEventArgs e)
        {
            scene.ClearWalls();
            scene.ClearHouseObjects();
            //menuItemLoadHousePlan.Visibility = Visibility.Visible;
            MessageBoxResult result=MessageBox.Show("Would you like to load a new house plan?", "Reset house plan", MessageBoxButton.YesNoCancel);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    {
                        NewProject project = new NewProject("Load house plan");
                        project.Show();
                        float height = project.GetHeight();
                        List<Wall> walls = project.currentHousePlan.GetWalls();

                        for (int i = 0; i < walls.Count; i++)
                        {
                            WallObject wall = new WallObject(walls[i], height);
                            //wall.InitializeTextures(openGLControl.OpenGL);
                            scene.AddWall(wall);
                        }

                        break;
                    }
                case MessageBoxResult.No:
                    {
                        break;
                    }
                case MessageBoxResult.Cancel:
                    {
                        break;
                    }
            }
        }
        private void menuItemResetObjects_Click(object sender, RoutedEventArgs e)
        {
            scene.ClearHouseObjects();
        }

        private void addExtendedMenuItems(String menuType)
        {
            switch(menuType)
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

                        for (int i = 0; i < configuration.Categories.Count;i++ )
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
            GenericCategory wndDesign = new GenericCategory(currentCategory, scene, configuration.Materials, sceneHeight);
            wndDesign.ShowDialog();
            if(scene.IsEmpty()==false)
            {
                groupBoxCurrentProject.Visibility = Visibility.Visible;
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
            Point3d direction=GetClickDirection(e.GetPosition(openGLControl));
            
            currentObject= scene.GetCollisionObject(scene.MainCamera.Translate, direction);
            if (currentObject != null)
            {
                Point3d floorPoint = GetFloorClickPoint(e.GetPosition(openGLControl));
                displace = currentObject.Translate - floorPoint;
                if(isCollision==false)
                {
                    lastValidObjectPosition = currentObject.Translate-displace;
                }                
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(isZoomedIn==false)
            {
                Point3d direction = GetClickDirection(e.GetPosition(openGLControl));
                float alpha = -(scene.MainCamera.Translate.Y) / direction.Y;
                Point3d destination = new Point3d(scene.MainCamera.Translate.X + direction.X*alpha, 0, scene.MainCamera.Translate.Z + direction.Z * alpha);
                scene.MainCamera.Translate = destination;
                scene.MainCamera.Rotate = new Point3d(0, 180, 0);
                isZoomedIn = true;
            }
            else
            {
                scene.MainCamera.Translate = new Point3d(80, 540, 560);
                scene.MainCamera.Rotate = new Point3d(-40, 180, 0);
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
            Point3d dif = scene.MainCamera.Right * v.X + scene.MainCamera.Top * v.Y;
            Point3d direction = scene.MainCamera.Forward * d -dif;
            return direction;
        }

        private Point3d GetFloorClickPoint(Point clickPoint)
        {
            Point3d direction = GetClickDirection(clickPoint);
            float alpha = -(scene.MainCamera.Translate.Y) / direction.Y;
            Point3d destination = new Point3d(scene.MainCamera.Translate.X + direction.X * alpha, 0, scene.MainCamera.Translate.Z + direction.Z * alpha);


            return destination;
        }
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point3d destination = GetFloorClickPoint(e.GetPosition(openGLControl));
            if (sceneObject != null)
            {
                if(sceneObject.Translate.Y!=0)
                {
                    destination.Y = sceneObject.Translate.Y;
                }
                sceneObject.Translate = destination;
                sceneObject.Rotate = new Point3d(0, 180, 0);
                sceneObject.Scale = new Point3d(20, 20, 20);
                scene.AddHouseObject(sceneObject);
                TotalPrice += sceneObject.Price;
                //lblTotalPrice.Content = "Total price is: " + TotalPrice + " " + currency.ToString();
                sceneObject = null;
            }
        }

        private void menutItemConfiguration_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemEditConfiguration_Click(object sender, RoutedEventArgs e)
        {
            SetupConfiguration setupConfiguration = new SetupConfiguration("Edit Configuration", configuration);
            setupConfiguration.ShowDialog();
            if(setupConfiguration.IsSavedConfiguration==true)
            {
                this.configuration = setupConfiguration.GetConfiguration();
                SerializeConfiguration();
                addExtendedMenuItems("Design");
            }
        }

        private void menuItemResetConfiguration_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to reset the configuration?", "Reset Configuration", MessageBoxButton.YesNo);
            if(result==MessageBoxResult.Yes)
            {
                configuration.Reset();
                SerializeConfiguration();
                addExtendedMenuItems("Design");
            }            
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentObject=null;
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
                    scene.MainCamera.RotateAroundPoint(new Point3d(0, 0, 0), angle);
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
                    isCollision=scene.CheckCurrentObjectCollisions(currentObject, d, out td);
                    
                    if(isCollision)
                    {
                        //td *= 0.9f;
                        //currentObject.Translate = lastValidObjectPosition + d * td;
                        currentObject.Translate = lastValidObjectPosition;
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
            float zoom ;
            Point3d center = GetFloorClickPoint(e.GetPosition(openGLControl));

            if (e.Delta > 0)
            {
                zoom = 0.8f;
            }
            else
            {
                zoom = 1.2f;
            }
            scene.MainCamera.Translate = (scene.MainCamera.Translate-center) * zoom+center;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            rotateAroudPosition = GetFloorClickPoint(e.GetPosition(openGLControl));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if(currentObject==null)
                {
                    MessageBox.Show("Select an object to delete!");
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Do you really want to delete this object?", "Delete object", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        TotalPrice -= currentObject.Price;
                        scene.DeleteHouseObject(currentObject);
                        //lblTotalPrice.Content = "Total price is: " + totalPrice + " " + currency.ToString();
                        currentObject = null;

                    }
                    e.Handled = true;
                }
                
            }
        }

        public enum Currency
        {
            Lei,
            Euro,
            Dollar,
            BritishPound
        };

        private void menuItemScreenshot_Click(object sender, RoutedEventArgs e)
        {
            int width = (int)openGLControl.RenderSize.Width;
            int height = (int)openGLControl.RenderSize.Height;
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(openGLControl);
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            MessageBoxResult result = MessageBox.Show("Do you want to take a screenshot of the current scene?", "Screenshot", MessageBoxButton.YesNo);
            if(result==MessageBoxResult.Yes)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".png";
                saveFileDialog.Filter = "HouseDesign Scenes (.png)|*.png";
                saveFileDialog.Title = "Screenshot";
                saveFileDialog.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\Screenshots";
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (Stream fileStream = File.Create(saveFileDialog.FileName))
                    {
                        pngImage.Save(fileStream);
                    }
                }

            }
            
        }

        private void menuItemUpView_Click(object sender, RoutedEventArgs e)
        {
            scene.MainCamera.Translate = new Point3d(0, 500, 0);
            scene.MainCamera.Rotate = new Point3d(-90, 180, 0);
        }

        
    }
}
