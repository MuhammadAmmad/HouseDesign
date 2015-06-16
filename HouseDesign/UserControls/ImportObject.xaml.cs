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

        private List<WorldObjectMaterial> currentObjectMaterials;

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

        private Decimal materialsPrice;
        private double currentTradeAllowance;
        
        public ImportObject(String title, FurnitureObject importedObject, List<Category<Material>> materials, bool isReadOnly, bool isEdited,
            double currentTradeAllowance)
        {
            InitializeComponent();
            mainGroupBox.Header = title;
            this.IsEdited = isEdited;
            this.materials=materials;
            this.currentObjectMaterials = new List<WorldObjectMaterial>();
            this.currentTradeAllowance = currentTradeAllowance;
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
            if(importedObject!=null)
            {
                this.importedObject = importedObject;
                InitializeCurrentObject();
                InitializeMaterials();
                stackPanelTotalPrice.Visibility = Visibility.Visible;
            }
            else
            {
                this.importedObject = new FurnitureObject();
            }

            CurrencyName = CurrencyHelper.GetCurrentCurrency().Name.ToString();
            ExistingImportedMaterials = false;
        }

        public void InitializeCurrentObject()
        {
            textBoxName.Text = importedObject.Name;
            textBoxDescription.Text = importedObject.Description;
            textBoxInitialPrice.Text = Math.Round(importedObject.InitialPrice, 2).ToString();
            currentObject = importedObject.GetInnerObject();
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
                importedObject.InitializeInnerObject();
                currentObject = importedObject.GetInnerObject();
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
                importedObject.SetInnerObjectMaterials(currentObjectMaterials);
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
            listViewMaterials.Items.Clear();
        }

        public FurnitureObject GetImportedObject()
        {
            return importedObject;
        }

        public void InitializeMaterials()
        {
            listViewMaterials.Items.Clear();
            currentObjectMaterials.Clear();
            listViewMaterials.Items.Add(new CustomizeHeader("NAME", "IMAGE", "PRICE/M²", "SURFACE", "TOTAL", "IMPORT"));

            if(currentObject.GetMaterials().Count>0)
            {
                currentObjectMaterials.AddRange(currentObject.GetMaterials());
                for(int i=0;i<currentObjectMaterials.Count;i++)
                {
                    Material material = currentObjectMaterials[i].Material;
                    double surfaceNeeded = currentObject.GetTotalAreaPerTexture(i);
                    materialsPrice += Convert.ToDecimal(surfaceNeeded) * material.Price;
                    CustomizeMaterial customizeMaterial = new CustomizeMaterial(i, material, surfaceNeeded, false, true);

                    customizeMaterial.MouseLeftButtonDown += customizeMaterial_MouseLeftButtonDown;
                    customizeMaterial.StatusUpdated += customizeMaterial_StatusUpdated;
                    listViewMaterials.Items.Add(customizeMaterial);
                }
            }
            else
            {
                List<String> textures = currentObject.GetTextures();
                materialsPrice = 0;
                for (int i = 0; i < textures.Count; i++)
                {
                    Material material = new Material();
                    material = GenericCategory.GetMaterialByImagePath(materials, textures[i]);
                    double surfaceNeeded = currentObject.GetTotalAreaPerTexture(i);

                    CustomizeMaterial customizeMaterial;
                    if (material != null)
                    {
                        materialsPrice += Convert.ToDecimal(surfaceNeeded) * material.Price;
                        customizeMaterial = new CustomizeMaterial(i, material, surfaceNeeded, false, true);
                        currentObjectMaterials.Add(new WorldObjectMaterial(material, surfaceNeeded));

                    }
                    else
                    {
                        customizeMaterial = new CustomizeMaterial(i, new Material("", textures[i], 0), surfaceNeeded, true, true);
                    }

                    customizeMaterial.MouseLeftButtonDown += customizeMaterial_MouseLeftButtonDown;
                    customizeMaterial.StatusUpdated += customizeMaterial_StatusUpdated;
                    listViewMaterials.Items.Add(customizeMaterial);
                }
            }

            

            Decimal totalPrice = materialsPrice + importedObject.InitialPrice + 
                Convert.ToDecimal(currentTradeAllowance) * (materialsPrice + importedObject.InitialPrice)/100;
            textBlockTotalPrice.Text = String.Format("{0:0.000}", totalPrice);
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
            if(GenericCategory.GetMaterialByImagePath(materials, currentMaterial.FullPath)!=null)
            {
                int i = GetIndexOfMaterial(oldMaterial);
                if(i==-1)
                {
                    i=currentObjectMaterials.Count;
                    currentObjectMaterials.Add(new WorldObjectMaterial(currentMaterial, currentObject.GetTotalAreaPerTextureByPath(currentMaterial.FullPath)));
                }
                else
                {
                    currentObjectMaterials[i] = new WorldObjectMaterial(currentMaterial, currentObjectMaterials[i].SurfaceNeeded);
                }
                
            }
            importedObject.Materials.Add(currentMaterial);
            InitializeMaterials();
        }

        private int GetIndexOfMaterial(Material material)
        {
            int index=-1;
            for (int i = 0; i < currentObjectMaterials.Count;i++ )
            {
                if(currentObjectMaterials[i].Material==material)
                {
                    index = i;
                    return index;
                }
            }

                return index;
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
