using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.ImageProcessing
{
    public class Wall2D
    {
        private static int cornersCount = 4;
        public Vector2d[] Corners { get; private set; }
        public DirectionLine Direction { get; private set; }

        private Vector2d v1;
        private Vector2d v2;
        private Vector2d c;


        public Wall2D(Vector2d[] corners, DirectionLine direction)
        {
            this.Corners = corners;
            this.Direction = direction;

            v1 = Corners[1] - Corners[0];
            v1.X += 0.0001;
            v1.Y += 0.0001;
            v2 = Corners[3] - Corners[0];
            v2.X += 0.0001;
            v2.Y += 0.0001;
            c = Corners[0];
        }

        public Wall2D Combine(Wall2D other)
        {
            if (FullyWithin(other))
                return other;
            if (other.FullyWithin(this))
                return this;
            return null;
        }

        public bool FullyWithin(Wall2D other)
        {
            int contains = 0;
            for(int i=0;i<cornersCount;++i)
            {
                if(other.Contains(Corners[i]))
                {
                    ++contains;
                }
            }
            return contains > 2;
        }        

        public bool Contains(Vector2d p)
        {
            double alpha1 = ((p.X - c.X) * v2.Y - (p.Y - c.Y) * v2.X) / (v1.X * v2.Y - v2.X * v1.Y);
            double alpha2 = (p.X - c.X - v1.X * alpha1) / v2.X;

            return alpha1 > -0.1 && alpha1 < 1.1 && alpha2 > -0.1 && alpha2 < 1.1;
        }
    }
}
