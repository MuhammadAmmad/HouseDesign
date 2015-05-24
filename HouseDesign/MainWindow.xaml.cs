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

namespace HouseDesign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorldObject sceneObject;
        private HouseDesign.Classes.Scene scene;
        private bool isZoomedIn = false;
        private String configurationFileName;
        private Configuration configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Importer importer = new Importer();
            scene = new HouseDesign.Classes.Scene();
            scene.MainCamera.Translate = new Point3d(0, 500, 0);
            scene.MainCamera.Rotate = new Point3d(-90, 0, 0);
            //scene.MainCamera.Translate = new Point3d(80, 540, 560);
            //scene.MainCamera.Rotate = new Point3d(-40, 180, 0);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            delta = 1000 / 40;
            timer.Interval = new TimeSpan(0, 0, 0, 0, delta);
            timer.Start();

            //openGLControl.AddHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler(), true);

            configurationFileName = "configuration.config";
            configuration = new Configuration();
            DeserializeConfiguration();
            
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
        void timer_Tick(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyUp(Key.Up))
            {
                scene.MainCamera.Translate += new Point3d(speed,0,0);
            }
            if (Keyboard.IsKeyUp(Key.Down))
            {
                scene.MainCamera.Translate += new Point3d(-speed, 0, 0);
            }
            if (Keyboard.IsKeyUp(Key.Left))
            {
                scene.MainCamera.Translate += new Point3d(0, 0, speed);
            }
            if (Keyboard.IsKeyUp(Key.Right))
            {
                scene.MainCamera.Translate += new Point3d(0, 0, -speed);

            }

            //if (Keyboard.IsKeyUp(Key.A))
            //{
            //    scene.MainCamera.Rotate += new Point3d(0, speed, 0);
            //}
            //if (Keyboard.IsKeyUp(Key.D))
            //{
            //    scene.MainCamera.Rotate += new Point3d(0, -speed, 0);

            //}


            //if (Keyboard.IsKeyUp(Key.Up))
            //{
            //    scene.MainCamera.Rotate += new Point3d(speed, 0, 0);
                
                
            //}
            //if (Keyboard.IsKeyUp(Key.Down))
            //{
            //    scene.MainCamera.Rotate += new Point3d(-speed, 0, 0);
                
            //}
            //if (Keyboard.IsKeyUp(Key.Left))
            //{
            //    scene.MainCamera.Rotate += new Point3d(0, speed, 0);
            //}
            //if (Keyboard.IsKeyUp(Key.Right))
            //{
            //    scene.MainCamera.Rotate += new Point3d(0, -speed, 0);

            //}

            //if (Keyboard.IsKeyUp(Key.LeftShift))
            //{
            //    scene.MainCamera.Translate += scene.MainCamera.Forward * -speed;
            //}
            //if (Keyboard.IsKeyUp(Key.RightShift))
            //{
            //    scene.MainCamera.Translate += scene.MainCamera.Forward * speed;
            //}

            //lblPosition.Content = scene.MainCamera.Translate.X + " " + scene.MainCamera.Translate.Y + " " + scene.MainCamera.Translate.Z + scene.MainCamera.Rotate.Y;
            lblPosition.Content = "R "+scene.MainCamera.Rotate.X + " " + scene.MainCamera.Rotate.Y + " " + scene.MainCamera.Rotate.Z+
                " T "+ scene.MainCamera.Translate.X+" "+scene.MainCamera.Translate.Y+" "+ scene.MainCamera.Translate.Z;
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

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
            OpenGL gl = openGLControl.OpenGL;

            //  Set the clear color.
           gl.ClearColor(0, 0, 0, 0);
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
            OpenGL gl = openGLControl.OpenGL;
           
            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 1000.0);

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
            float height = project.GetHeight();
            List<Wall> walls = project.currentHousePlan.GetWalls();

            for(int i=0;i<walls.Count;i++)
            {
                scene.AddWall(new WallObject(walls[i], height));
            }

            groupBoxCurrentProject.Visibility = Visibility.Visible;
        }

        private void menuItemOpenProject_Click(object sender, RoutedEventArgs e)
        {
            groupBoxCurrentProject.Visibility = Visibility.Visible;
        }

        private void menuItemSaveProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuItemResetHousePlan_Click(object sender, RoutedEventArgs e)
        {
            scene.ClearWalls();
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
                            scene.AddWall(new WallObject(walls[i], height));
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

        private void menuItemLoadHousePlan_Click(object sender, RoutedEventArgs e)
        {

        }
        private void menuItemResetObjects_Click(object sender, RoutedEventArgs e)
        {
            scene.ClearHouseObjects();
        }

        private void menuItemImportObject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemImportHousePlan_Click(object sender, RoutedEventArgs e)
        {

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
                            Category currentCategory = configuration.Categories[i];
                            addExtendedMenuItem(currentCategory);
                        }
                        break;
                    }
                    
            }
            
        }

        private void addExtendedMenuItem(Category category)
        {
            ExtendedMenuItem item = new ExtendedMenuItem(category.Path, category.Name);
            item.Tag = category;
            menuExtended.Items.Add(item);
            item.MouseLeftButtonDown += menuItemDesign_MouseLeftButtonDown;
        }

        void menuItemDesign_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Category currentCategory = ((ExtendedMenuItem)sender).Tag as Category;
            GenericCategory wndDesign = new GenericCategory(currentCategory, scene);
            wndDesign.ShowDialog();
            if(scene.IsEmpty()==false)
            {
                groupBoxCurrentProject.Visibility = Visibility.Visible;
                sceneObject = wndDesign.SelectedObject;
            }
        }

        private void openGLControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(isZoomedIn==false)
            {
                double height = openGLControl.ActualHeight;
                double width = openGLControl.ActualWidth;
                float d = Convert.ToSingle(height * Math.Sqrt(3) / 2);
                Point3d position = new Point3d(Convert.ToSingle(e.GetPosition(openGLControl).X), Convert.ToSingle(e.GetPosition(openGLControl).Y), 0);
                Point3d v = new Point3d(Convert.ToSingle(e.GetPosition(openGLControl).X - width / 2), Convert.ToSingle(e.GetPosition(openGLControl).Y - height / 2), 0);
                Point3d dif = scene.MainCamera.Right * v.X + scene.MainCamera.Top * v.Y;
                Point3d direction = scene.MainCamera.Forward * d - dif;
                float alpha = -(scene.MainCamera.Translate.Y) / direction.Y;
                Point3d destination = new Point3d(scene.MainCamera.Translate.X + direction.X, 0, scene.MainCamera.Translate.Z + direction.Z * alpha);
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

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            double height = openGLControl.ActualHeight;
            double width = openGLControl.ActualWidth;
            float d = Convert.ToSingle(height * Math.Sqrt(3) / 2);
            Point3d position = new Point3d(Convert.ToSingle(e.GetPosition(openGLControl).X), Convert.ToSingle(e.GetPosition(openGLControl).Y), 0);
            Point3d v = new Point3d(Convert.ToSingle(e.GetPosition(openGLControl).X - width / 2), Convert.ToSingle(e.GetPosition(openGLControl).Y - height / 2), 0);
            Point3d dif = scene.MainCamera.Right * v.X + scene.MainCamera.Top * v.Y;
            Point3d direction = scene.MainCamera.Forward * d - dif;
            float alpha = -(scene.MainCamera.Translate.Y) / direction.Y;
            Point3d destination = new Point3d(scene.MainCamera.Translate.X + direction.X, 0, scene.MainCamera.Translate.Z + direction.Z * alpha);
            if (sceneObject != null)
            {
                sceneObject.Translate = destination;
                sceneObject.Rotate = new Point3d(0, 180, 0);
                sceneObject.Scale = new Point3d(20, 20, 20);
                scene.AddHouseObject(sceneObject);
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
            }
        }

        private void menuItemResetConfiguration_Click(object sender, RoutedEventArgs e)
        {
            configuration.Reset();
            SerializeConfiguration();
        }

              
        
    }
}
