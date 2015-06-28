using HouseDesign.Classes;
using HouseDesign.UserControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        private Configuration configuration;
        private Project currentProject;
        public Receipt(List<WorldObject> houseObjects, Configuration configuration, Project project)
        {
            InitializeComponent();
            this.houseObjects = houseObjects;
            currentCurrency = CurrencyHelper.GetProjectCurrency();
            lastCurrency = CurrencyHelper.GetProjectCurrency();
            InitializeTreeViewReceipt();
            InitializeComboBoxCurrentCurrency();
            this.configuration = configuration;
            this.currentProject = project;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
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
                houseObject.Tag = houseObjects[i];
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

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            const string directory = "Receipts";
            SaveFileDialog saveProject = new SaveFileDialog();
            saveProject.Filter = "PDF files (.pdf)|*.pdf";
            saveProject.Title = "Export Receipt";
            saveProject.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", directory));
            if (saveProject.ShowDialog() == true)
            {
                String fileName = saveProject.FileName;
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                Document doc = new Document(PageSize.A4, 36, 72, 108, 180);
                doc.SetMargins(70f, 70f, 50f, 50f);
                doc.AddTitle("RECEIPT");
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                iTextSharp.text.Paragraph newLines = new iTextSharp.text.Paragraph();
                newLines.Add("\n\n\n");
                doc.Add(newLines);
                doc.Add(newLines);
                iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph();
                title.Alignment = Element.ALIGN_CENTER;
                title.Font = FontFactory.GetFont("Times New Roman", 32, Font.UNDERLINE);
                title.Add("RECEIPT");
                doc.Add(title);
                doc.Add(newLines);
                doc.Add(newLines);

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(configuration.CurrentCompany.LogoPath);
                img.SetAbsolutePosition(380, 750);
                img.ScaleToFit(60, 60);
                doc.Add(img);

                PdfContentByte cb = writer.DirectContent;
                ColumnText ct = new ColumnText(cb);
                String text = configuration.CurrentCompany.CompanyName + '\n' + configuration.CurrentCompany.Address + '\n' +
                    configuration.CurrentCompany.TelephoneNumber.ToString() + '\n' + configuration.CurrentCompany.EmailAddress + '\n' +
                    configuration.CurrentCompany.Website + '\n';
                ct.SetSimpleColumn(new Phrase(new Chunk(text, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.BOLD))),
                                  450, 750, 600, 810, 10, Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Go();

                iTextSharp.text.Paragraph clientParagraph = new iTextSharp.text.Paragraph();
                clientParagraph.Alignment = Element.ALIGN_LEFT;
                clientParagraph.Font = FontFactory.GetFont("Times New Roman", 18, iTextSharp.text.Font.UNDERLINE);
                clientParagraph.Add("CLIENT");
                clientParagraph.Font = FontFactory.GetFont("Times New Roman", 14);
                clientParagraph.Add("\n\nName: " + currentProject.Client.Name);
                clientParagraph.Add("\nEmail: " + currentProject.Client.EmailAddress);
                clientParagraph.Add("\nTelephone Number: " + currentProject.Client.TelephoneNumber);
                clientParagraph.Add("\n\n");
                
                doc.Add(clientParagraph);
                iTextSharp.text.Paragraph currencyParagraph = new iTextSharp.text.Paragraph();
                currencyParagraph.Alignment = Element.ALIGN_LEFT;
                currencyParagraph.Font = FontFactory.GetFont("Times New Roman", 18, iTextSharp.text.Font.UNDERLINE);
                String currencyName=Enum.GetName(typeof(Currency.CurrencyName), configuration.CurrentCurrency.Name);
                currencyParagraph.Add("CURRENCY:");
                clientParagraph.Font = FontFactory.GetFont("Times New Roman", 18);
                currencyParagraph.Add(" " + currencyName);                
                doc.Add(currencyParagraph);
                doc.Add(newLines);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 90f;
                PrintHeaders(table);
                for (int i = 0; i < treeViewReceipt.Items.Count;i++ )
                {
                    HouseObjectUserControl currentObj = (treeViewReceipt.Items[i] as TreeViewItem).Header as HouseObjectUserControl;
                    if(currentObj!=null)
                    {
                        PrintHouseObject(currentObj, table);
                    }
                    
                }

                doc.Add(table);
                doc.Add(newLines);
                PdfPTable tablePrice = new PdfPTable(2);
                PrintTotalPrices(tablePrice);
                doc.Add(tablePrice);
                doc.Close();
            }
        }

        private void PrintHeaders(PdfPTable table)
        {
            var f = FontFactory.GetFont("Times New Roman", 16, Font.BOLD);
            PdfPCell cell = new PdfPCell(new Phrase("Quantity", f));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            ///table.AddCell(new Phrase("Quantity",f));
            cell = new PdfPCell(new Phrase("Name", f));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Initial Price", f));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Materials Price", f));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Total Price", f));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
        }
        private void PrintHouseObject(HouseObjectUserControl currentObject, PdfPTable table)
        {
            var f = FontFactory.GetFont("Times New Roman", 14, Font.BOLD);
            table.AddCell(new Phrase(currentObject.textBlockQuantity.Text, f));
            table.AddCell(new Phrase(currentObject.textBlockName.Text, f));
            table.AddCell(new Phrase(currentObject.textBlockInitialPrice.Text, f));
            table.AddCell(new Phrase(currentObject.textBlockMaterialsPrice.Text, f));
            table.AddCell(new Phrase(currentObject.textBlockTotalPrice.Text, f));
            WorldObject obj = currentObject.Tag as WorldObject;
            if(obj!=null)
            {
                List<WorldObjectMaterial> materials=new List<WorldObjectMaterial>();
                materials = obj.GetMaterials();
                PdfPTable materialsTable = new PdfPTable(4);
                var ff = FontFactory.GetFont("Times New Roman", 10, Font.BOLD);
                PdfPCell cell = new PdfPCell(new Phrase("Material Name", ff));
                cell.HorizontalAlignment = 1;
                materialsTable.AddCell(cell);
                cell = new PdfPCell(new Phrase("Price/m²", ff));
                cell.HorizontalAlignment = 1;
                materialsTable.AddCell(cell);
                cell = new PdfPCell(new Phrase("Surface Needed", ff));
                cell.HorizontalAlignment = 1;
                materialsTable.AddCell(cell);
                cell = new PdfPCell(new Phrase("Total Price", ff));
                cell.HorizontalAlignment = 1;
                materialsTable.AddCell(cell);
                for(int i=0;i<materials.Count;i++)
                {
                    cell = new PdfPCell(new Phrase(materials[i].Material.Name));
                    cell.HorizontalAlignment = 1;
                    materialsTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(ConvertValueToDisplay(materials[i].Material.Price)));
                    cell.HorizontalAlignment = 1;
                    materialsTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(Math.Round(materials[i].SurfaceNeeded, 2).ToString()));
                    cell.HorizontalAlignment = 1;
                    materialsTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(ConvertValueToDisplay(materials[i].Material.Price * Convert.ToDecimal(materials[i].SurfaceNeeded))));
                    cell.HorizontalAlignment = 1;
                    materialsTable.AddCell(cell);
                }

                PdfPCell cell2 = new PdfPCell(materialsTable);
                cell2.Colspan = 5;
                cell2.Padding = 0;
                cell2.Rowspan = materials.Count-1;
                table.AddCell(cell2);
            }
            
        }

        private void PrintTotalPrices(PdfPTable table)
        {
           
            var f = FontFactory.GetFont("Times New Roman", 14, Font.BOLD);
            table.HorizontalAlignment = (int)HorizontalAlignment.Right;
            
            table.AddCell(new Phrase("Total Price:", f));
            table.AddCell(new Phrase(textBlockTotalPrice.Text + " " +Enum.GetName(typeof(Currency.CurrencyName), currentCurrency.Name), f));

        }

    }
}
