using HouseDesign.Classes;
using HouseDesign.UserControls;
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
using System.Windows.Shapes;

namespace HouseDesign
{
    /// <summary>
    /// Interaction logic for EditObject.xaml
    /// </summary>
    public partial class EditObject : Window
    {
        private WorldObject currentObject;
        private WorldObject oldObject;
        private List<WorldObjectMaterial> selectedObjectMaterials;
        private List<Category<Material>> materials;
        private float sceneHeight;
        public float ChosenHeight { get; set; }
        private Decimal currentTradeAllowance;
        private Decimal actualPrice;
        private Decimal projectBudget;
        private Project.UnitOfMeasurement measurementUnit;
        private float realHeightScaleFactor;
        private float scaleFactor;
        private HouseDesign.Classes.Scene scene;
        public EditObject(WorldObject currentObject, List<Category<Material>> materials, float sceneHeight,
            Decimal actualPrice, Decimal projectBudget, Project.UnitOfMeasurement measurementUnit, HouseDesign.Classes.Scene scene)
        {
            InitializeComponent();
            //is.currentObject = new WorldObject();
            this.currentObject = currentObject.Clone();
            this.scene = scene;

            this.currentObject.Translate = new Point3d(0, 0, 0);
            this.currentObject.Scale = new Point3d(1, 1, 1);
            this.oldObject = currentObject;
            selectedObjectMaterials = new List<WorldObjectMaterial>();
            this.materials = materials;
            ChosenHeight = 0;
            this.sceneHeight = sceneHeight * 0.0025f;
            this.actualPrice = actualPrice;
            this.projectBudget = projectBudget;
            this.measurementUnit = measurementUnit;
            textBlockMeasurementUnit.Text = measurementUnit.ToString();

            currentTradeAllowance = GetTradeAllowance(currentObject);

            selectedObjectMaterials.AddRange(currentObject.GetMaterials());
            InitializeScaleFactors();
            if (currentObject != null)            {
                groupBoxPrices.Visibility = Visibility.Visible;
                InitializeMaterials();
                InitializePrices();
                InitializeDimensions();
            }            
            
            if(currentObject.IsSuspendable)
            {
                checkBoxIsSuspendable_Checked(this, new RoutedEventArgs());
                checkBoxIsSuspendable.IsChecked = true;
                textBoxChosenHeight.Text=(currentObject.Translate.Y/(realHeightScaleFactor*50)).ToString();
            }

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void InitializeScaleFactors()
        {
            realHeightScaleFactor = 1;
            if (measurementUnit == Project.UnitOfMeasurement.cm)
            {
                scaleFactor = 0.022f;
                realHeightScaleFactor = 0.01f;
            }
            else
            {
                if (measurementUnit == Project.UnitOfMeasurement.m)
                {
                    scaleFactor = 2.2f;
                    realHeightScaleFactor = 1;
                }
                else
                {
                    scaleFactor = 0.0022f;
                    realHeightScaleFactor = 0.001f;
                }
            }

        }
        private static Decimal GetTradeAllowance(WorldObject currentObject)
        {
            return (currentObject.Price / (currentObject.MaterialsPrice + currentObject.InitialPrice) - 1)*100;
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

        private void btnCustomizeObject_Click(object sender, RoutedEventArgs e)
        {
            groupBoxCustomizeCurrentObject.Visibility = Visibility.Visible;
            InitializeMaterials(); 
        }
        public void InitializeMaterials()
        {
            listViewMaterials.Items.Clear();
            listViewMaterials.Items.Add(new CustomizeHeader("NAME", "IMAGE", "PRICE/M²", "SURFACE", "TOTAL", ""));
            for (int i = 0; i < selectedObjectMaterials.Count; i++)
            {
                CustomizeMaterial customizeMaterial = new CustomizeMaterial(i, selectedObjectMaterials[i].Material, selectedObjectMaterials[i].SurfaceNeeded, false, false);
                customizeMaterial.MouseLeftButtonDown += customizeMaterial_MouseLeftButtonDown;
                listViewMaterials.Items.Add(customizeMaterial);
            }
        }

        void customizeMaterial_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = (sender as CustomizeMaterial).Index;
            Material oldMaterial = (sender as CustomizeMaterial).GetCurrentMaterial();
            GenericMaterial genericMaterial = new GenericMaterial(materials, index);
            genericMaterial.StatusUpdated += genericMaterial_StatusUpdated;
            genericMaterial.ShowDialog();
            Material currentMaterial = genericMaterial.GetCurrentMaterial();
            int i = GetIndexOfMaterial(oldMaterial);
            selectedObjectMaterials[i] = new WorldObjectMaterial(currentMaterial, selectedObjectMaterials[i].SurfaceNeeded);
            InitializeMaterials();
            InitializePrices();
        }
        private int GetIndexOfMaterial(Material material)
        {
            int index = -1;
            for (int i = 0; i < selectedObjectMaterials.Count; i++)
            {
                if (selectedObjectMaterials[i].Material == material)
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
            //POSSIBLE BUG
            currentObject.SetTexture(g.Index, currentMaterial.FullPath, openGLControl.OpenGL);
            //InitializeMaterials();
            //InitializePrices();
        }

        public void InitializePrices()
        {
            Currency projectCurrency = CurrencyHelper.GetProjectCurrency();
            Decimal actualInitialPrice = CurrencyHelper.FromCurrencyToCurrency(CurrencyHelper.GetCurrentCurrency(), currentObject.InitialPrice, projectCurrency);
            textBlockInitialPrice.Text = string.Format("{0:0.000}", actualInitialPrice);
            Decimal materialsPrice = GetMaterialsPrice();
            Decimal actualMaterialsPrice = CurrencyHelper.FromCurrencyToCurrency(CurrencyHelper.GetCurrentCurrency(), materialsPrice, projectCurrency);
            textBlockMaterialsPrice.Text = string.Format("{0:0.000}", actualMaterialsPrice);

            textBlockTotalPrice.Text = string.Format("{0:0.000}", (actualMaterialsPrice + actualInitialPrice +
                currentTradeAllowance / 100 * (actualMaterialsPrice + actualInitialPrice)));
        }

        public void InitializeDimensions()
        {
            textBlockDimensionHeight.Text = measurementUnit.ToString();
            textBlockDimensionWidth.Text = measurementUnit.ToString();
            textBlockDimensionLength.Text = measurementUnit.ToString();

            float height, width, length;

            height = currentObject.Height * (1 / scaleFactor);
            length = currentObject.Length * (1 / scaleFactor);
            width = currentObject.Width * (1 / scaleFactor);            

            textBlockLength.Text = Math.Round(length, 1).ToString();
            textBlockHeight.Text = Math.Round(height, 1).ToString();
            textBlockWidth.Text = Math.Round(width, 1).ToString();
        }

        private Decimal GetMaterialsPrice()
        {
            Decimal sum = 0;
            for (int i = 0; i < selectedObjectMaterials.Count; i++)
            {
                double surfaceNeeded = selectedObjectMaterials[i].SurfaceNeeded;
                sum += Convert.ToDecimal(surfaceNeeded) * selectedObjectMaterials[i].Material.Price;
            }

            return sum;
        }

        private void checkBoxIsSuspendable_Checked(object sender, RoutedEventArgs e)
        {
            stackPanelChosenHeight.Visibility = Visibility.Visible;
        }

        private void checkBoxIsSuspendable_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxChosenHeight.Text = "";
            stackPanelChosenHeight.Visibility = Visibility.Collapsed;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxIsSuspendable.IsChecked == true)
            {
                if (textBoxChosenHeight.Text.Length == 0 || Convert.ToSingle(textBoxChosenHeight.Text) == 0)
                {
                    MessageBox.Show("Type a height for the suspended object!");
                    return;
                }
                else
                {
                    ChosenHeight = Convert.ToSingle(textBoxChosenHeight.Text);

                    if (currentObject.Height + ChosenHeight * scaleFactor > sceneHeight)
                    {
                        MessageBox.Show("The chosen height is invalid! Type another!");
                        return;
                    }
                }

            }

            Decimal lastPrice = currentObject.Price;
            currentObject.Price = Convert.ToDecimal(textBlockTotalPrice.Text);
            if (actualPrice- lastPrice + currentObject.Price > projectBudget)
            {
                MessageBox.Show("The object can't be edited! You are exceeding the budget!");
                return;
            }

            if (currentObject.Height > sceneHeight)
            {
                MessageBox.Show("The object can't be edited! Its height is exceeding the walls height!");
                return;
            }

           
            //currentObject.Translate = new Point3d(currentObject.Translate.X, ChosenHeight * realHeightScaleFactor * 50, currentObject.Translate.Z);
            float objectHeight = ChosenHeight * realHeightScaleFactor * 50;
            
            //InitializecurrentObjectMaterials();
            if (objectHeight > 0)
            {
                currentObject.IsSuspendable = true;
            }
            currentObject.SetMaterials(selectedObjectMaterials);
            currentObject.MaterialsPrice = Convert.ToDecimal(textBlockMaterialsPrice.Text);
            Point3d validPosition = oldObject.Translate;

            oldObject.Translate = new Point3d(oldObject.Translate.X, objectHeight, oldObject.Translate.Z);


            float td;
            if (scene.CheckCurrentObjectCollisions(oldObject, new Point3d(0, -1, 0), out td))
            {
                oldObject.Translate = validPosition;
                MessageBox.Show("The chosen height is invalid! Type another!");
                return;
            }
            oldObject.SetMaterials(selectedObjectMaterials);
            for (int i = 0; i < selectedObjectMaterials.Count;i++ )
            {
                oldObject.SetTexture(i, selectedObjectMaterials[i].Material.FullPath, openGLControl.OpenGL);
            }
                oldObject.MaterialsPrice = currentObject.MaterialsPrice;
            oldObject.Price = currentObject.Price;
            oldObject.IsSuspendable = currentObject.IsSuspendable;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            currentObject = oldObject;
            this.Close();
        }

        public WorldObject GetCurrentObject()
        {
            return oldObject;
        }
    }
}
