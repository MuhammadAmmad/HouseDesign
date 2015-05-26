using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public class BoundingBox
    {
        public List<Point3d> points { get; set; }
        
        public BoundingBox(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            points = new List<Point3d>();
            points.Add(new Point3d(minX, minY, minZ));
            points.Add(new Point3d(maxX, minY, minZ));
            points.Add(new Point3d(maxX, minY, maxZ));
            points.Add(new Point3d(minX, minY, maxZ));
            points.Add(new Point3d(minX, maxY, minZ));
            points.Add(new Point3d(maxX, maxY, minZ));
            points.Add(new Point3d(maxX, maxY, maxZ));
            points.Add(new Point3d(minX, maxY, maxZ));

        }

        public bool CheckCollision(Point3d point, Point3d direction)
        {
            return true;
        }

    }
}
