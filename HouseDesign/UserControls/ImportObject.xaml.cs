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
        public event EventHandler ImportMaterialStatusUpdated;

        private WorldObject currentObject;

        private FurnitureObject importedObject;
        public bool IsEdited { get; set; }

        public String CurrencyName
        {
            get { return (String)GetValue(CurrencyNameProperty); }
            set { SetValue(CurrencyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrencyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrencyNameProperty =
            DependencyProperty.Register("CurrencyName", typeof(String), typeof(ImportObject));

        private List<Category<Material>> materials;

        public bool ExistingImportedMaterials { get; set; }
        public String TotalPrice
        {
            get { return (String)GetValue(TotalPriceProperty); }
            set { SetValue(TotalPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalPriceProperty =
            DependencyProperty.Register("TotalPrice", typeof(String), typeof(ImportObject));

        

        
        public ImportObject(String title, FurnitureObject currentObject, List<Category<Material>> materials, bool isReadOnly, bool isEdited)
        {
            InitializeComponent();
            mainGroupBox.Header = title;
            this.IsEdited = isEdited;
            this.materials=materials;
            if(isReadOnly)
            {
                textBoxName.IsReadOnly = true;
                textBoxDescription.IsReadOnly = true;
                textBoxInitialPrice.IsReadOnly = true;
                btnCancel.IsEnabled = false;
                btnLoadObject.IsEnabled = false;
                btnOK.IsEnabled = false;
                groupBoxPreviewObject.Visibility = Visibility.Visible;
                stackPanelTotalPrice.Visibility = Visibility.Visible;
            }
            if(currentObject!=null)
            {
                this.importedObject = currentObject;
                InitializeCurrentObject();
                InitializeMaterials();
            }
            else
            {
                importedObject = new FurnitureObject();
            }

            CurrencyName = CurrencyHelper.GetCurrentCurrency().Name.ToString();
            ExistingImportedMaterials = false;
        }

        public void InitializeCurrentObject()
        {
            textBoxName.Text = importedObject.Name;
            textBoxDescription.Text = importedObject.Description;
            textBoxInitialPrice.Text = Math.Round(importedObject.InitialPrice, 2).ToString();
            //Decimal materialsPrice = GetMaterialsPrice();
            //textBlockTotalPrice.Text = (Math.Round(importedObject.InitialPrice, 2) + materialsPrice).ToString();
            Importer importer = new Importer();
            currentObject = importer.Import(importedObject.FullPath);
            //currentObject.InitializeTextures(openGLControl.OpenGL);
        }

        //private Decimal GetMaterialsPrice()
        //{
            
        //}

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
            InitializeMaterials();
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
                if(CheckObjectMaterials()==false)
                {
                    MessageBox.Show("Some object materials do not exist! Please customize or import!");
                    return;
                }
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
        private bool CheckObjectMaterials()
        {
            List<String> textures=currentObject.GetTextures();
            for(int i=0;i<textures.Count;i++)
            {
                if(GenericCategory.GetMaterialByImagePath(materials, textures[i])==null)
                {
                    return false;
                }
            }

            return true;
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

        public void InitializeMaterials()
        {
            listViewMaterials.Items.Clear();
            importedObject.Materials.Clear();
            listViewMaterials.Items.Add(new CustomizeHeader("NAME", "IMAGE", "PRICE/M²", "SURFACE", "TOTAL", "IMPORT"));

            List<String> textures=currentObject.GetTextures();
            for(int i=0;i<textures.Count;i++)
            {
                Material material = new Material();
                material = GenericCategory.GetMaterialByImagePath(materials, textures[i]);
                double surfaceNeeded = currentObject.getTotalAreaPerTexture(i);
                CustomizeMaterial customizeMaterial;
                if(material!=null)
                {
                    
                    customizeMaterial = new CustomizeMaterial(i, material, surfaceNeeded, false);
                    importedObject.Materials.Add(material);
                    
                }
                else
                {
                    customizeMaterial = new CustomizeMaterial(i, new Material("", textures[i], 0), surfaceNeeded, true);
                }

                customizeMaterial.MouseLeftButtonDown+=customizeMaterial_MouseLeftButtonDown;
                customizeMaterial.StatusUpdated+=customizeMaterial_StatusUpdated;
                listViewMaterials.Items.Add(customizeMaterial);
            }
        }

        void customizeMaterial_StatusUpdated(object sender, EventArgs e)
        {
            CustomizeMaterial customizeMaterial=sender as CustomizeMaterial;
            String imagePath=customizeMaterial.ImagePath;
            ImportMaterialAutomatically wndImportMaterialAutomatically = new ImportMaterialAutomatically(materials, imagePath);
            wndImportMaterialAutomatically.ShowDialog();
            materials = wndImportMaterialAutomatically.GetMaterials();
            ExistingImportedMaterials = true;
            InitializeMaterials();  
        }

        void customizeMaterial_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = (sender as CustomizeMaterial).Index;
            Material oldMaterial = (sender as CustomizeMaterial).GetCurrentMaterial();
            GenericMaterial genericMaterial = new GenericMaterial(materials, index);
            genericMaterial.StatusUpdated += genericMaterial_StatusUpdated;
            genericMaterial.ShowDialog();
            Material currentMaterial = genericMaterial.GetCurrentMaterial();
            int i = importedObject.Materials.IndexOf(oldMaterial);
            importedObject.Materials[i] = currentMaterial;
            InitializeMaterials();
        }

        void genericMaterial_StatusUpdated(object sender, EventArgs e)
        {
            GenericMaterial g = (sender as GenericMaterial);
            Material currentMaterial = g.GetCurrentMaterial();
            currentObject.SetTexture(g.Index, currentMaterial.FullPath, openGLControl.OpenGL);
        }

        public List<Category<Material>> GetMaterials()
        {
            return this.materials;
        }

    }
}
