using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HouseDesign.ImageProcessing
{
    public class LineSegmentator : ILineCrossable
    {
        Point beginPoint;
        Point endPoint;
        int lastX;
        int lastY;
        bool isFoundFirst;
        List<Segment> segments;
        int thresold = 20;
        Image<Gray, byte> image;

        public LineSegmentator(Image<Gray,byte> image)
        {
            this.image = image;

            beginPoint = new Point();
            endPoint = new Point();
            lastX = 0;
            lastY = 0;
            isFoundFirst = false;
            segments = new List<Segment>();
        }

        public void AtLinePoint(int x, int y)
        {
            if (isFoundFirst == false)
            {
                if (image.Data[y, x, 0] == 0)
                {
                    beginPoint = new Point(x, y);
                    isFoundFirst = true;

                }
            }
            else
            {
                if (image.Data[y, x, 0] == 255)
                {
                    endPoint = new Point(lastX, lastY);
                    isFoundFirst = false;
                    if (Math.Sqrt((endPoint.Y - beginPoint.Y) * (endPoint.Y - beginPoint.Y) + (endPoint.X - beginPoint.X) * (endPoint.X - beginPoint.X)) > thresold)
                    {
                        segments.Add(new Segment(beginPoint, endPoint));
                    }

                }
            }

            lastY = y;
            lastX = x;            
        }

        public List<Segment> GetSegments()
        {
            return segments;
        }

        public void SetLine(int r, int t)
        {
        }
    }
}
