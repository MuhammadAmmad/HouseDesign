using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Material:StoredObject
    {       
        public Decimal Price { get; set; }
        public String Description { get; set; } 

        public Material()
        {
        }

        public Material( String name, String imagePath, Decimal price):this()
        {
            this.Name = name;
            this.FullPath = imagePath;
            this.Price = price;
            DefaultIconPath = FullPath;

        }
    }
}
