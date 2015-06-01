using HouseDesign.Classes;
using HouseDesign.UserControls;
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
    /// Interaction logic for GenericMaterial.xaml
    /// </summary>
    public partial class GenericMaterial : Window
    {
        public event EventHandler StatusUpdated;
        private List<Category<Material>> materials;
        private Material currentMaterial;
        public int Index { get; set; }
        public GenericMaterial(List<Category<Material>> materials, int index)
        {
            InitializeComponent();
            this.materials = new List<Category<Material>>();            
            this.materials.AddRange(materials);
            this.Index = index;
            InitializeTreeViewMaterials();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        public void InitializeTreeViewMaterials()
        {
            for(int i=0;i<materials.Count;i++)
            {
                TreeViewItem item = new TreeViewItem();
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(materials[i].Path, materials[i].Name, materials[i].Path);
                item.Tag = materials[i];
                item.Header = extendedItem;
                InitializeTreeViewItemMaterials(materials[i], item);
                treeViewMaterials.Items.Add(item);
            }
        }

        public void InitializeTreeViewItemMaterials(Category<Material> materialCategory, TreeViewItem item)
        {
            for (int i = 0; i < materialCategory.SubCategories.Count;i++ )
            {
                Category<Material> currentCategory = materialCategory.SubCategories[i];
                TreeViewItem successorItem = new TreeViewItem();
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(currentCategory.Path, currentCategory.Name, currentCategory.Path);
                successorItem.Tag = currentCategory;
                successorItem.Header = extendedItem;
                InitializeTreeViewItemMaterials(currentCategory, successorItem);
                item.Items.Add(successorItem);
            }

            for (int i = 0; i < materialCategory.StoredObjects.Count; i++)
            {
                Material material = materialCategory.StoredObjects[i];
                TreeViewItem successorItem = new TreeViewItem();
                ExtendedTreeViewItem extendedSuccessor = new ExtendedTreeViewItem(material.FullPath, material.Name, material.FullPath);
                successorItem.Tag = material;
                successorItem.Header = extendedSuccessor;
                item.Items.Add(successorItem);
            }

        }
        private void btnAddMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (this.StatusUpdated != null)
            {
                this.StatusUpdated(this, new EventArgs());
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void treeViewMaterials_Selected(object sender, RoutedEventArgs e)
        {
            Material material=(e.OriginalSource as TreeViewItem).Tag as Material;
            if(material!=null)
            {
                currentMaterial = material;
                ImportMaterial previewMaterial = new ImportMaterial(material.Name, material, true, false);
                Grid grid = new Grid();
                grid.Children.Add(previewMaterial);
                groupBoxPreviewMaterial.Content = grid;
            }
            else
            {
                groupBoxPreviewMaterial.Content = null;
            }
        }

        public Material GetCurrentMaterial()
        {
            return currentMaterial;
        }
    }
}
