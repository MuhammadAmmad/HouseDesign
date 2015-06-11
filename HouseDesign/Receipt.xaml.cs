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
    /// Interaction logic for Receipt.xaml
    /// </summary>
    public partial class Receipt : Window
    {
        private List<WorldObject> houseObjects;
        private Currency lastCurrency;
        private Currency currentCurrency;
        public Receipt(List<WorldObject> houseObjects)
        {
            InitializeComponent();
            this.houseObjects = houseObjects;
            currentCurrency = CurrencyHelper.GetProjectCurrency();
            lastCurrency = CurrencyHelper.GetProjectCurrency();
            InitializeTreeViewReceipt();
            InitializeComboBoxCurrentCurrency();
        }

        private void InitializeTreeViewReceipt()
        {
            treeViewReceipt.Items.Clear();
            int count;
            Decimal totalPrice = 0;
            for (int i = 0; i < houseObjects.Count; i++)
            {
                count = 1;
                int j = i + 1;
                while (j < houseObjects.Count && CheckEquivalenceBetweenObjects(houseObjects[i], houseObjects[j]))
                {
                    count++;
                    j++;
                }
                i=j-1;
                totalPrice += count * houseObjects[i].Price;
                TreeViewItem item = new TreeViewItem();
                HouseObjectUserControl houseObject = new HouseObjectUserControl(count, houseObjects[i].Name, ConvertValueToDisplay(houseObjects[i].InitialPrice),
                    ConvertValueToDisplay(houseObjects[i].MaterialsPrice), ConvertValueToDisplay(houseObjects[i].Price));
                item.Header = houseObject;
                List<WorldObjectMaterial> currentObjectMaterials = houseObjects[i].GetMaterials();
                InitializeMaterialHeaders(item);
                for(int k=0;k<currentObjectMaterials.Count;k++)
                {
                    TreeViewItem materialItem = new TreeViewItem();
                    Decimal totalPriceMaterial=Convert.ToDecimal(currentObjectMaterials[k].SurfaceNeeded) * currentObjectMaterials[k].Material.Price;
                    MaterialUserControl material = new MaterialUserControl(currentObjectMaterials[k].Material.FullPath,
                        currentObjectMaterials[k].Material.Name, ConvertValueToDisplay(currentObjectMaterials[k].Material.Price),
                        ConvertValueToDisplay(Convert.ToDecimal(currentObjectMaterials[k].SurfaceNeeded)), 
                        ConvertValueToDisplay(totalPriceMaterial));
                    materialItem.Header = material;
                    item.Items.Add(materialItem);
                }

                treeViewReceipt.Items.Add(item);
                
            }

            textBlockTotalPrice.Text = ConvertValueToDisplay(totalPrice);
        }

        private void InitializeMaterialHeaders(TreeViewItem item)
        {
            Grid gridHeaders = new Grid();
            ColumnDefinition header = new ColumnDefinition();
            header.Width = new GridLength(50, GridUnitType.Pixel);
            gridHeaders.ColumnDefinitions.Add(header);

            header = new ColumnDefinition();
            header.Width = new GridLength(150, GridUnitType.Pixel);
            gridHeaders.ColumnDefinitions.Add(header);

            header = new ColumnDefinition();
            header.Width = new GridLength(100, GridUnitType.Pixel);
            gridHeaders.ColumnDefinitions.Add(header);

            header = new ColumnDefinition();
            header.Width = new GridLength(100, GridUnitType.Pixel);
            gridHeaders.ColumnDefinitions.Add(header);

            header = new ColumnDefinition();
            header.Width = new GridLength(100, GridUnitType.Pixel);
            gridHeaders.ColumnDefinitions.Add(header);

            TextBlock headerText = new TextBlock();
            headerText.Text = "Preview";
            headerText.FontSize = 12;
            headerText.FontWeight = FontWeights.Bold;
            headerText.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(headerText, 0);
            gridHeaders.Children.Add(headerText);

            headerText = new TextBlock();
            headerText.Text = "Name";
            headerText.FontSize = 12;
            headerText.FontWeight = FontWeights.Bold;
            headerText.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(headerText, 1);
            gridHeaders.Children.Add(headerText);

            headerText = new TextBlock();
            headerText.Text = "Price/m²";
            headerText.FontSize = 12;
            headerText.FontWeight = FontWeights.Bold;
            headerText.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(headerText, 2);
            gridHeaders.Children.Add(headerText);

            headerText = new TextBlock();
            headerText.Text = "Surface";
            headerText.FontSize = 12;
            headerText.FontWeight = FontWeights.Bold;
            headerText.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(headerText, 3);
            gridHeaders.Children.Add(headerText);

            headerText = new TextBlock();
            headerText.Text = "Total Price";
            headerText.FontSize = 12;
            headerText.FontWeight = FontWeights.Bold;
            headerText.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(headerText, 4);
            gridHeaders.Children.Add(headerText);

            TreeViewItem headerItem = new TreeViewItem();
            headerItem.Header = gridHeaders;
            item.Items.Add(headerItem);
        }

        private static bool CheckEquivalenceBetweenObjects(WorldObject obj1, WorldObject obj2)
        {
            if(obj1.Price!=obj2.Price)
            {
                return false;
            }
            List<WorldObjectMaterial> materialsObj1 = obj1.GetMaterials();
            List<WorldObjectMaterial> materialsObj2 = obj2.GetMaterials();

            if(materialsObj1.Count!=materialsObj2.Count)
            {
                return false;
            }
            else
            {
                for(int i=0;i<materialsObj1.Count;i++)
                {
                    if(materialsObj1[i].SurfaceNeeded != materialsObj2[i].SurfaceNeeded || 
                        materialsObj1[i].Material.FullPath!=materialsObj2[i].Material.FullPath)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private String ConvertValueToDisplay(Decimal value)
        {
            
            Decimal actualValue = CurrencyHelper.FromCurrencyToCurrency(lastCurrency, value, currentCurrency);
            return String.Format("{0:0.000}", actualValue);
        }

        private void comboBoxCurrencies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem currentItem = comboBoxCurrencies.SelectedItem as ComboBoxItem;
            if (currentItem.Content != null)
            {
                Currency.CurrencyName currencyName = (Currency.CurrencyName)Enum.Parse(typeof(Currency.CurrencyName), currentItem.Content.ToString());
                if(currencyName!=Currency.CurrencyName.RON)
                {
                    currentCurrency = CurrencyHelper.GetCurrencyByName(currencyName);
                }
                else
                {
                    currentCurrency = CurrencyHelper.GetDefaultCurrency();
                }

                InitializeComboBoxCurrentCurrency();
                InitializeTreeViewReceipt();
            }   
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InitializeComboBoxCurrentCurrency()
        {

            comboBoxCurrencies.SelectedValue = currentCurrency.Name.ToString();
        }
    }
}
