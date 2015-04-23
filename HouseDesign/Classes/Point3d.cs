﻿using System;
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

        public static Point3d operator +(Point3d p1,Point3d p2)
        {
            return new Point3d(p1.X+p2.X, p1.Y+p2.Y, p1.Z+p2.Z);
        }
        public static Point3d operator -(Point3d p1, Point3d p2)
        {
            return new Point3d(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }
        public static Point3d operator *(Point3d p1, float value)
        {
            return new Point3d(p1.X * value, p1.Y * value, p1.Z * value);
        }

        public Point3d RotateY(float angle)
        {
            return new Point3d(Convert.ToSingle(X * Math.Cos(angle * Math.PI / 180) + Z * Math.Sin(angle * Math.PI / 180)), Y, Convert.ToSingle(-X * Math.Sin(angle * Math.PI / 180) + Z * Math.Cos(angle * Math.PI / 180)));
        }

        public Point3d RotateX(float angle)
        {
            return new Point3d(X, Convert.ToSingle(Y * Math.Cos(angle * Math.PI / 180) - Z * Math.Sin(angle * Math.PI / 180)), Convert.ToSingle(Y * Math.Sin(angle * Math.PI / 180) + Z * Math.Cos(angle * Math.PI / 180)));
        }

    }
}
