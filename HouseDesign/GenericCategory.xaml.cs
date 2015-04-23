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
using System.Windows.Shapes;
using SharpGL.SceneGraph;
using SharpGL;
using HouseDesign.Classes;
using HouseDesign.UserControls;
using Microsoft.Win32;

namespace HouseDesign
{
    /// <summary>
    /// Interaction logic for GenericCategory.xaml
    /// </summary>
    public partial class GenericCategory : Window
    {
        private Category category{ get; set; }

        public WorldObject SelectedObject { get; set; }

        private HouseDesign.Classes.Scene scene;
        public GenericCategory(String categoryName, HouseDesign.Classes.Scene scene)
        {
            InitializeComponent();
            this.Title = categoryName;
            this.category = new Category(categoryName, "");
            TreeViewItem mainTreeViewItem = new TreeViewItem();
            mainTreeViewItem.IsExpanded = true;
            treeViewCategory.Items.Add(mainTreeViewItem);
            PopulateTreeView(category, mainTreeViewItem);
            this.scene = scene;
        }

        public void PopulateTreeView(Category mainCategory, TreeViewItem currentItem)
        {
            foreach(String obj in mainCategory.Files)
            {
                String[] tokens = obj.Split('.');
                String objectName = tokens[0].Split('\\').Last();
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(@"D:\Licenta\HouseDesign\HouseDesign\Assets\pillowTexture.jpg", objectName, obj);
                extendedItem.Tag = "object";
                TreeViewItem item = new TreeViewItem();
                item.Header = extendedItem;
                currentItem.Items.Add(item);
            }
            foreach(Category c in mainCategory.SubCategories)
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(@"D:\Licenta\HouseDesign\HouseDesign\Assets\burgundt.jpg", c.Name, c.Path);
                extendedItem.Tag = "category";
                TreeViewItem item = new TreeViewItem();
                item.Header = extendedItem;
                currentItem.Items.Add(item);
                PopulateTreeView(c, item);
                
            }

        }

        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


            //  Load the identity matrix.
            gl.LoadIdentity();



            //  Rotate around the Y axis.
            gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

            if(SelectedObject!=null)
            {
                SelectedObject.Draw(gl);
            }
           

            //  Nudge the rotation.
            rotation += 3.0f;
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
            gl.ClearColor(1, 1, 1, 0);
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
            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(-5, 5, -5, 0, 0, 0, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        /// The current rotation.
        /// </summary>
        private float rotation = 0.0f;


        private void btnImportObject_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Import object";
            fdlg.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\Exports";
            fdlg.Filter = "FBX files (*.fbx;)|*.fbx;";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == true)
            {
                //To be implemented

            }
        }

        private void btnAddToScene_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCustomizeObject_Click(object sender, RoutedEventArgs e)
        {
            groupBoxCustomizeCurrentObject.Visibility = Visibility.Visible;
            InitializeTextures();           
        }

        public void InitializeTextures()
        {
            listViewTextures.Items.Clear();
            int rank = 1;
            foreach (String texture in SelectedObject.GetTextures())
            {
                ChooseTexture chooseTexture = new ChooseTexture(rank, texture);
                chooseTexture.MouseLeftButtonDown += chooseTexture_MouseLeftButtonDown;
                listViewTextures.Items.Add(chooseTexture);
                rank++;
            }
        }

        void chooseTexture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Choose texture";
            fdlg.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\Assets";
            fdlg.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == true)
            {
                string fullPath =System.IO.Path.GetFullPath(fdlg.FileName);
                ChooseTexture currentTexture = sender as ChooseTexture;
                SelectedObject.SetTexture(currentTexture.Index-1, fullPath);
                InitializeTextures();
                
            }
        }

        private void treeViewCategory_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedTreeViewItem=treeViewCategory.SelectedItem as TreeViewItem;
            if (treeViewCategory.Items.Count > 0 &&  selectedTreeViewItem!= null)
            {
                if((selectedTreeViewItem.Header as ExtendedTreeViewItem).Tag=="object")
                {
                    String path = (selectedTreeViewItem.Header as ExtendedTreeViewItem).FullPath;
                    Importer importer = new Importer();
                    SelectedObject=importer.Import(path);
                    groupBoxObj.Visibility = Visibility.Visible;
                    InitializeTextures();
                }
            }


        }
    }
}
