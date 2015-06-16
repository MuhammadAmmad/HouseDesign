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
        private LastSelectedItemType lastSelectedItemType;
        private bool canPaste;
        private bool isCopying;
        private TreeViewItem copiedItem;
        public bool IsEditedCurrency { get; set; }



        public String CurrentCurrency
        {
            get { return (String)GetValue(CurrentCurrencyProperty); }
            set { SetValue(CurrentCurrencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentCurrency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCurrencyProperty =
            DependencyProperty.Register("CurrentCurrency", typeof(String), typeof(SetupConfiguration));

        
        public SetupConfiguration(String title, Configuration conf)
        {
            InitializeComponent();
            mainTabControl.Height = this.Height - 70;
            this.Title = title;
            this.configuration = conf;
            CurrentCurrency = CurrencyHelper.GetCurrentCurrency().Name.ToString();
            IsSavedConfiguration = false;
            canPaste = false;
            isCopying = false;
            InitializeExtendedMenuItems();
            InitializeTreeViewCategories();
            InitializeTreeViewMaterials();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            if(configuration.CurrentCompany==null)
            {
                companyInformation.isEdited = true;
            }


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
                AddCategory addCategory = new AddCategory("New Category", null, false, false, @"D:\Licenta\HouseDesign\HouseDesign\Icons\IconsCategory");
                addCategory.StatusUpdated += addCategory_StatusUpdated;
                Grid grid = new Grid();
                grid.Children.Add(addCategory);
                groupBoxRightSide.Content = grid;
            }
            else
            {
                AddMaterialCategory addMaterialCategory = new AddMaterialCategory("New Material Category", null, false, false, @"D:\Licenta\HouseDesign\HouseDesign\Icons\IconsMaterialCategory");
                addMaterialCategory.StatusUpdated += addMaterialCategory_StatusUpdated;
                Grid grid = new Grid();
                grid.Children.Add(addMaterialCategory);
                groupBoxPreviewMaterial.Content = grid;
            }
        }

        void addMaterialCategory_StatusUpdated(object sender, EventArgs e)
        {
            Category<Material> currentCategory = (sender as AddMaterialCategory).CurrentCategory;
            if ((sender as AddMaterialCategory).IsEdited == true)
            {
                if (selectedTreeViewItem != null)
                {
                    selectedTreeViewItem.Tag = currentCategory;
                    SaveMaterials();
                    InitializeTreeViewMaterials();
                    groupBoxPreviewMaterial.Content = null;
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
                    treeViewMaterials.Items.Add(item);
                }
                else
                {
                    selectedTreeViewItem.Items.Add(item);
                    selectedTreeViewItem.IsExpanded = true;
                }
            }
        }

        private void extendedMenuItemSaveConfiguration_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckSaveConfiguration();
        }

        void addCategory_StatusUpdated(object sender, EventArgs e)
        {
            Category<FurnitureObject> currentCategory = (sender as AddCategory).CurrentCategory;
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

        private void importObject_StatusUpdated(object sender, EventArgs e)
        {
            ImportObject importObject=sender as ImportObject;
            FurnitureObject importedObject = importObject.GetImportedObject();
            if ((sender as ImportObject).IsEdited)
            {
                selectedTreeViewItem.Tag = importedObject;
                SaveCategories();
                InitializeTreeViewCategories();
                TreeViewItem parent = selectedTreeViewItem.Parent as TreeViewItem;
                if (parent != null)
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
                if(importObject.ExistingImportedMaterials==true)
                {
                    configuration.Materials = importObject.GetMaterials();
                    InitializeTreeViewMaterials();
                    groupBoxPreviewMaterial.Content = null;
                }
            }
        }

        private void extendedMenuItemEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabItem item = mainTabControl.SelectedItem as TabItem;
            if(item!=null)
            {
                if(item.Header.ToString()=="Company")
                {
                    companyInformation.UnsetReadOnlyFields();
                    companyInformation.isEdited = true;
                    return;
                }
            }
            if (selectedItemType==LastSelectedItemType.Category)
            {
                Category<FurnitureObject> currentCategory = selectedTreeViewItem.Tag as Category<FurnitureObject>;
                AddCategory addCategory = new AddCategory("Edit Category", currentCategory, false, true, @"D:\Licenta\HouseDesign\HouseDesign\Icons\IconsCategory");
                addCategory.StatusUpdated += addCategory_StatusUpdated;
                Grid grid = new Grid();
                grid.Children.Add(addCategory);
                groupBoxRightSide.Content = grid;
            }
            else
            {
                if(selectedItemType==LastSelectedItemType.FurnitureObject)
                {
                    //electedItemType = LastSelectedItemType.FurnitureObject;
                    FurnitureObject currentObject = selectedTreeViewItem.Tag as FurnitureObject;
                    double tradeAllowance = ((selectedTreeViewItem.Parent as TreeViewItem).Tag as Category<FurnitureObject>).TradeAllowance;
                    ImportObject importObject = new ImportObject("Import Object", currentObject, configuration.Materials, false, true, tradeAllowance);
                    importObject.StatusUpdated += importObject_StatusUpdated;
                    importObject.ImportMaterialStatusUpdated += importObject_ImportMaterialStatusUpdated;
                    Grid grid = new Grid();
                    grid.Children.Add(importObject);
                    groupBoxRightSide.Content = grid;
                }
                else
                {
                    if(selectedItemType==LastSelectedItemType.CategoryMaterial)
                    {
                        Category<Material> currentMaterialCategory = selectedTreeViewItem.Tag as Category<Material>;
                        AddMaterialCategory addMaterialCategory = new AddMaterialCategory("Edit Material Category", currentMaterialCategory, false, true, @"D:\Licenta\HouseDesign\HouseDesign\Icons\IconsCategory");
                        addMaterialCategory.StatusUpdated += addMaterialCategory_StatusUpdated;
                        Grid grid = new Grid();
                        grid.Children.Add(addMaterialCategory);
                        groupBoxPreviewMaterial.Content = grid;

                    }
                    else
                    {
                        Material currentMaterial = selectedTreeViewItem.Tag as Material;
                        ImportMaterial importMaterial = new ImportMaterial("Import Material", currentMaterial, false, true);
                        importMaterial.StatusUpdated += importMaterial_StatusUpdated;
                        Grid grid = new Grid();
                        grid.Children.Add(importMaterial);
                        groupBoxPreviewMaterial.Content = grid;
                    }
                }

            }
        }

        void importObject_ImportMaterialStatusUpdated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
                if(selectedItemType!=lastSelectedItemType)
                {
                    if(lastSelectedItemType==LastSelectedItemType.FurnitureObject && selectedItemType==LastSelectedItemType.Category 
                        || lastSelectedItemType==LastSelectedItemType.Material && selectedItemType==LastSelectedItemType.CategoryMaterial)
                    {
                        TreeViewItem item = GetTreeViewItemCopy(lastSelectedTreeViewItem);
                        selectedTreeViewItem.Items.Add(item);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Check the type of the paste Category!");
                        return;
                    }                    
                }
                if(selectedItemType==LastSelectedItemType.Category || selectedItemType==LastSelectedItemType.CategoryMaterial)
                {
                    
                    if(isCopying)
                    {
                        TreeViewItem item = GetTreeViewItemCopy(lastSelectedTreeViewItem);
                        //for (int i = 0; i < lastSelectedTreeViewItem.Items.Count; i++)
                        //{
                        //    TreeViewItem successorItem = GetTreeViewItemCopy(lastSelectedTreeViewItem.Items[i] as TreeViewItem);
                        //    item.Items.Add(successorItem);
                        //}
                            selectedTreeViewItem.Items.Add(item);
                    }
                    else
                    {
                        selectedTreeViewItem.Items.Add(lastSelectedTreeViewItem);
                    }
                    //isCopying = false;
                    canPaste=false;
                    SaveCategories();
                    SaveMaterials();
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
            for (int i = 0; i < item.Items.Count;i++ )
            {
                TreeViewItem currentSuccessor = GetTreeViewItemCopy(item.Items[i] as TreeViewItem);
                copy.Items.Add(currentSuccessor);
            }
                return copy;

        }

        private void extendedMenuItemImport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabItem currentTabItem = (TabItem)mainTabControl.SelectedItem;
            if(currentTabItem!=null)
            {
                if (currentTabItem.Header.ToString() == "Categories")
                {
                    if (selectedItemType == LastSelectedItemType.Category)
                    {
                        if(treeViewMaterials.Items.Count==0)
                        {
                            MessageBox.Show("Please define material categories!");
                            mainTabControl.SelectedIndex = 1;
                            return;
                        }
                        double tradeAllowance = (selectedTreeViewItem.Tag as Category<FurnitureObject>).TradeAllowance;
                        ImportObject importObject = new ImportObject("Import Object", null, configuration.Materials, false, false, tradeAllowance);
                        importObject.StatusUpdated += importObject_StatusUpdated;
                        importObject.ImportMaterialStatusUpdated += importObject_ImportMaterialStatusUpdated;
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
                    if (selectedItemType == LastSelectedItemType.CategoryMaterial)
                    {
                        ImportMaterial importMaterial = new ImportMaterial("Import Material", null, false, false);
                        importMaterial.StatusUpdated += importMaterial_StatusUpdated;
                        Grid grid = new Grid();
                        grid.Children.Add(importMaterial);
                        groupBoxPreviewMaterial.Content = grid;
                    }
                    else
                    {
                        MessageBox.Show("Select a category!");
                        return;
                    }
                }
            }
               
        }
        void importMaterial_StatusUpdated(object sender, EventArgs e)
        {
            Material importedMaterial = (sender as ImportMaterial).GetImportedMaterial();
            if ((sender as ImportMaterial).IsEdited)
            {
                selectedTreeViewItem.Tag = importedMaterial;
                SaveMaterials();
                InitializeTreeViewMaterials();
                TreeViewItem parent = selectedTreeViewItem.Parent as TreeViewItem;
                if (parent != null)
                {
                    parent.IsExpanded = true;
                }
                //(sender as ImportMaterial).SetImportedMaterial(null);
            }
            else
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(importedMaterial.FullPath, importedMaterial.Name, importedMaterial.FullPath);
                TreeViewItem item = new TreeViewItem();
                item.Tag = importedMaterial;
                item.Header = extendedItem;
                if (selectedTreeViewItem != null)
                {
                    selectedTreeViewItem.Items.Add(item);
                    Category<Material> currentCategory = selectedTreeViewItem.Tag as Category<Material>;
                    currentCategory.StoredObjects.Add(importedMaterial);
                    SaveMaterials();
                }
                (sender as ImportMaterial).SetImportedMaterial(new Material());
            } 
        }
        //void MaterialimportObject_StatusUpdated(object sender, EventArgs e)
        //{
        //    FurnitureObject importedObject = (sender as ImportObject).GetImportedObject();
        //    if((sender as ImportObject).IsEdited)
        //    {
        //        selectedTreeViewItem.Tag = importedObject;
        //        SaveCategories();
        //        InitializeTreeViewCategories();
        //        TreeViewItem parent = selectedTreeViewItem.Parent as TreeViewItem;
        //        if(parent!=null)
        //        {
        //            parent.IsExpanded = true;
        //        }
        //    }
        //    else
        //    {
        //        ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(importedObject.DefaultIconPath, importedObject.Name, importedObject.FullPath);
        //        TreeViewItem item = new TreeViewItem();
        //        item.Tag = importedObject;
        //        item.Header = extendedItem;
        //        if (selectedTreeViewItem != null)
        //        {
        //            selectedTreeViewItem.Items.Add(item);
        //            Category<FurnitureObject> currentCategory = selectedTreeViewItem.Tag as Category<FurnitureObject>;
        //            currentCategory.StoredObjects.Add(importedObject);
        //            SaveCategories();
        //        }
        //    }           
        //}
        private void extendedMenuItemDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(selectedTreeViewItem!=null)
            {
                TreeViewItem parent = selectedTreeViewItem.Parent as TreeViewItem;
                if(parent==null)
                {
                     if (selectedItemType==LastSelectedItemType.Category || selectedItemType==LastSelectedItemType.FurnitureObject)
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
                SaveMaterials();
            }
            else
            {
                MessageBox.Show("Select an item!");
            }
        }

        private void treeViewCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }
        private void treeViewCategories_Selected(object sender, RoutedEventArgs e)
        {
            lastSelectedTreeViewItem = selectedTreeViewItem;
            lastSelectedItemType = selectedItemType;
            selectedTreeViewItem = e.OriginalSource as TreeViewItem;
            if (selectedTreeViewItem.Tag is Category<FurnitureObject>)
            {
                selectedItemType = LastSelectedItemType.Category;
                Category<FurnitureObject> currentCategory = selectedTreeViewItem.Tag as Category<FurnitureObject>;
                AddCategory addCategory = new AddCategory(currentCategory.Name, currentCategory, true, false, @"D:\Licenta\HouseDesign\HouseDesign\Icons\IconsCategory");
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
                    double tradeAllowance = ((selectedTreeViewItem.Parent as TreeViewItem).Tag as Category<FurnitureObject>).TradeAllowance;
                    ImportObject importObject = new ImportObject("Object", currentObject, configuration.Materials, true, false, tradeAllowance);
                    importObject.StatusUpdated += importObject_StatusUpdated;
                    Grid grid = new Grid();
                    grid.Children.Add(importObject);
                    groupBoxRightSide.Content = grid;
                }
            }
        }
        private void treeViewMaterials_Selected(object sender, RoutedEventArgs e)
        {
            if(selectedItemType==LastSelectedItemType.CategoryMaterial || selectedItemType==LastSelectedItemType.Material)
            {
                lastSelectedTreeViewItem = selectedTreeViewItem;
                lastSelectedItemType = selectedItemType;
            }
            
            selectedTreeViewItem = e.OriginalSource as TreeViewItem;
            if (selectedTreeViewItem.Tag is Category<Material>)
            {
                selectedItemType = LastSelectedItemType.CategoryMaterial;
                Category<Material> currentCategory = selectedTreeViewItem.Tag as Category<Material>;
                AddMaterialCategory addCategory = new AddMaterialCategory(currentCategory.Name, currentCategory, true, false, @"D:\Licenta\HouseDesign\HouseDesign\Icons\IconsMaterialCategory");
                Grid grid = new Grid();
                grid.Children.Add(addCategory);
                groupBoxPreviewMaterial.Content = grid;

            }
            else
            {
                if (selectedTreeViewItem.Tag is Material)
                {
                    selectedItemType = LastSelectedItemType.Material;
                    Material material = selectedTreeViewItem.Tag as Material;
                    ImportMaterial importMaterial = new ImportMaterial("Material", material, true, false);
                    importMaterial.StatusUpdated += importMaterial_StatusUpdated;
                    Grid grid = new Grid();
                    grid.Children.Add(importMaterial);
                    groupBoxPreviewMaterial.Content = grid;
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
        public void InitializeTreeViewMaterials()
        {
            treeViewMaterials.Items.Clear();
            List<Category<Material>> categories = configuration.Materials;
            for (int i = 0; i < categories.Count; i++)
            {
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(categories[i].Path, categories[i].Name, "");
                TreeViewItem item = new TreeViewItem();
                item.Tag = categories[i];
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
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(currentCategory.SubCategories[i].Path, currentCategory.SubCategories[i].Name, "");
                TreeViewItem successorItem = new TreeViewItem();
                successorItem.Tag = currentCategory.SubCategories[i];
                successorItem.Header = extendedItem;
                InitializeTreeViewItemMaterials(successorItem);
                item.Items.Add(successorItem);
            }

            for (int i = 0; i < currentCategory.StoredObjects.Count; i++)
            {
                Material material = currentCategory.StoredObjects[i];
                ExtendedTreeViewItem extendedItem = new ExtendedTreeViewItem(material.FullPath, material.Name, material.FullPath);
                TreeViewItem objectItem = new TreeViewItem();
                objectItem.Tag = material;
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
        public void SaveMaterials()
        {
            configuration.Materials.Clear();
            for (int i = 0; i < treeViewMaterials.Items.Count; i++)
            {
                TreeViewItem currentItem = treeViewMaterials.Items[i] as TreeViewItem;
                Category<Material> currentCategory = currentItem.Tag as Category<Material>;
                if (currentCategory != null)
                {
                    SaveMaterialItem(currentCategory, currentItem);
                    configuration.Materials.Add(currentCategory);
                }

            }
        }

        public void SaveMaterialItem(Category<Material> materialCategory, TreeViewItem item)
        {
            materialCategory.SubCategories.Clear();
            materialCategory.StoredObjects.Clear();
            for (int i = 0; i < item.Items.Count; i++)
            {
                TreeViewItem successorItem = item.Items[i] as TreeViewItem;
                Category<Material> successorCategory = successorItem.Tag as Category<Material>;
                if (successorCategory != null)
                {
                    SaveMaterialItem(successorCategory, successorItem);
                    materialCategory.SubCategories.Add(successorCategory);
                }
                else
                {
                    Material material = successorItem.Tag as Material;
                    if (material != null)
                    {
                        materialCategory.StoredObjects.Add(material);
                    }
                }
                
                   
            }
        }

        public void SaveCategoryItem(Category<FurnitureObject> category, TreeViewItem item)
        {
            category.SubCategories.Clear();
            category.StoredObjects.Clear();
            for(int i=0;i<item.Items.Count;i++)
            {
                TreeViewItem successorItem = item.Items[i] as TreeViewItem;
                Category<FurnitureObject> successorCategory = successorItem.Tag as Category<FurnitureObject>;
                if(successorCategory!=null)
                {
                    SaveCategoryItem(successorCategory, successorItem);
                    category.SubCategories.Add(successorCategory);
                }
                else
                {
                    FurnitureObject furnitureObject=successorItem.Tag as FurnitureObject;
                    if(furnitureObject!=null)
                    {
                        category.StoredObjects.Add(furnitureObject);
                    }
                }
            }
        }

        public void CheckSaveConfiguration()
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to save the configuration?", "Save configuration", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                SaveCategories();
                SaveMaterials();
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
                selectedTreeViewItem = null;
                UnselectTreeViewItem(treeViewMaterials);
                UnselectTreeViewItem(treeViewCategories);
                groupBoxCurrencies.Visibility = Visibility.Collapsed;
                TabItem item = (e.Source as TabControl).SelectedItem as TabItem;
                if(item!=null && item.Header.ToString()=="Company")
                {
                    InitializeCompany();
                }
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

        private void btnChangeCurrency_Click(object sender, RoutedEventArgs e)
        {
            InitializeCurrencies();
            groupBoxCurrencies.Visibility = Visibility.Visible;
        }

        public void InitializeCurrencies()
        {
            List<Currency> currencies = CurrencyHelper.GetCurrencies();
            Currency defaultCurrency=CurrencyHelper.GetDefaultCurrency();
            CurrencyUserControl headers = new CurrencyUserControl("CURRENCY", "NAME", "VALUE", "RELATIVE CURRENCY");
            headers.Tag="Header";
            listViewCurrencies.Items.Add(headers);
            for(int i=0;i<currencies.Count;i++)
            {
                CurrencyUserControl currency = new CurrencyUserControl(currencies[i], defaultCurrency);
                currency.Tag = currencies[i];
                listViewCurrencies.Items.Add(currency);
            }

            CurrencyUserControl defaultCurrencyUserControl = new CurrencyUserControl(defaultCurrency, defaultCurrency);
            defaultCurrencyUserControl.Tag = defaultCurrency;
            listViewCurrencies.Items.Add(defaultCurrencyUserControl);
        }

        private void listViewCurrencies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Currency currentCurrency = (listViewCurrencies.SelectedItem as CurrencyUserControl).Tag as Currency;
            if(currentCurrency!=null)
            {
                String currencyInternationalName=Enum.GetName(typeof(Currency.InternationalName), (int)currentCurrency.Name);
                CurrencyHelper.SetLastCurrency(CurrencyHelper.GetCurrentCurrency());
                CurrencyHelper.SetCurrentCurrency(currentCurrency);
                CurrentCurrency = currentCurrency.Name.ToString();
                MessageBox.Show("The current currency is: " + currentCurrency.Name + " - " + currencyInternationalName);
                configuration.ConvertAllPrices(CurrencyHelper.GetLastCurrency(), CurrencyHelper.GetCurrentCurrency());
                configuration.CurrentCurrency = CurrencyHelper.GetCurrentCurrency();
                IsEditedCurrency = true;
                InitializeTreeViewCategories();
                InitializeTreeViewMaterials();
                groupBoxPreviewMaterial.Content = null;
                groupBoxRightSide.Content = null;
            }
        }

        private void InitializeCompany()
        {
            if(configuration.CurrentCompany!=null)
            {
                companyInformation.textBoxCompanyName.Text = configuration.CurrentCompany.CompanyName;
                companyInformation.textBoxAddress.Text = configuration.CurrentCompany.Address;
                companyInformation.textBoxTelephoneNumber.Text = configuration.CurrentCompany.TelephoneNumber.ToString();
                companyInformation.textBoxEmailAddress.Text = configuration.CurrentCompany.EmailAddress;
                companyInformation.textBoxWebsite.Text = configuration.CurrentCompany.Website;
                companyInformation.SetReadOnlyFields();
                companyInformation.imgLogo.Source = new BitmapImage(new Uri(configuration.CurrentCompany.LogoPath));
            }
            else
            {
                companyInformation.UnsetReadOnlyFields();
            }
            
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(companyInformation.isEdited)
            {
                if(configuration.CurrentCompany==null)
                {
                    String companyName=companyInformation.textBoxCompanyName.Text;
                    String address=companyInformation.textBoxAddress.Text;
                    long telephoneNumber=Convert.ToInt64(companyInformation.textBoxTelephoneNumber.Text);
                    String email=companyInformation.textBoxEmailAddress.Text;
                    String website=companyInformation.textBoxWebsite.Text;
                    if(companyName.Length==0 || address.Length==0 || companyInformation.textBoxTelephoneNumber.Text.Length==0 
                        || email.Length==0 || website.Length==0 || companyInformation.imgLogo.Tag==null)
                    {
                        MessageBox.Show("Complete all fields!");
                        return;
                    }
                    Company company = new Company(companyName, address, telephoneNumber, email, website, 
                        companyInformation.imgLogo.Tag.ToString());
                    configuration.CurrentCompany = company;
                }
                else
                {
                    configuration.CurrentCompany.CompanyName = companyInformation.textBoxCompanyName.Text;
                    configuration.CurrentCompany.Address = companyInformation.textBoxAddress.Text;
                    configuration.CurrentCompany.TelephoneNumber = Convert.ToInt64(companyInformation.textBoxTelephoneNumber.Text);
                    configuration.CurrentCompany.EmailAddress = companyInformation.textBoxEmailAddress.Text;
                    configuration.CurrentCompany.Website = companyInformation.textBoxWebsite.Text;
                    configuration.CurrentCompany.LogoPath = companyInformation.imgLogo.Tag.ToString();
                }
                
               
                MessageBox.Show("The company information was successfully edited!");
                companyInformation.SetReadOnlyFields();
                companyInformation.isEdited = false;
            }
        }

        
    }
}
