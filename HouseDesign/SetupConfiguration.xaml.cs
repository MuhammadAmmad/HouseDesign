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
    /// Interaction logic for SetupConfiguration.xaml
    /// </summary>
    public partial class SetupConfiguration : Window
    {
        private Configuration configuration;
        public SetupConfiguration(String title, Configuration conf)
        {
            InitializeComponent();
            this.Title = title;
            this.configuration = conf;
            InitializeExtendedMenuItems();

        }

        public void InitializeExtendedMenuItems()
        {
            ExtendedMenuItem extendedMenuItemNew = new ExtendedMenuItem("D:\\Licenta\\HouseDesign\\HouseDesign\\Images\\new.png", "New");
            menuShortcuts.Items.Add(extendedMenuItemNew);
            extendedMenuItemNew.MouseLeftButtonDown += extendedMenuItemNew_MouseLeftButtonDown;
            ExtendedMenuItem extendedMenuItemEdit = new ExtendedMenuItem("D:\\Licenta\\HouseDesign\\HouseDesign\\Images\\edit.png", "Edit");
            menuShortcuts.Items.Add(extendedMenuItemEdit);
            extendedMenuItemEdit.MouseLeftButtonDown += extendedMenuItemEdit_MouseLeftButtonDown;
            ExtendedMenuItem extendedMenuItemCut = new ExtendedMenuItem("D:\\Licenta\\HouseDesign\\HouseDesign\\Images\\cut.png", "Cut");
            menuShortcuts.Items.Add(extendedMenuItemCut);
            extendedMenuItemCut.MouseLeftButtonDown += extendedMenuItemCut_MouseLeftButtonDown;
            ExtendedMenuItem extendedMenuItemCopy = new ExtendedMenuItem("D:\\Licenta\\HouseDesign\\HouseDesign\\Images\\copy.png", "Copy");
            menuShortcuts.Items.Add(extendedMenuItemCopy);
            extendedMenuItemCopy.MouseLeftButtonDown += extendedMenuItemCopy_MouseLeftButtonDown;
            ExtendedMenuItem extendedMenuItemPaste = new ExtendedMenuItem("D:\\Licenta\\HouseDesign\\HouseDesign\\Images\\paste.png", "Paste");
            menuShortcuts.Items.Add(extendedMenuItemPaste);
            extendedMenuItemPaste.MouseLeftButtonDown += extendedMenuItemPaste_MouseLeftButtonDown;
            ExtendedMenuItem extendedMenuItemImport = new ExtendedMenuItem("D:\\Licenta\\HouseDesign\\HouseDesign\\Images\\import.png", "Import");
            menuShortcuts.Items.Add(extendedMenuItemImport);
            extendedMenuItemImport.MouseLeftButtonDown += extendedMenuItemImport_MouseLeftButtonDown;
            ExtendedMenuItem extendedMenuItemDelete = new ExtendedMenuItem("D:\\Licenta\\HouseDesign\\HouseDesign\\Images\\deleted.png", "Delete");
            menuShortcuts.Items.Add(extendedMenuItemDelete);
            extendedMenuItemDelete.MouseLeftButtonDown += extendedMenuItemDelete_MouseLeftButtonDown;
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

            //if (SelectedObject != null)
            //{
            //    SelectedObject.Draw(gl);
            //}


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
        private void extendedMenuItemNew_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void extendedMenuItemEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void extendedMenuItemCut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void extendedMenuItemCopy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void extendedMenuItemPaste_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void extendedMenuItemImport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabItem currentTabItem = (TabItem)mainTabControl.SelectedItem;
            if(currentTabItem.Header.ToString()=="Categories")
            {
                groupBoxPreviewObject.Visibility = Visibility.Visible;
            }
            else
            {
                groupBoxPreviewMaterial.Visibility = Visibility.Visible;
            }
        }

        private void extendedMenuItemDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void treeViewCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void btnAddObject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddMaterial_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
