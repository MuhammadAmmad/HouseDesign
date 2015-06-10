﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class FurnitureObject:StoredObject
    {     
        
        public Decimal InitialPrice { get; set; }
        public List<Material> Materials { get; set; }

        private WorldObject innerWorldObject;

        public FurnitureObject()
        {
            DefaultIconPath = @"D:\Licenta\HouseDesign\HouseDesign\Images\defaultObjectIcon.png";
            Materials = new List<Material>();
        }
        public FurnitureObject(String name, String fullPath, String description, Decimal initialPrice):this()
        {
            this.Name = name;
            this.FullPath = fullPath;
            if(FullPath.Length>0)
            {
                InitializeInnerObject();
            }
            this.Description = description;
            this.InitialPrice = initialPrice;
           
        }

        public WorldObject GetInnerObject()
        {
            return this.innerWorldObject.Clone();
        }

        public void AddMaterial(Material material, double surfaceNeeded)
        {
            innerWorldObject.AddMaterial(material, surfaceNeeded);
        }

        public void InitializeInnerObject()
        {
            Importer importer = new Importer();
            innerWorldObject = new WorldObject();
            innerWorldObject = importer.Import(FullPath);
        }
    }
}
