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
    /// Interaction logic for ImportMaterialAutomatically.xaml
    /// </summary>
    /// 
    
    public partial class ImportMaterialAutomatically : Window
    {
        private TreeViewItem selectedTreeViewItem;
        private List<Category<Material>> materials;
        private Material currentMaterial;
        public ImportMaterialAutomatically(List<Category<Material>> materials, String imagePath)
        {
            InitializeComponent();
            this.materials = materials;
            selectedTreeViewItem = null;
            currentMaterial = new Material("", imagePath, 0);
            ImportMaterial importMaterial = new ImportMaterial("Import Material", currentMaterial, false, false);
            importMaterial.StatusUpdated += importMaterial_StatusUpdated;
            Grid grid = new Grid();
            grid.Children.Add(importMaterial);
            groupBoxPreviewMaterial.Content = grid;
            InitializeTreeViewMaterials();
        }

        void importMaterial_StatusUpdated(object sender, EventArgs e)
        {
            if (selectedTreeViewItem != null)
            {
                Material importedMaterial = (sender as ImportMaterial).GetImportedMaterial();
                Category<Material> currentMaterialCategory = selectedTreeViewItem.Tag as Category<Material>;
                currentMaterialCategory.StoredObjects.Add(importedMaterial);
                currentMaterial = importedMaterial;
                this.Close();
            }
            else
            {
                ImportMaterial importMaterial = new ImportMaterial("Import Material", currentMaterial, false, false);
                importMaterial.StatusUpdated += importMaterial_StatusUpdated;
                Grid grid = new Grid();
                grid.Children.Add(importMaterial);
                groupBoxPreviewMaterial.Content = grid;
                MessageBox.Show("Select a parent category!");
                return;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void treeViewMaterials_Selected(object sender, RoutedEventArgs e)
        {
            selectedTreeViewItem = e.OriginalSource as TreeViewItem;
        }

        public void InitializeTreeViewMaterials()
        {
            treeViewMaterials.Items.Clear();
            for (int i = 0; i < materials.Count; i++)
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(materials[i].Path, materials[i].Name, "");
                TreeViewItem item = new TreeViewItem();
                item.Tag = materials[i];
                item.Header = extendedItem;
                InitializeTreeViewItemMaterials(item);
                treeViewMaterials.Items.Add(item);
            }
        }
        public void InitializeTreeViewItemMaterials(TreeViewItem item)
        {
            Category<Material> currentCategory = item.Tag as Category<Material>;

            for (int i = 0; i < currentCategory.SubCategories.Count; i++)
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(currentCategory.SubCategories[i].Path, 
                    currentCategory.SubCategories[i].Name, "");
                TreeViewItem successorItem = new TreeViewItem();
                successorItem.Tag = currentCategory.SubCategories[i];
                successorItem.Header = extendedItem;
                InitializeTreeViewItemMaterials(successorItem);
                item.Items.Add(successorItem);
            }
        }

        public List<Category<Material>> GetMaterials()
        {
            return this.materials;
        }
    }
}
