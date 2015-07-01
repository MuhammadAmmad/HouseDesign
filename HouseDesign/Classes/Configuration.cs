using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Controls;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Configuration
    {
        public List<Category<FurnitureObject>> Categories { get; set; }
        public List<Category<Material>> Materials { get; set; }
        public bool IsEmpty { get; set; }

        public Currency CurrentCurrency { get; set; }

        public Company CurrentCompany { get; set; }

        public Configuration()
        {
            Categories = new List<Category<FurnitureObject>>();
            Materials = new List<Category<Material>>();
            CurrentCompany = new Company();
        }

        public void Reset()
        {
            Categories.Clear();
            Materials.Clear();
            CurrentCompany = null;
        }

        public void ConvertAllPrices(Currency lastCurrency, Currency currentCurrency)
        {
            for (int i = 0; i < Categories.Count; i++)
            {
                ConvertFurnitureObjectPrices(lastCurrency, currentCurrency, Categories[i]);
            }

            for(int i=0;i<Materials.Count;i++)
            {
                ConvertMaterialPrices(lastCurrency, currentCurrency, Materials[i]);
            }
        }

        private void ConvertFurnitureObjectPrices(Currency lastCurrency, Currency currentCurrency, Category<FurnitureObject> currentCategory)
        {
           
            for (int j = 0; j < currentCategory.StoredObjects.Count; j++)
            {
                currentCategory.StoredObjects[j].InitialPrice = CurrencyHelper.FromCurrencyToCurrency(lastCurrency,
                    currentCategory.StoredObjects[j].InitialPrice, currentCurrency);
            }

            for(int j=0;j<currentCategory.SubCategories.Count;j++)
            {
                ConvertFurnitureObjectPrices(lastCurrency, currentCurrency, currentCategory.SubCategories[j]);
            }
        }

        private void ConvertMaterialPrices(Currency lastCurrency, Currency currentCurrency, Category<Material> currentCategory)
        {
            for(int j=0;j<currentCategory.StoredObjects.Count;j++)
            {
                currentCategory.StoredObjects[j].Price = CurrencyHelper.FromCurrencyToCurrency(lastCurrency, 
                    currentCategory.StoredObjects[j].Price, currentCurrency);
            }

            for(int j=0;j<currentCategory.SubCategories.Count;j++)
            {
                ConvertMaterialPrices(lastCurrency, currentCurrency, currentCategory.SubCategories[j]);
            }
        }

        public void ClearAllFurnitureObjects()
        {
            for(int i=0;i<Categories.Count;i++)
            {
                ClearAllFurnitureObjectItems(Categories[i]);
            }
        }

        public void ClearAllFurnitureObjectItems(Category<FurnitureObject> category)
        {
            category.StoredObjects.Clear();
            for(int i=0;i<category.SubCategories.Count;i++)
            {
                ClearAllFurnitureObjectItems(category.SubCategories[i]);
            }
        }

        //TO BE BETTER IMPLEMENTED
        public bool Equals(Configuration conf)
        {
            if(this.CurrentCurrency.Name!=conf.CurrentCurrency.Name || this.IsEmpty!=conf.IsEmpty)
            {
                return false;
            }
            if(this.Materials.Count!=conf.Materials.Count || this.Categories.Count!=conf.Categories.Count)
            {
                return false;
            }

            return true;
        }
        
    }
}
