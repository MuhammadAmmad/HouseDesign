using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
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
            obj.Rotating += obj_RecalculateActualPoints;
        }

        public double GetMaxY()
        {
            return ActualPoints[7].Y;
        }

        public double GetMinY()
        {
            return ActualPoints[0].Y;
        }

        public Point3d[] GetTopPoints ()
        {
            return new Point3d[] { ActualPoints[0], ActualPoints[1], ActualPoints[2], ActualPoints[3] };
        }

        void obj_RecalculateActualPoints(object sender, EventArgs e)
        {
            WorldObject obj=(sender as WorldObject);
            for (int i = 0; i < count; i++)
            {
                Point3d pR = new Point3d();
                float x = points[i].X;
                float y = points[i].Y;
                float z=points[i].Z;

                float xAngle = Convert.ToSingle(obj.Rotate.X * Math.PI) / 180;
                float yAngle = Convert.ToSingle(obj.Rotate.Y * Math.PI) / 180;
                float zAngle = Convert.ToSingle(obj.Rotate.Z * Math.PI) / 180;

                pR.X = Convert.ToSingle(x * (Math.Cos(yAngle) * Math.Cos(zAngle) - Math.Sin(xAngle) * Math.Sin(yAngle) * Math.Sin(zAngle))
                    - y * (Math.Sin(zAngle) * Math.Cos(xAngle)) + z * (Math.Sin(yAngle) * Math.Cos(zAngle) +
                    Math.Sin(xAngle) * Math.Sin(zAngle) * Math.Cos(yAngle)));

                pR.Y = Convert.ToSingle(x * (Math.Sin(zAngle) * Math.Cos(yAngle) + Math.Sin(xAngle) * Math.Sin(yAngle) * Math.Cos(zAngle)) +
                    y * (Math.Cos(xAngle) * Math.Cos(zAngle)) + z * (Math.Sin(yAngle) * Math.Sin(zAngle) 
                    - Math.Sin(xAngle) * Math.Cos(yAngle) * Math.Cos(zAngle)));

                pR.Z = Convert.ToSingle(-x * Math.Sin(yAngle) * Math.Cos(xAngle) + y * Math.Sin(xAngle) + 
                    z * Math.Cos(xAngle) * Math.Cos(yAngle));
                ActualPoints[i] = pR * obj.Scale + obj.Translate;
                //ActualPoints[i] = points[i] * obj.Scale + obj.Translate;
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
