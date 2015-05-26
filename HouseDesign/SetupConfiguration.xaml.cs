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
        private TreeViewItem selectedTreeViewItem;
        private TreeViewItem lastSelectedTreeViewItem;
        public bool IsSavedConfiguration { get; set; }
        private LastSelectedItemType selectedItemType;
        private bool canPaste;
        private bool isCopying;
        private TreeViewItem copiedItem;
        public SetupConfiguration(String title, Configuration conf)
        {
            InitializeComponent();
            mainTabControl.Height = this.Height - 70;
            this.Title = title;
            this.configuration = conf;
            IsSavedConfiguration = false;
            canPaste = false;
            isCopying = false;
            InitializeExtendedMenuItems();
            InitializeTreeViewCategories();
            InitializeTreeViewMaterials();


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

            ExtendedMenuItem extendedMenuItemSaveConfiguration = new ExtendedMenuItem("D:\\Licenta\\HouseDesign\\HouseDesign\\Images\\save.png", "Save");
            menuShortcuts.Items.Add(extendedMenuItemSaveConfiguration);
            extendedMenuItemSaveConfiguration.MouseLeftButtonDown += extendedMenuItemSaveConfiguration_MouseLeftButtonDown;
        }

        
        private void extendedMenuItemNew_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabItem currentTabItem = (TabItem)mainTabControl.SelectedItem;
            if (currentTabItem.Header.ToString() == "Categories")
            {
                AddCategory addCategory = new AddCategory("New Category", null, false, false);
                addCategory.StatusUpdated += addCategory_StatusUpdated;
                Grid grid = new Grid();
                grid.Children.Add(addCategory);
                groupBoxRightSide.Content = grid;
            }
            else
            {
                AddCategory addCategory = new AddCategory("New Category", null, false, false);
                addCategory.StatusUpdated += addCategory_StatusUpdated;
                Grid grid = new Grid();
                grid.Children.Add(addCategory);
                groupBoxRightSide.Content = grid;
            }
        }

        private void extendedMenuItemSaveConfiguration_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckSaveConfiguration();
        }

        void addCategory_StatusUpdated(object sender, EventArgs e)
        {
            Category<FurnitureObject> currentCategory = (sender as AddCategory).currentCategory;
            if((sender as AddCategory).IsEdited==true)
            {
                if(selectedTreeViewItem!=null)
                {
                    selectedTreeViewItem.Tag = currentCategory;
                    SaveCategories();
                    InitializeTreeViewCategories();
                    groupBoxRightSide.Content = null;
                }                
            }
            else
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(currentCategory.Path, currentCategory.Name, "");
                TreeViewItem item = new TreeViewItem();
                item.Tag = currentCategory;
                item.Header = extendedItem;
                if (selectedTreeViewItem == null)
                {
                    treeViewCategories.Items.Add(item);
                }
                else
                {
                    selectedTreeViewItem.Items.Add(item);
                    selectedTreeViewItem.IsExpanded = true;
                }
            }
            

        }

        private void extendedMenuItemEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedItemType==LastSelectedItemType.Category)
            {
                Category<FurnitureObject> currentCategory = selectedTreeViewItem.Tag as Category<FurnitureObject>;
                AddCategory addCategory = new AddCategory("Edit Category", currentCategory, false, true);
                addCategory.StatusUpdated += addCategory_StatusUpdated;
                Grid grid = new Grid();
                grid.Children.Add(addCategory);
                groupBoxRightSide.Content = grid;
            }
            else
            {
                if(selectedItemType==LastSelectedItemType.FurnitureObject)
                {
                    selectedItemType = LastSelectedItemType.FurnitureObject;
                    FurnitureObject currentObject = selectedTreeViewItem.Tag as FurnitureObject;
                    ImportObject importObject = new ImportObject("Import Object", currentObject, false, true);
                    importObject.StatusUpdated += importObject_StatusUpdated;
                    Grid grid = new Grid();
                    grid.Children.Add(importObject);
                    groupBoxRightSide.Content = grid;
                }

            }
        }

        private void extendedMenuItemCut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedTreeViewItem != null)
            {
                canPaste = true;
                isCopying = false;
                TreeViewItem parent = selectedTreeViewItem.Parent as TreeViewItem;
                lastSelectedTreeViewItem = selectedTreeViewItem;
                if(parent!=null)
                {
                    parent.Items.Remove(selectedTreeViewItem);
                }
                else
                {
                    
                    if(selectedItemType==LastSelectedItemType.Category)
                    {                        
                        treeViewCategories.Items.Remove(selectedTreeViewItem);                        
                    }
                    else
                    {
                        if(selectedItemType==LastSelectedItemType.CategoryMaterial)
                        {
                            treeViewMaterials.Items.Remove(selectedTreeViewItem);
                        }
                    }                    
                }
                selectedTreeViewItem = lastSelectedTreeViewItem;
            }
        }

        private void extendedMenuItemCopy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(selectedTreeViewItem!=null)
            {
                canPaste = true;
                isCopying = true;
                copiedItem = selectedTreeViewItem;

            }
        }

        private void extendedMenuItemPaste_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(canPaste)
            {
                if(selectedItemType==LastSelectedItemType.Category || selectedItemType==LastSelectedItemType.CategoryMaterial)
                {
                    
                    if(isCopying)
                    {
                        TreeViewItem item = GetTreeViewItemCopy(lastSelectedTreeViewItem);
                        for (int i = 0; i < lastSelectedTreeViewItem.Items.Count; i++)
                        {
                            TreeViewItem successorItem = GetTreeViewItemCopy(lastSelectedTreeViewItem.Items[i] as TreeViewItem);
                            item.Items.Add(successorItem);
                        }
                            selectedTreeViewItem.Items.Add(item);
                    }
                    else
                    {
                        selectedTreeViewItem.Items.Add(lastSelectedTreeViewItem);
                    }
                    isCopying = false;
                    SaveCategories();
                }
                else
                {
                    MessageBox.Show("Select a category!");
                }

                
            }
            else
            {
                MessageBox.Show("This operation is not possible! Check Copy or Cut options!");
            }
        }

        private TreeViewItem GetTreeViewItemCopy(TreeViewItem item)
        {
            TreeViewItem copy = new TreeViewItem();
            copy.Tag = item.Tag;
            ExtendedTreeViewItem f = item.Header as ExtendedTreeViewItem;
            copy.Header = new ExtendedTreeViewItem(f.IconPath, f.HeaderName, f.FullPath);
            return copy;

        }

        private void extendedMenuItemImport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabItem currentTabItem = (TabItem)mainTabControl.SelectedItem;
            if(currentTabItem.Header.ToString()=="Categories")
            {
                if(selectedItemType==LastSelectedItemType.Category)
                {
                    ImportObject importObject = new ImportObject("Import Object", null, false, false);
                    importObject.StatusUpdated += importObject_StatusUpdated;
                    Grid grid = new Grid();
                    grid.Children.Add(importObject);
                    groupBoxRightSide.Content = grid;
                }
                else
                {
                    MessageBox.Show("Select a category!");
                    return;
                }
                
            }
            else
            {
            }
        }

        void importObject_StatusUpdated(object sender, EventArgs e)
        {
            FurnitureObject importedObject = (sender as ImportObject).GetImportedObject();
            if((sender as ImportObject).IsEdited)
            {
                selectedTreeViewItem.Tag = importedObject;
                SaveCategories();
                InitializeTreeViewCategories();
                TreeViewItem parent = selectedTreeViewItem.Parent as TreeViewItem;
                if(parent!=null)
                {
                    parent.IsExpanded = true;
                }
            }
            else
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(importedObject.DefaultIconPath, importedObject.Name, importedObject.FullPath);
                TreeViewItem item = new TreeViewItem();
                item.Tag = importedObject;
                item.Header = extendedItem;
                if (selectedTreeViewItem != null)
                {
                    selectedTreeViewItem.Items.Add(item);
                    Category<FurnitureObject> currentCategory = selectedTreeViewItem.Tag as Category<FurnitureObject>;
                    currentCategory.StoredObjects.Add(importedObject);
                    SaveCategories();
                }
            }
            
        }

        private void extendedMenuItemDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(selectedTreeViewItem!=null)
            {
                TreeViewItem parent = selectedTreeViewItem.Parent as TreeViewItem;
                if(parent==null)
                {
                     TabItem currentTabItem = (TabItem)mainTabControl.SelectedItem;
                     if (currentTabItem.Header.ToString() == "Categories")
                     {
                         treeViewCategories.Items.Remove(selectedTreeViewItem);
                     }
                     else
                     {
                         treeViewMaterials.Items.Remove(selectedTreeViewItem);
                     }
                }
                else
                {
                    parent.Items.Remove(selectedTreeViewItem);
                }                
                SaveCategories();
            }
            else
            {
                MessageBox.Show("Select an item!");
            }
        }

        private void treeViewCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //selectedTreeViewItem =e.OriginalSource as TreeViewItem;
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

        private void treeViewCategories_Selected(object sender, RoutedEventArgs e)
        {
            lastSelectedTreeViewItem = selectedTreeViewItem;
            selectedTreeViewItem = e.OriginalSource as TreeViewItem;
            if (selectedTreeViewItem.Tag is Category<FurnitureObject>)
            {
                selectedItemType = LastSelectedItemType.Category;
                Category<FurnitureObject> currentCategory = selectedTreeViewItem.Tag as Category<FurnitureObject>;
                AddCategory addCategory = new AddCategory(currentCategory.Name, currentCategory, true, false);
                Grid grid = new Grid();
                grid.Children.Add(addCategory);
                groupBoxRightSide.Content = grid;
                
            }
            else
            {
                if(selectedTreeViewItem.Tag is FurnitureObject)
                {
                    selectedItemType = LastSelectedItemType.FurnitureObject;
                    FurnitureObject currentObject = selectedTreeViewItem.Tag as FurnitureObject;
                    ImportObject importObject = new ImportObject("Import Object", currentObject, true, false);
                    importObject.StatusUpdated += importObject_StatusUpdated;
                    Grid grid = new Grid();
                    grid.Children.Add(importObject);
                    groupBoxRightSide.Content = grid;
                }
                else
                {
                    if(selectedTreeViewItem.Tag is Category<Material>)
                    {
                        selectedItemType = LastSelectedItemType.CategoryMaterial;
                    }
                }
            }
        }        

        private void treeViewMaterials_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        public Configuration GetConfiguration()
        {
            return this.configuration;
        }

        public void InitializeTreeViewCategories()
        {
            treeViewCategories.Items.Clear();
            List<Category<FurnitureObject>> categories = configuration.Categories;
            for(int i=0;i<categories.Count;i++)
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(categories[i].Path, categories[i].Name, "");                
                TreeViewItem item=new TreeViewItem();
                item.Tag = categories[i];
                item.Header=extendedItem;
                InitializeTreeViewItemCategories(item);
                treeViewCategories.Items.Add(item);
            }
        }

        public void InitializeTreeViewItemCategories(TreeViewItem item)
        {
            Category<FurnitureObject> currentCategory = item.Tag as Category<FurnitureObject>;

            for(int i=0;i<currentCategory.SubCategories.Count;i++)
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(currentCategory.SubCategories[i].Path, currentCategory.SubCategories[i].Name, "");
                TreeViewItem successorItem = new TreeViewItem();
                successorItem.Tag = currentCategory.SubCategories[i];
                successorItem.Header = extendedItem;
                InitializeTreeViewItemCategories(successorItem);
                item.Items.Add(successorItem);
            }

            for (int i = 0; i < currentCategory.StoredObjects.Count; i++)
            {
                FurnitureObject currentObject = currentCategory.StoredObjects[i];
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(currentObject.DefaultIconPath, currentObject.Name, currentObject.FullPath);
                TreeViewItem objectItem = new TreeViewItem();
                objectItem.Tag = currentObject;
                objectItem.Header = extendedItem;
                item.Items.Add(objectItem);
            }
        }

        public void SaveCategories()
        {
            configuration.Categories.Clear();
            for (int i = 0; i < treeViewCategories.Items.Count; i++)
            {
                TreeViewItem currentItem = treeViewCategories.Items[i] as TreeViewItem;
                Category<FurnitureObject> currentCategory = currentItem.Tag as Category<FurnitureObject>;
                if(currentCategory!=null)
                {
                    SaveCategoryItem(currentCategory, currentItem);
                    configuration.Categories.Add(currentCategory);
                }                

            }
        }

        public void SaveCategoryItem(Category<FurnitureObject> category, TreeViewItem item)
        {
            String x = (item.Header as ExtendedTreeViewItem).HeaderName;
            if(x=="Desk Chair")
            {

            }
            category.SubCategories.Clear();
            for(int i=0;i<item.Items.Count;i++)
            {
                TreeViewItem successorItem = item.Items[i] as TreeViewItem;
                Category<FurnitureObject> successorCategory = successorItem.Tag as Category<FurnitureObject>;
                if(successorCategory!=null)
                {
                    SaveCategoryItem(successorCategory, successorItem);
                    category.SubCategories.Add(successorCategory);
                }                
            }
        }

        public void InitializeTreeViewMaterials()
        {

        }

        public void CheckSaveConfiguration()
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to save the configuration?", "Save configuration", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                SaveCategories();
                IsSavedConfiguration = true;
            }
            else
            {
                IsSavedConfiguration = false;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CheckSaveConfiguration();
        }

        public enum LastSelectedItemType
        {
            NoSelection,
            Category,
            FurnitureObject,
            CategoryMaterial,
            Material
        };

        private void mainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is TabControl)
            {
                //selectedTreeViewItem = null;
                UnselectTreeViewItem(treeViewMaterials);
                UnselectTreeViewItem(treeViewCategories);
            }
            
        }
        
        private void UnselectTreeViewItem(TreeView pTreeView)
        {
            if (pTreeView.SelectedItem == null)
                return;

            if (pTreeView.SelectedItem is TreeViewItem)
            {
                (pTreeView.SelectedItem as TreeViewItem).IsSelected = false;
            }
            else
            {
                TreeViewItem item = pTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
                if (item != null)
                {
                    item.IsSelected = true;
                    item.IsSelected = false;
                }
            }
        }

        private void treeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TreeView treeView = sender as TreeView;
            //TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            //item.IsSelected = false;
            ////selectedTreeViewItem = null;
            //treeView.Focus();
        }
    }
}
