using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace ISIP_FrameworkGUI.Classes
{
    public static class HoughLines
    {
        static int[,] houghMatrix;
        static int houghWidth;
        static int houghHeight;
        static int originalImageWidth;
        static int originalImageHeight;
        static Image<Gray, byte> image;
        const double radianConverter = Math.PI / 180.0;
        delegate void CrossLine(int r,int t, ILineCrossable function);

        public static void InitializeHoughMatrix(Image<Gray, byte> originalImage)
        {
            originalImageWidth = originalImage.Width;
            originalImageHeight = originalImage.Height;
            image=originalImage;
            houghWidth = 270;
            houghHeight = Convert.ToInt16(Math.Sqrt(originalImageHeight * originalImageHeight + originalImageWidth * originalImageWidth));

            HoughTableCalculator houghCalculator = new HoughTableCalculator(image,houghHeight,houghWidth);

            int r, y, t;
            for (r = 0; r < houghHeight; r++)
            {
                for (t = 0; t < houghWidth; t++)
                {
                    if ((t >= 0 && t <= 45) || (t >= 135 && t <= 225))
                    {
                        CrossLineViaX(r, t, houghCalculator);
                    }
                    else
                    {
                        CrossLineViaY(r, t, houghCalculator);
                    }
                }               
            }

            houghMatrix = houghCalculator.GetHoughMatrix();
        }

        public static List<Segment> GetSegments(Image<Gray, byte> image, int threshold)
        {
            InitializeHoughMatrix(image);
            List<Segment> segments = new List<Segment>();
            for (int r = 0; r < houghHeight; r++)
            {               
                for (int t = 0; t < houghWidth; t++)
                {
                    if(houghMatrix[r, t]>threshold)
                    {
                        if ((t >= 0 && t <= 45) || (t >= 135 && t <= 225))
                        {
                            segments.AddRange(GetSegmentFromPair(r, t, CrossLineViaX));
                        }
                        else
                        {
                            segments.AddRange(GetSegmentFromPair(r, t, CrossLineViaY));
                        }
                        
                    }
                }
            }

            return segments;
        }
               
        private static List<Segment> GetSegmentFromPair(int r, int t, CrossLine crossLine)
        {
            LineSegmentator segmentator = new LineSegmentator(image);
            crossLine(r, t, segmentator);

            return segmentator.GetSegments();
        }

        public static Image<Gray, byte> NiceTry(Image<Gray, byte> originalImage)
        {
            int height=originalImage.Height;
            int width=originalImage.Width;
            Image<Gray, byte> result = new Image<Gray, byte>(width, height);
            int d=2;
            int size = 12;


            for(int i=d;i<height-d-1;i++)
            {
                for(int j=d;j<width-d-1;j++)
                {
                    if(originalImage.Data[i, j, 0]==0)
                    {
                        int count = 0;
                        for(int k=i-d;k<=i+d;k++)
                        {
                            for(int l=j-d;l<=j+d;l++)
                            {
                                if(originalImage.Data[k,l,0]==0)
                                {
                                    count++;
                                }
                            }
                        }
                        if(count<size)
                        {
                            result.Data[i, j, 0] = 255;
                        }

                    }
                    else
                    {
                        result.Data[i, j, 0] = 255;
                    }
                }
            }

            //return result;
            return originalImage;
        }
        
        private static void CrossLineViaX(int r, int t, ILineCrossable function)
        {
            int y;
            function.SetLine(r, t);
            for (int x = 0; x < originalImageWidth; x++)
            {
                y = Convert.ToInt32((r - x * Math.Cos((t - 90) * radianConverter)) / (Math.Sin((t - 90) * radianConverter)));
                if (y > 0 && y < originalImageHeight)
                {
                    function.AtLinePoint( x, y);
                }
            }
        }

        private static void CrossLineViaY(int r, int t, ILineCrossable function)
        {
            int x;
            function.SetLine(r, t);
            for (int y = 0; y < originalImageHeight; y++)
            {
                x = Convert.ToInt32((r - y * Math.Sin((t - 90) * radianConverter)) / (Math.Cos((t - 90) * radianConverter))); 
                if (x > 0 && x < originalImageWidth)
                {
                    function.AtLinePoint(x, y);
                }
            }
        }
    }
}
