using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.ImageProcessing
{
    public struct Vector2d
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Magnitude
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        public Vector2d(double x, double y):this()
        {
            this.X = x;
            this.Y = y;
        }

        public static double operator*(Vector2d v1, Vector2d v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static Vector2d operator *(Vector2d point, double alpha)
        {
            return new Vector2d(point.X * alpha, point.Y * alpha);
        }    

        public static Vector2d operator +(Vector2d p1, Vector2d p2)
        {
            return new Vector2d(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Vector2d operator -(Vector2d p1, Vector2d p2)
        {
            return new Vector2d(p1.X-p2.X, p1.Y-p2.Y);
        }
        
        public Vector2d Normalized()
        {
            return new Vector2d(X, Y) * (1.0 / Magnitude);
        }
    }
}
