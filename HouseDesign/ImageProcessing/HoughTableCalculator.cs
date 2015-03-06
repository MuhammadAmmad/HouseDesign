using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISIP_FrameworkGUI.Classes
{
    public class HoughTableCalculator : ILineCrossable
    {
        private int r, t;
        private int[,] houghMatrix;
        private Image<Gray, byte> image;

        public HoughTableCalculator(Image<Gray,byte> image,int houghHeight,int houghWidth)
        {
            this.image = image;
            houghMatrix = new int[houghHeight, houghWidth];
        }

        public void AtLinePoint(int x, int y)
        {
            if (image.Data[y, x, 0] == 0)
            {
                houghMatrix[r, t]++;
            }
        }

        public void SetLine(int r, int t)
        {
            this.r = r;
            this.t = t;
        }

        public int[,] GetHoughMatrix()
        {
            return houghMatrix;
        }
    }
}
