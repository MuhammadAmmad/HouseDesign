using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace HouseDesign.ImageProcessing
{
    public class Segment
    {
        public Point BeginPoint { get; set; }
        public Point EndPoint { get; set; }

        public double Length { get; set; }

        public double Slope { get; set; }

        private double n2;

        public Segment(Point beginPoint, Point endPoint)
        {
            this.BeginPoint = beginPoint;
            this.EndPoint = endPoint;
            this.Length = Math.Sqrt((endPoint.X - beginPoint.X) * (endPoint.X - beginPoint.X) + (endPoint.Y - beginPoint.Y) * (endPoint.Y - beginPoint.Y));
            this.Slope = (EndPoint.Y - beginPoint.Y) / (EndPoint.X - BeginPoint.X+0.0001);
            
            n2 = EndPoint.Y - EndPoint.X * Slope;
        }

        public double GetDistance(Point point)
        {
            Point projection = GetProjection(point);
            return Math.Sqrt((projection.Y - point.Y) * (projection.Y - point.Y) + (projection.X - point.X) * (projection.X - point.X));
        }
        public Point GetProjection(Point point)
        {

            double y = (Slope * Slope * point.Y + Slope * point.X + n2) / (Slope * Slope + 1);
            double x = point.X - (y - point.Y) * Slope;

            return new Point(x, y);
        }
    };
}
