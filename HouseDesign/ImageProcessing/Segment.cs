using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace ISIP_FrameworkGUI.Classes
{
    public class Segment
    {
        public Point BeginPoint { get; set; }
        public Point EndPoint { get; set; }

        public Segment(Point beginPoint, Point endPoint)
        {
            this.BeginPoint = beginPoint;
            this.EndPoint = endPoint;
        }
    };
}
