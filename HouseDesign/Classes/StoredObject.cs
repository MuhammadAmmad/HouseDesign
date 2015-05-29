﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public abstract class StoredObject
    {
        public String Name { get; set; }
        public String FullPath { get; set; }
        public String DefaultIconPath { get; set; }
        public String Description { get; set; }
    }
}
