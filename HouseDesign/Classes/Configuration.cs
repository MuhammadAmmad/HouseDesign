using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Configuration
    {
        private List<Category> categories;
        private List<CategoryMaterial> materials;
        public bool IsEmpty { get; set; }

        public Configuration()
        {
            categories = new List<Category>();
            materials = new List<CategoryMaterial>();
        }

        public List<Category> GetCategories()
        {
            return categories;
        }

        public List<CategoryMaterial> GetMaterials()
        {
            return materials;
        }
    }
}
