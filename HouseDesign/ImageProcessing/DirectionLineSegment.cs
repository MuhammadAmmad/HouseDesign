using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace HouseDesign.ImageProcessing
{
    public class DirectionLineSegment
    {
        public DirectionLine Line { get; private set; }
        private double startAlpha;
        private double endAlpha;

        public Vector2d BeginPoint { get; private set; }
        public Vector2d EndPoint {get; private set;}

        public double Size { get; private set; }

        public DirectionLineSegment(DirectionLine line, double startAlpha, double endAlpha)
        {
            this.Line = line;
            this.startAlpha = startAlpha;
            this.endAlpha = endAlpha;

            BeginPoint = line.GetPointByPosition(startAlpha);
            EndPoint = line.GetPointByPosition(endAlpha);

            this.Size = (BeginPoint - EndPoint).Magnitude;
        }

        public static Wall2D GetWallBetweenTwoSegments(DirectionLineSegment segment1, DirectionLineSegment segment2)
        {
            DirectionLine line1Test = new DirectionLine(segment1.BeginPoint, new Vector2d((int)(segment1.EndPoint.X - segment1.BeginPoint.X), (int)(segment1.EndPoint.Y - segment1.BeginPoint.Y)));
            DirectionLine line2Test = new DirectionLine(segment2.BeginPoint, new Vector2d((int)(segment2.EndPoint.X - segment2.BeginPoint.X), (int)(segment2.EndPoint.Y - segment2.BeginPoint.Y)));
            List<double> line1Alphas = new List<double>();
            line1Alphas.Add(line1Test.AlphaProjection(segment1.BeginPoint));
            line1Alphas.Add(line1Test.AlphaProjection(segment1.EndPoint));
            line1Alphas.Add(line1Test.AlphaProjection(segment2.BeginPoint));
            line1Alphas.Add(line1Test.AlphaProjection(segment2.EndPoint));

            if(line1Alphas[0] > line1Alphas[1])
            {
                double aux = line1Alphas[0];
                line1Alphas[0] = line1Alphas[1];
                line1Alphas[1] = aux;
            }
            if(line1Alphas[2] > line1Alphas[1] && line1Alphas[3] > line1Alphas[1] ||
                line1Alphas[2] < line1Alphas[0] && line1Alphas[3] < line1Alphas[0])
            {
                return null;
            }

            line1Alphas.Sort();

            Vector2d p1 = line1Test.GetPointByPosition(line1Alphas[0]);
            Vector2d p2 = line1Test.GetPointByPosition(line1Alphas[3]);
            Vector2d p3 = line2Test.GetPointProjection(p2);
            Vector2d p4 = line2Test.GetPointProjection(p1);

            return new Wall2D(new Vector2d[] { p1, p2, p3, p4 }, DirectionLine.GetMiddle(line1Test,line2Test));
        }

        public void MarkSegmentOnMap(List<DirectionLineSegment>[,] segmentsCount,int width,int height)
        {
            double angle = DirectionLine.GetAngleBetweenLines(Line, new DirectionLine(new Vector2d(0, 0), new Vector2d(1, 0)));

            if (angle > Math.PI / 4.0 && angle < Math.PI * 3.0 / 4.0)
            {
                int start = (int)(BeginPoint.Y < EndPoint.Y ? BeginPoint.Y : EndPoint.Y);
                double end = BeginPoint.Y > EndPoint.Y ? BeginPoint.Y : EndPoint.Y;

                for (int y = start; y < end; y++)
                {
                    int x = (int)Line.GetPointByY(y).X;

                    if (x >= 0 && x < width)
                    {
                        segmentsCount[y, x].Add(this);
                    }
                }
            }
            else
            {
                int start = (int)(BeginPoint.X < EndPoint.X ? BeginPoint.X : EndPoint.X);
                double end = BeginPoint.X > EndPoint.X ? BeginPoint.X : EndPoint.X;

                for (int x = start; x < end; x++)
                {
                    int y = (int)Line.GetPointByX(x).Y;

                    if (y >= 0 && y < height)
                    {
                        segmentsCount[y, x].Add(this);
                    }
                }
            }
        }
    }
}
