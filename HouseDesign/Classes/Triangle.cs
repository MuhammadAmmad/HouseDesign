using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Triangle
    {
        public int vertex1 { get; private set; }
        public int vertex2 { get; private  set; }
        public int vertex3 { get; private set; }

        public int TextureIndex { get; set; }

        public Triangle(int vertex1, int vertex2, int vertex3)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
        }

        public Triangle Clone()
        {
            Triangle triangle = new Triangle(vertex1, vertex2, vertex3);
            triangle.TextureIndex = TextureIndex;
            return triangle;
        }
    }
}
