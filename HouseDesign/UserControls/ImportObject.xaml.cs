using HouseDesign.Classes;
using Microsoft.Win32;
using SharpGL;
using SharpGL.SceneGraph;
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
    /// Interaction logic for ImportObject.xaml
    /// </summary>
    public partial class ImportObject : UserControl
    {
        public event EventHandler StatusUpdated;

        private WorldObject currentObject;

        private FurnitureObject importedObject;
        public bool IsEdited { get; set; }
        public ImportObject(String title, FurnitureObject currentObject, bool isReadOnly, bool isEdited)
        {
            InitializeComponent();
            mainGroupBox.Header = title;
            this.IsEdited = isEdited;
            if(isReadOnly)
            {
                textBoxName.IsReadOnly = true;
                textBoxDescription.IsReadOnly = true;
                textBoxInitialPrice.IsReadOnly = true;
                btnCancel.IsEnabled = false;
                btnLoadObject.IsEnabled = false;
                btnOK.IsEnabled = false;
                groupBoxPreviewObject.Visibility = Visibility.Visible;
            }
            if(currentObject!=null)
            {
                this.importedObject = currentObject;
                InitializeCurrentObject();
            }
            else
            {
                importedObject = new FurnitureObject();
            }            
        }

        public void InitializeCurrentObject()
        {
            textBoxName.Text = importedObject.Name;
            textBoxDescription.Text = importedObject.Description;
            textBoxInitialPrice.Text = importedObject.InitialPrice.ToString();
            Importer importer = new Importer();
            currentObject = importer.Import(importedObject.FullPath);
            //currentObject.InitializeTextures(openGLControl.OpenGL);
        }

        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL object.
            //OpenGL gl = openGLControl.OpenGL;
            OpenGL gl = args.OpenGL as OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


            //  Load the identity matrix.
            gl.LoadIdentity();



            //  Rotate around the Y axis.
            gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

            if (currentObject != null)
            {
                currentObject.Draw(gl);
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
            //OpenGL gl = openGLControl.OpenGL;
            OpenGL gl = args.OpenGL as OpenGL;

            //  Set the clear color.
            gl.ClearColor(1, 1, 1, 0);
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
            gl.Perspective(60.0f, (double)openGLControl.ActualWidth / (double)openGLControl.ActualHeight, 0.01, 100.0);

            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(-5, 5, -5, 0, 0, 0, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        /// The current rotation.
        /// </summary>
        private float rotation = 0.0f;

        private void btnLoadObject_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Import object";
            fdlg.InitialDirectory = @"D:\Licenta\HouseDesign\HouseDesign\Exports";
            fdlg.Filter = "FBX files (*.fbx;)|*.fbx;";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == true)
            {
                importedObject.FullPath = fdlg.FileName;
                Importer importer = new Importer();
                currentObject = importer.Import(importedObject.FullPath);
               // currentObject.InitializeTextures(openGLControl.OpenGL);
                groupBoxPreviewObject.Visibility = Visibility.Visible;

            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(textBoxName.Text.Length==0 || textBoxInitialPrice.Text.Length==0)
            {
                MessageBox.Show("Complete mandatory fields!");
                return;
            }
            else
            {
                importedObject.Name = textBoxName.Text;
                importedObject.Description = textBoxDescription.Text;
                importedObject.InitialPrice = Convert.ToDecimal(textBoxInitialPrice.Text);
                ClearAllFields();
                if (this.StatusUpdated != null)
                {
                    this.StatusUpdated(this, new EventArgs());
                }  
                
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
        }

        private void ClearAllFields()
        {
            groupBoxPreviewObject.Visibility = Visibility.Collapsed;
            textBoxDescription.Clear();
            textBoxInitialPrice.Clear();
            textBoxName.Clear();
        }

        public FurnitureObject GetImportedObject()
        {
            return importedObject;
        }

    }
}
