using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class CategoryMaterial
    {
        public String Name { get; set; }
        public List<CategoryMaterial> SubCategories;
        public List<Material> Materials;
        public String Description { get; set; }
        public String Path { get; set; }

        public CategoryMaterial(String name, string path)
        {
            this.Name=name;
            this.Path = path;
            this.SubCategories = new List<CategoryMaterial>();
            this.Materials = new List<Material>();

        }
    }
}
