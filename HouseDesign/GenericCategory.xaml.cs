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
        private List<Material> selectedObjectMaterials;
        public WorldObject SelectedObject { get; set; }

        private HouseDesign.Classes.Scene scene;
        private Decimal selectedObjectInitialPrice;
        private float sceneHeight;
        public float ChosenHeight { get; set; }
        private DimensionType dimensionType;
        public GenericCategory(Category<FurnitureObject> category,  HouseDesign.Classes.Scene scene, List<Category<Material>> materials, float sceneHeight)
        {
            InitializeComponent();
            this.category = category;
            this.materials = materials;
            selectedObjectMaterials = new List<Material>();
            TreeViewItem mainTreeViewItem = new TreeViewItem();
            mainTreeViewItem.IsExpanded = true;
            treeViewCategory.Items.Add(mainTreeViewItem);
            PopulateTreeView(category, mainTreeViewItem);
            this.scene = scene;
            ChosenHeight = 0;
            this.sceneHeight = sceneHeight;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Title = category.Name;
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
            if(checkBoxIsSuspended.IsChecked==true )
            {
                if (textBoxChosenHeight.Text.Length == 0 || Convert.ToSingle(textBoxChosenHeight.Text) == 0)
                {
                    MessageBox.Show("Type a height for the suspended object!");
                    return;
                }
                else
                {
                    if(dimensionType==DimensionType.cm)
                    {
                        ChosenHeight *= 10;
                    }
                    else
                    {
                        if(dimensionType==DimensionType.m)
                        {
                            ChosenHeight *= 1000;
                        }
                    }
                    ChosenHeight = 0.05f*Convert.ToSingle(textBoxChosenHeight.Text);
                    if(SelectedObject.Height+ChosenHeight>sceneHeight*0.05f)
                    {
                        MessageBox.Show("The chosen height is invalid! Type another!");
                        return;
                    }
                }
                
            }

            SelectedObject.Price = Convert.ToDecimal(textBlockTotalPrice.Text);
            SelectedObject.Translate = new Point3d(SelectedObject.Translate.X, ChosenHeight, SelectedObject.Translate.Z);
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
            listViewMaterials.Items.Add(new CustomizeHeader("NAME", "IMAGE", "PRICE/M²", "SURFACE", "TOTAL"));
            for(int i=0;i<selectedObjectMaterials.Count;i++)
            {
                double surfaceNeeded = SelectedObject.getTotalAreaPerTexture(i);
                CustomizeMaterial customizeMaterial = new CustomizeMaterial(i, selectedObjectMaterials[i], surfaceNeeded);
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
            //selectedObjectMaterials.Remove(oldMaterial);
            //selectedObjectMaterials.Add(currentMaterial);
            int i = selectedObjectMaterials.IndexOf(oldMaterial);
            selectedObjectMaterials[i] = currentMaterial;
            //oldMaterial = currentMaterial;
            InitializeMaterials();
            InitializePrices();
        }

        void genericMaterial_StatusUpdated(object sender, EventArgs e)
        {            
            GenericMaterial g = (sender as GenericMaterial);
            Material currentMaterial = g.GetCurrentMaterial();
            SelectedObject.SetTexture(g.Index, currentMaterial.FullPath, openGLControl.OpenGL);
            //InitializeMaterials();
            //InitializePrices();
        }
        private void treeViewCategory_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedTreeViewItem=treeViewCategory.SelectedItem as TreeViewItem;
            if (treeViewCategory.Items.Count > 0 &&  selectedTreeViewItem!= null)
            {
                if((selectedTreeViewItem.Header as ExtendedTreeViewItem).Tag is FurnitureObject)
                {
                    String path = (selectedTreeViewItem.Header as ExtendedTreeViewItem).FullPath;
                    Importer importer = new Importer();
                    SelectedObject=importer.Import(path);
                    //SelectedObject.InitializeTextures(openGLControl.OpenGL);
                    if(SelectedObject!=null)
                    {
                        InitializeSelectedObjectMaterials();
                        groupBoxObj.Visibility = Visibility.Visible;
                        groupBoxPrices.Visibility = Visibility.Visible;
                        InitializeMaterials();
                        selectedObjectInitialPrice=((selectedTreeViewItem.Header as ExtendedTreeViewItem).Tag as FurnitureObject).InitialPrice;
                        InitializePrices();
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
            textBlockInitialPrice.Text = string.Format("{0:0.000}", selectedObjectInitialPrice);
            Decimal materialsPrice = GetMaterialsPrice();
            textBlockMaterialsPrice.Text = string.Format("{0:0.000}", materialsPrice);
            textBlockTotalPrice.Text = string.Format("{0:0.000}", (materialsPrice + selectedObjectInitialPrice));
        }

        private Decimal GetMaterialsPrice()
        {
            Decimal sum = 0;
            for(int i=0;i<selectedObjectMaterials.Count;i++)
            {
                double surfaceNeeded = SelectedObject.getTotalAreaPerTexture(i);
                sum += Convert.ToDecimal(surfaceNeeded) * selectedObjectMaterials[i].Price;
            }

            return sum;
        }
        private void InitializeSelectedObjectMaterials()
        {
            List<String> textures=SelectedObject.GetTextures();
            for(int i=0;i<textures.Count;i++)
            {
                Material material = new Material();
                material = GetMaterialByImagePath(materials, textures[i]);
                if(material!=null)
                {
                    selectedObjectMaterials.Add(material);
                }
                else
                {
                    MessageBox.Show("The material does not exist!");
                }
            }
        }

        private Material GetMaterialByImagePath(List<Category<Material>> materials, String imagePath)
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

        private void checkBoxIsSuspended_Checked(object sender, RoutedEventArgs e)
        {
            stackPanelChosenHeight.Visibility = Visibility.Visible;
        }

        private void checkBoxIsSuspended_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxChosenHeight.Text = "";
            stackPanelChosenHeight.Visibility = Visibility.Collapsed;
        }

        private void comboBoxDimensionType_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            dimensionType = (DimensionType)comboBoxDimensionType.SelectedValue;
        }

        private enum DimensionType
        {
            m,
            cm,
            mm
        };

        private void comboBoxDimensionType_DropDownClosed(object sender, EventArgs e)
        {
            switch(comboBoxDimensionType.SelectionBoxItem.ToString())
            {
                case "m":
                    {
                        dimensionType = DimensionType.m;
                        break;
                    }
                case"cm":
                    {
                        dimensionType = DimensionType.cm;
                        break;
                    }
                case "mm":
                    {
                        dimensionType = DimensionType.mm;
                        break;
                    }
            }
        }
    }
}
