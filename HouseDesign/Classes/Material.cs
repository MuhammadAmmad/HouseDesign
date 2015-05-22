using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public class Material
    {
        public String Name { get; set; }
        public String ImagePath { get; set; }

        public Decimal Price { get; set; }

        public Material( String name, String imagePath, Decimal price)
        {
            this.Name = name;
            this.ImagePath = imagePath;
            this.Price = price;

        }
    }
}
