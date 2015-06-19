using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HouseDesign.ImageProcessing
{
    public class LineSegments
    {
        public DirectionLine Line { get; private set; }
        public Image<Gray, byte> ImageContext { get; private set; }

        public double ErrorMargin { get; private set; }

        private List<double> segmentPoints;

        public LineSegments(Image<Gray,byte> imageContext, DirectionLine line, double errorMargin,double minSegmentSize)
        {
            this.ImageContext = imageContext;
            this.Line = line;
            this.ErrorMargin = errorMargin;

            segmentPoints = CalculateSegmentPoints();
            EliminateErrorPoints();
            EliminateSmallSegments(minSegmentSize);
        }

        private void EliminateErrorPoints()
        {
            for (int i = 1; i < segmentPoints.Count - 1; i++)
            {
                if (Math.Abs(segmentPoints[i] - segmentPoints[i + 1]) < ErrorMargin)
                {
                    segmentPoints.RemoveAt(i);
                    segmentPoints.RemoveAt(i);
                    --i;
                }
            }
        }

        private void EliminateSmallSegments(double minSegmentSize)
        {
            for (int i = 0; i < segmentPoints.Count - 1; i+=2)
            {
                if(Math.Abs(segmentPoints[i] - segmentPoints[i+1]) < minSegmentSize)
                {
                    segmentPoints.RemoveAt(i);
                    segmentPoints.RemoveAt(i);
                    i -= 2;
                }
            }
        }

        public List<DirectionLineSegment> GetSegments()
        {
            List<DirectionLineSegment> segments = new List<DirectionLineSegment>();

            for (int i = 1; i < segmentPoints.Count; i +=2 )
            {
                segments.Add(new DirectionLineSegment(Line,segmentPoints[i],segmentPoints[i-1]));
            }

            return segments;
        }

        private List<double> CalculateSegmentPoints()
        {
            List<double> positions = new List<double>();

            int width = ImageContext.Width;
            int height = ImageContext.Height;

            double angle = DirectionLine.GetAngleBetweenLines(Line, new DirectionLine(new Vector2d(0, 0), new Vector2d(1, 0)));

            byte colorSearch = 0;

            if(angle > Math.PI/4.0 && angle < Math.PI*3.0/4.0)
            {
                for (int y = 0; y < height; y++)
                {
                    int x = (int)Line.GetPointByY(y).X;

                    if (x >= 0 && x < width)
                    {
                        if (ImageContext.Data[y, x, 0] == colorSearch)
                        {
                            positions.Add(Line.GetPositionOfY(y));
                            colorSearch = (byte)(255 - colorSearch);
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    int y = (int)Line.GetPointByX(x).Y;

                    if (y >= 0 && y < height)
                    {
                        if (ImageContext.Data[y, x, 0] == colorSearch)
                        {
                            positions.Add(Line.GetPositionOfX(x));
                            colorSearch = (byte)(255 - colorSearch);
                        }
                    }
                }
            }

            return positions;
        }
    }
}
