using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HouseDesign.ImageProcessing
{
    public class HoughTableCalculator : ILineCrossable
    {
        private int r, t;
        private int[,] houghMatrix;
        private Image<Gray, byte> image;
        int houghHeight;
        int houghWidth;
        List<Node> maximums;

        public HoughTableCalculator(Image<Gray,byte> image,int houghHeight,int houghWidth)
        {
            this.image = image;
            houghMatrix = new int[houghHeight, houghWidth];
            this.houghHeight = houghHeight;
            this.houghWidth = houghWidth;
            maximums = new List<Node>();
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

        public int[,] GetLocalMaximum(int maskHDimension, int maskWDimension, int threshold)
        {
            int[,] result = new int[houghHeight, houghWidth];
            double radius = 8;
            
            for(int i=maskHDimension;i<houghHeight-maskHDimension;i++)
            {
                for(int j=maskWDimension;j<houghWidth-maskWDimension;j++)
                {
                    if(isMaximum(i, j, maskHDimension, maskWDimension, threshold))
                    {
                        maximums.Add(new Node(houghMatrix[i, j], j, i));
                    }
                }
            }

            MessageBox.Show(maximums.Count + "CCC");

            for (int i = 0; i < maximums.Count - 1;i++ )
            {
                Node currentNode=maximums[i];
                for(int j=i+1;j<maximums.Count;j++)
                {
                    int currentValue=currentNode.CompareTo(maximums[j], radius);
                    if(currentValue>0)
                    {
                        maximums.RemoveAt(j);
                        j--;
                    }
                    else
                    {
                        if(currentValue<0)
                        {
                            maximums.RemoveAt(i);
                            i--;
                            break;
                            
                        }
                    }
                }
            }

            //double value = Math.Sqrt((maximums[1].X - maximums[0].X) * (maximums[1].X - maximums[0].X) + (maximums[1].Y - maximums[0].Y) * (maximums[1].Y - maximums[0].Y));
            //MessageBox.Show(value+"BLABALA");
            for (int i = 0; i < maximums.Count;i++ )
            {
                result[maximums[i].Y, maximums[i].X] = maximums[i].Value;
            }

                return result;
        }

        public bool isMaximum(int l, int c, int maskHDImension, int maskWDimension, int threshold)
        {
            int currentItem=houghMatrix[l, c];
            if(currentItem < threshold)
            {
                return false;
            }
            for(int i=l-maskHDImension;i<=l+maskHDImension;i++)
            {
                for(int j=c-maskWDimension;j<=c+maskWDimension;j++)
                {
                    if(houghMatrix[i, j] > currentItem)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
