using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public struct Point3d
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Point3d(float x, float y, float z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
