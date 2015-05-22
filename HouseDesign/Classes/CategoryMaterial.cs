using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public class CategoryMaterial
    {
        public String Name { get; set; }
        public List<CategoryMaterial> SubCategories;
        public List<Material> Materials;

        public CategoryMaterial(String name)
        {
            this.Name=name;
            this.SubCategories = new List<CategoryMaterial>();
            this.Materials = new List<Material>();

        }
    }
}
