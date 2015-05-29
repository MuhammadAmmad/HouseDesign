using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public class BoundingBox
    {
        private Point3d[] points;

        public Point3d[] ActualPoints { get; private set; }
        
        private int count;
        public BoundingBox(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, WorldObject obj)
        {
            count = 8;
            points = new Point3d[count];
            points[0]=new Point3d(minX, minY, minZ);
            points[1]=new Point3d(maxX, minY, minZ);
            points[2]=new Point3d(maxX, minY, maxZ);
            points[3]=new Point3d(minX, minY, maxZ);
            points[4]=new Point3d(minX, maxY, minZ);
            points[5]=new Point3d(maxX, maxY, minZ);
            points[6]=new Point3d(maxX, maxY, maxZ);
            points[7]=new Point3d(minX, maxY, maxZ);

            ActualPoints = new Point3d[count];
            for (int i = 0; i < count;i++ )
            {
                ActualPoints[i] = points[i];
            }

            obj.Translating += obj_RecalculateActualPoints;
            obj.Scaling += obj_RecalculateActualPoints;

        }
        void obj_RecalculateActualPoints(object sender, EventArgs e)
        {
            WorldObject obj=(sender as WorldObject);
            for (int i = 0; i < count; i++)
            {
                ActualPoints[i] = points[i] * obj.Scale + obj.Translate;
            }
        }

        public bool CheckCollision(Point3d point, Point3d direction)
        {
            return true;
        }

        public Point3d[] GetPoints()
        {
            return points;
        }

    }
}
