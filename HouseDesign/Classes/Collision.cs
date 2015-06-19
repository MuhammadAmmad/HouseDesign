using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public interface ICollision
    {
        float GetMaxTD(Point3d d);
    }

    public class PointInObjectCollision2D : ICollision
    {
        public Point3d P1 { get; set; }
        public Point3d P2 { get; set; }
        public Point3d V2 { get; set; }
        public Point3d V3 { get; set; }

        public PointInObjectCollision2D(Point3d p1, Point3d p2, Point3d v2, Point3d v3)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.V2 = v2;
            this.V3 = v3;
        }

        public float GetMaxTD(Point3d d)
        {
            Point3d p1 = P1 - d;
            float[,] system1 = new float[2, 3];
            system1[0, 0] = d.X;
            system1[0, 1] = -V3.X;
            system1[0, 2] = P2.X - p1.X;
            system1[1, 0] = d.Z;
            system1[1, 1] = -V3.Z;
            system1[1, 2] = P2.Z - p1.Z;

            float[,] system2 = new float[2, 3];
            system2[0, 0] = d.X;
            system2[0, 1] = -V3.X;
            system2[0, 2] = P2.X + V2.X - p1.X;
            system2[1, 0] = d.Z;
            system2[1, 1] = -V3.Z;
            system2[1, 2] = P2.Z + V2.Z - p1.Z;

            float[,] system3 = new float[2, 3];
            system3[0, 0] = d.X;
            system3[0, 1] = -V2.X;
            system3[0, 2] = P2.X + V3.X - p1.X;
            system3[1, 0] = d.Z;
            system3[1, 1] = -V2.Z;
            system3[1, 2] = P2.Z + V3.Z - p1.Z;

            float[,] system4 = new float[2, 3];
            system4[0, 0] = d.X;
            system4[0, 1] = -V2.X;
            system4[0, 2] = P2.X - p1.X;
            system4[1, 0] = d.Z;
            system4[1, 1] = -V2.Z;
            system4[1, 2] = P2.Z - p1.Z;

            float t1 = 0, t2 = 0;
            float max = 0;

            WorldObject.GetSystem2DSolutions(system1, ref t1, ref t2);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem2DSolutions(system2, ref t1, ref t2);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem2DSolutions(system3, ref t1, ref t2);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem2DSolutions(system4, ref t1, ref t2);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }

            return max;
        }
    }

    public class Collision2D : ICollision
    {
        public Point3d P1 { get; set; }
        public Point3d P2 { get; set; }
        public Point3d V1 { get; set; }
        public Point3d V2 { get; set; }

        public Collision2D(Point3d p1, Point3d v1, Point3d p2, Point3d v2)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.V1 = v1;
            this.V2 = v2;
        }

        public float GetMaxTD(Point3d d)
        {
            Point3d p1 = P1 - d;
            float[,] system1 = new float[2, 3];
            system1[0, 0] = d.X;
            system1[0, 1] = -V2.X;
            system1[0, 2] = P2.X - p1.X - V1.X;
            system1[1, 0] = d.Z;
            system1[1, 1] = -V2.Z;
            system1[1, 2] = P2.Z - p1.Z - V1.Z;

            float[,] system2 = new float[2, 3];
            system2[0, 0] = d.X;
            system2[0, 1] = -V2.X;
            system2[0, 2] = P2.X - p1.X;
            system2[1, 0] = d.Z;
            system2[1, 1] = -V2.Z;
            system2[1, 2] = P2.Z - p1.Z;

            float[,] system3 = new float[2, 3];
            system3[0, 0] = d.X;
            system3[0, 1] = V1.X;
            system3[0, 2] = P2.X + V2.X - p1.X;
            system3[1, 0] = d.Z;
            system3[1, 1] = V1.Z;
            system3[1, 2] = P2.Z + V2.Z - p1.Z;

            float[,] system4 = new float[2, 3];
            system4[0, 0] = d.X;
            system4[0, 1] = V1.X;
            system4[0, 2] = P2.X - p1.X;
            system4[1, 0] = d.Z;
            system4[1, 1] = V1.Z;
            system4[1, 2] = P2.Z - p1.Z;

            float t1 = 0, t2 = 0;
            WorldObject.GetSystem2DSolutions(system1, ref t1, ref t2);
            float max = 0;
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem2DSolutions(system2, ref t1, ref t2);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem2DSolutions(system3, ref t1, ref t2);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem2DSolutions(system4, ref t1, ref t2);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }

            return max;
        }
    }

    public class Collision
    {
        public Point3d P1 { get; set; }
        public Point3d P2 { get; set; }
        public Point3d V1 { get; set; }
        public Point3d V2 { get; set; }
        public Point3d V3 { get; set; }

        public Collision(Point3d p1, Point3d p2, Point3d v1, Point3d v2, Point3d v3)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
        }

        public float GetMaxTD(Point3d d)
        {
            Point3d p1 = P1 - d;
            float[,] system1 = new float[3, 4];
            system1[0, 0] = d.X;
            system1[0, 1] = -V2.X;
            system1[0, 2] = -V3.X;
            system1[0, 3] = P2.X - V1.X-p1.X;

            system1[1, 0] = d.Y;
            system1[1, 1] = -V2.Y;
            system1[1, 2] = -V3.Y;
            system1[1, 3] = P2.Y - V1.Y-p1.Y;

            system1[2, 0] = d.Z;
            system1[2, 1] = -V2.Z;
            system1[2, 2] = -V3.Z;
            system1[2, 3] = P2.Z - V1.Z-p1.Z;

            float[,] system2 = new float[3, 4];
            system2[0, 0] = d.X;
            system2[0, 1] = V1.X;
            system2[0, 2] = -V3.X;
            system2[0, 3] = P2.X + V2.X - p1.X;

            system2[1, 0] = d.Y;
            system2[1, 1] = V1.Y;
            system2[1, 2] = -V3.Y;
            system2[1, 3] = P2.Y + V2.Y - p1.Y;

            system2[2, 0] = d.Z;
            system2[2, 1] = V1.Z;
            system2[2, 2] = -V3.Z;
            system2[2, 3] = P2.Z + V2.Z - p1.Z;

            float[,] system3 = new float[3, 4];
            system3[0, 0] = d.X;
            system3[0, 1] = V1.X;
            system3[0, 2] = -V2.X;
            system3[0, 3] = P2.X + V3.X - p1.X;

            system3[1, 0] = d.Y;
            system3[1, 1] = V1.Y;
            system3[1, 2] = -V2.Y;
            system3[1, 3] = P2.Y + V3.Y - p1.Y;

            system3[2, 0] = d.Z;
            system3[2, 1] = V1.Z;
            system3[2, 2] = -V2.Z;
            system3[2, 3] = P2.Z + V3.Z - p1.Z;

            float[,] system4 = new float[3, 4];
            system4[0, 0] = d.X;
            system4[0, 1] = V1.X;
            system4[0, 2] = -V3.X;
            system4[0, 3] = P2.X - p1.X;

            system4[1, 0] = d.Y;
            system4[1, 1] = V1.Y;
            system4[1, 2] = -V3.Y;
            system4[1, 3] = P2.Y - p1.Y;

            system4[2, 0] = d.Z;
            system4[2, 1] = V1.Z;
            system4[2, 2] = -V3.Z;
            system4[2, 3] = P2.Z - p1.Z;

            float[,] system5 = new float[3, 4];
            system5[0, 0] = d.X;
            system5[0, 1] = V1.X;
            system5[0, 2] = -V2.X;
            system5[0, 3] = P2.X - p1.X;

            system5[1, 0] = d.Y;
            system5[1, 1] = V1.Y;
            system5[1, 2] = -V2.Y;
            system5[1, 3] = P2.Y - p1.Y;

            system5[2, 0] = d.Z;
            system5[2, 1] = V1.Z;
            system5[2, 2] = -V2.Z;
            system5[2, 3] = P2.Z - p1.Z;

            float[,] system6 = new float[3, 4];
            system6[0, 0] = d.X;
            system6[0, 1] = -V2.X;
            system6[0, 2] = -V3.X;
            system6[0, 3] = P2.X - p1.X;

            system6[1, 0] = d.Y;
            system6[1, 1] = -V2.Y;
            system6[1, 2] = -V3.Y;
            system6[1, 3] = P2.Y - p1.Y;

            system6[2, 0] = d.Z;
            system6[2, 1] = -V2.Z;
            system6[2, 2] = -V3.Z;
            system6[2, 3] = P2.Z - p1.Z;

            float t1 = 0, t2 = 0, t3 = 0;
            WorldObject.GetSystem3DSolutions(system1, ref t1, ref t2, ref t3);
            float max = 0;
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem3DSolutions(system2, ref t1, ref t2, ref t3);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem3DSolutions(system3, ref t1, ref t2, ref t3);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem3DSolutions(system4, ref t1, ref t2, ref t3);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem3DSolutions(system5, ref t1, ref t2, ref t3);
            if (max < t1 && t1 < 1)
            {
                max = t1;
            }
            WorldObject.GetSystem3DSolutions(system6, ref t1, ref t2, ref t3);
            if (max < t1 && t1 < 1 )
            {
                max = t1;
            }

            return max;
        }
    }
}
