using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.ImageProcessing
{
    public struct Point2d
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point2d(int x, int y):this()
        {
            this.X = x;
            this.Y = y;
        }
    }
}
