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
    public partial class GenericCategory : Window, IDisposable
    {
        private Category<FurnitureObject> category;
        private List<Category<Material>> materials;
        private List<WorldObjectMaterial> selectedObjectMaterials;
        public WorldObject SelectedObject { get; set; }
        private Decimal selectedObjectInitialPrice;
        private float sceneHeight;
        public float ChosenHeight { get; set; }
        private Decimal currentTradeAllowance;
        private Decimal actualPrice;
        private Decimal projectBudget;
        private Project.UnitOfMeasurement measurementUnit;
        private float realHeightScaleFactor;
        private float scaleFactor;
        public GenericCategory(Category<FurnitureObject> category, List<Category<Material>> materials, float sceneHeight, 
            Decimal actualPrice, Decimal projectBudget, Project.UnitOfMeasurement measurementUnit)
        {
            InitializeComponent();
            this.category = category;
            this.materials = materials;
            selectedObjectMaterials = new List<WorldObjectMaterial>();
            TreeViewItem mainTreeViewItem = new TreeViewItem();
            mainTreeViewItem.IsExpanded = true;
            InitializeScaleFactors();
            treeViewCategory.Items.Add(mainTreeViewItem);
            PopulateTreeView(category, mainTreeViewItem);
            ChosenHeight = 0;
            this.sceneHeight = sceneHeight*0.0025f;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Title = category.Name;
            currentTradeAllowance = Convert.ToDecimal(category.TradeAllowance);
            this.actualPrice = actualPrice;
            this.projectBudget = projectBudget;
            this.measurementUnit = measurementUnit;
            textBlockMeasurementUnit.Text = measurementUnit.ToString();
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
        public void PopulateTreeView(Category<FurnitureObject> mainCategory, TreeViewItem currentItem)
        {
            foreach(FurnitureObject obj in mainCategory.StoredObjects)
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(obj.DefaultIconPath, obj.Name, obj.FullPath);
                extendedItem.Tag = obj;
                TreeViewItem item = new TreeViewItem();
                item.Header = extendedItem;
                currentItem.Items.Add(item);
            }
            foreach (Category<FurnitureObject> c in mainCategory.SubCategories)
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(c.Path, c.Name, c.Path);
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
            //OpenGL gl = openGLControl.OpenGL;
            OpenGL gl = args.OpenGL as OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


            //  Load the identity matrix.
            gl.LoadIdentity();



            //  Rotate around the Y axis.
            gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

            if (SelectedObject != null)
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

        private void btnAddToScene_Click(object sender, RoutedEventArgs e)
        {
            if(checkBoxIsSuspendable.IsChecked==true )
            {
                if (textBoxChosenHeight.Text.Length == 0 || Convert.ToSingle(textBoxChosenHeight.Text) == 0)
                {
                    MessageBox.Show("Type a height for the suspended object!");
                    return;
                }
                else
                {
                    ChosenHeight = Convert.ToSingle(textBoxChosenHeight.Text);                   
                    if(SelectedObject.Height+ChosenHeight*scaleFactor>sceneHeight)
                    {
                        MessageBox.Show("The chosen height is invalid! Type another!");
                        return;
                    }
                }
                
            }

            SelectedObject.Price = Convert.ToDecimal(textBlockTotalPrice.Text);
            if (actualPrice + SelectedObject.Price > projectBudget)
            {
                MessageBox.Show("The object can't be added! You are exceeding the budget!");
                return;
            }

            if(SelectedObject.Height>sceneHeight)
            {
                MessageBox.Show("The object can't be added! Its height is exceeding the walls height!");
                return;
            }
            SelectedObject.Translate = new Point3d(SelectedObject.Translate.X, ChosenHeight*realHeightScaleFactor*50, SelectedObject.Translate.Z);
            //InitializeSelectedObjectMaterials();
            SelectedObject.SetMaterials(selectedObjectMaterials);
            SelectedObject.MaterialsPrice=Convert.ToDecimal(textBlockMaterialsPrice.Text);

            if (ChosenHeight * realHeightScaleFactor * 50>0)
            {
                SelectedObject.IsSuspendable = true;
            }
            
            this.Close();
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCustomizeObject_Click(object sender, RoutedEventArgs e)
        {
            groupBoxCustomizeCurrentObject.Visibility = Visibility.Visible;
            InitializeMaterials();           
        }

        public void InitializeMaterials()
        {
            listViewMaterials.Items.Clear();
            listViewMaterials.Items.Add(new CustomizeHeader("NAME", "IMAGE", "PRICE/M²", "SURFACE", "TOTAL", ""));
            for(int i=0;i<selectedObjectMaterials.Count;i++)
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
            SelectedObject.SetTexture(g.Index, currentMaterial.FullPath, openGLControl.OpenGL);
            //InitializeMaterials();
            //InitializePrices();
        }
        private void treeViewCategory_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedTreeViewItem=treeViewCategory.SelectedItem as TreeViewItem;
            if (treeViewCategory.Items.Count > 0 &&  selectedTreeViewItem!= null)
            {
                FurnitureObject currentObject=(selectedTreeViewItem.Header as ExtendedTreeViewItem).Tag as FurnitureObject;
                if(currentObject!=null)
                {
                    SelectedObject=currentObject.GetInnerObject();
                    selectedObjectMaterials.Clear();
                    selectedObjectMaterials.AddRange(currentObject.GetInnerObject().GetMaterials());
                    SelectedObject.Name = currentObject.Name;
                    SelectedObject.InitialPrice = currentObject.InitialPrice;
                    if(SelectedObject!=null)
                    {
                        //InitializeSelectedObjectMaterials();
                        groupBoxObj.Visibility = Visibility.Visible;
                        groupBoxPrices.Visibility = Visibility.Visible;
                        InitializeMaterials();
                        selectedObjectInitialPrice=currentObject.InitialPrice;                   
                        InitializePrices();
                        InitializeDimensions();
                    }                    
                }
                else
                {
                    groupBoxPrices.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void InitializePrices()
        {
            Currency projectCurrency=CurrencyHelper.GetProjectCurrency();
            Decimal actualInitialPrice = CurrencyHelper.FromCurrencyToCurrency(CurrencyHelper.GetCurrentCurrency(), selectedObjectInitialPrice, projectCurrency);
            textBlockInitialPrice.Text = string.Format("{0:0.000}", actualInitialPrice);
            Decimal materialsPrice = GetMaterialsPrice();
            Decimal actualMaterialsPrice = CurrencyHelper.FromCurrencyToCurrency(CurrencyHelper.GetCurrentCurrency(), materialsPrice, projectCurrency);
            textBlockMaterialsPrice.Text = string.Format("{0:0.000}", actualMaterialsPrice);

            textBlockTotalPrice.Text = string.Format("{0:0.000}", (actualMaterialsPrice + actualInitialPrice +
                currentTradeAllowance/100*(actualMaterialsPrice + actualInitialPrice)));
        }

        public void InitializeDimensions()
        {
            textBlockDimensionHeight.Text = measurementUnit.ToString();
            textBlockDimensionWidth.Text = measurementUnit.ToString();
            textBlockDimensionLength.Text = measurementUnit.ToString();            

            float height, width, length;
            height = SelectedObject.Height * (1 / scaleFactor);
            length = SelectedObject.Length * (1 /scaleFactor);
            width = SelectedObject.Width * (1 / scaleFactor);

            textBlockLength.Text = Math.Round(length, 1).ToString();
            textBlockHeight.Text = Math.Round(height, 1).ToString();
            textBlockWidth.Text = Math.Round(width, 1).ToString();
        }

        private Decimal GetMaterialsPrice()
        {
            Decimal sum = 0;
            for(int i=0;i<selectedObjectMaterials.Count;i++)
            {
                double surfaceNeeded = selectedObjectMaterials[i].SurfaceNeeded;
                sum += Convert.ToDecimal(surfaceNeeded) * selectedObjectMaterials[i].Material.Price;
            }

            return sum;
        }
        private void InitializeSelectedObjectMaterials()
        {
            selectedObjectMaterials.Clear();
            selectedObjectMaterials.AddRange(SelectedObject.GetMaterials());
        }

        public static Material GetMaterialByImagePath(List<Category<Material>> materials, String imagePath)
        {
            for(int i=0;i<materials.Count;i++)
            {
                for(int j=0;j<materials[i].StoredObjects.Count;j++)
                {
                    Material material = materials[i].StoredObjects[j];
                    if(material.FullPath==imagePath)
                    {
                        return material;
                    }
                }
                GetMaterialByImagePath(materials[i].SubCategories, imagePath);
            }

            return null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
        public void Dispose()
        {
            SelectedObject = null;
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
                
    }
}
