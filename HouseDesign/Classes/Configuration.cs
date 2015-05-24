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
        public List<Category> Categories { get; set; }
        public List<Material> Materials { get; set; }
        public bool IsEmpty { get; set; }

        public Configuration()
        {
            Categories = new List<Category>();
            Materials = new List<Material>();
        }

        public void Reset()
        {
            Categories.Clear();
            Materials.Clear();
        }
    }
}
