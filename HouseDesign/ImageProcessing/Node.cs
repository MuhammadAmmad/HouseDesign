using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HouseDesign.ImageProcessing
{
    public struct Node
    {
        public int Value { get; set; }
        public int X { get; set; }

        public int Y { get; set; }
        
        public Node(int value, int x, int y): this()
        {
            this.Value = value;
            this.X = x;
            this.Y = y;
        }

        public int CompareTo(Node node, double radius)
        {
            int currentX=(node.X-this.X);
            int currentY=(node.Y-this.Y);
            double distance = Math.Sqrt(currentX * currentX + currentY * currentY);
            if(distance > radius )
            {
                return 0;
            }
            else
            {
                //if (this.Value == node.Value)
                //    return -1;
                return this.Value - node.Value;
            }
        }
    }
}
