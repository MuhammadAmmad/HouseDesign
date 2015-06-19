using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.ImageProcessing
{
    public class DirectionLine
    {
        public Vector2d Point { get; set; }
        public Vector2d Direction { get; set; }

        public DirectionLine(Vector2d point, Vector2d direction)
        {
            this.Point = point;
            this.Direction = direction.Normalized();
        }

        public Vector2d GetPointByX(double x)
        {
            double alpha = (x - Point.X) / Direction.X;
            double y = Point.Y + alpha * Direction.Y;

            return new Vector2d(x, y);
        }

        public Vector2d GetPointByY(double y)
        {
            double alpha = (y - Point.Y) / Direction.Y;
            double x = Point.X + alpha * Direction.X;

            return new Vector2d(x, y);
        }

        public double GetPositionOfX(double x)
        {
            return (x - Point.X) / Direction.X;
        }

        public double GetPositionOfY(double y)
        {
            return (y - Point.Y) / Direction.Y;
        }

        public Vector2d GetPointByPosition(double alpha)
        {
            return Point + Direction * alpha;
        }

        public static double GetAngleBetweenLines(DirectionLine line1, DirectionLine line2)
        {
            return Math.Acos((line1.Direction * line2.Direction) / (line1.Direction.Magnitude * line2.Direction.Magnitude));
        }

        public static double GetDistanceBetweenLines(DirectionLine line1, DirectionLine line2)
        {
            Vector2d prim = line1.GetPointProjection(line2.Point);
            return (prim - line2.Point).Magnitude;
        }

        public double AlphaProjection(Vector2d projectionPoint)
        {
            return ((projectionPoint.X - Point.X) * Direction.X + (projectionPoint.Y - Point.Y) * Direction.Y) / (Direction.X * Direction.X + Direction.Y * Direction.Y);
        }

        public Vector2d GetPointProjection(Vector2d projectionPoint)
        {
            double alpha = AlphaProjection(projectionPoint);
            return Point + Direction * alpha;
        }

        public static DirectionLine GetMiddle(DirectionLine line1,DirectionLine line2)
        {
            return new DirectionLine((line1.Point + line2.Point) * 0.5, line1.Direction + line2.Direction);
        }
        
    }
}
