﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Category<T> where T:StoredObject
    {
        public String Name { get; set; }
        public List<Category<T>> SubCategories { get; set; }
        public List<T> StoredObjects { get; set; }
        public String Path { get; set; }
        public String Description { get; set; }
        public double TradeAllowance { get; set; }

        public Category(String name, String path, String description, double tradeAllowance)
        {
            this.Name = name;
            this.Path = path;
            this.Description = description;
            this.TradeAllowance = tradeAllowance;
            SubCategories = new List<Category<T>>();
            StoredObjects = new List<T>();
        }
    }
}
