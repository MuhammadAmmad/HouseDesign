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

        public Configuration()
        {
            Categories = new List<Category<FurnitureObject>>();
            Materials = new List<Category<Material>>();
        }

        public void Reset()
        {
            Categories.Clear();
            Materials.Clear();
        }
    }
}
