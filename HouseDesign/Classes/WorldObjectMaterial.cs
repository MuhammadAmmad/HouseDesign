using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class WorldObjectMaterial
    {
        public Material Material { get; set; }
        public double SurfaceNeeded { get; set; }

        public WorldObjectMaterial(Material material, double surfaceNeeded)
        {
            this.Material = material;
            this.SurfaceNeeded = surfaceNeeded;
        }

        public WorldObjectMaterial Clone()
        {
            WorldObjectMaterial material = new WorldObjectMaterial(Material, SurfaceNeeded);
            return material;
        }
    }
}
