using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public class Wall
    {
        public Point3d P1 { get; set; }
        public Point3d P2 { get; set; }
        public Point3d P3 { get; set; }
        public Point3d P4 { get; set; }

        public Wall(Point3d p1, Point3d p2, Point3d p3, Point3d p4)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.P3 = p3;
            this.P4 = p4;
        }
    }
}
