using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public struct Triangle
    {
        public int vertex1 { get; set; }
        public int vertex2 { get; set; }
        public int vertex3 { get; set; }

        public Triangle(int vertex1, int vertex2, int vertex3):this()
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
        }
    }
}
