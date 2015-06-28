using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.ImageProcessing
{
    public static class WallDetector
    {
        private static int tolerance = 180;
        public static List<Wall2D> GetWalls(Image<Gray, byte> currentImage)
        {
            NewProject.ProgressMessage = "Processing house plan";
            //currentImage = StandardOperation.Binarize(currentImage, 127);
            currentImage = StandardOperation.GetImageWithExtraPixels(currentImage, StandardOperation.BinaryColor.Black, 3);
            NewProject.ProgressValue = 3.35;

            currentImage = Skeletation.GetProcessedImage(StandardOperation.Binarize(currentImage, 127), 0);
            NewProject.ProgressValue = 5.51;
            
            currentImage = StandardOperation.Binarize(currentImage, 127);
            StandardOperation.Invert(currentImage);
            NewProject.ProgressValue = 10.38;


            NewProject.ProgressMessage = "Detecting lines";
            Hough hough = new Hough(currentImage);
            NewProject.ProgressValue = 64.73;

            NewProject.ProgressMessage = "Generating walls";
            List<DirectionLine> lines = hough.GetLines(15);

            int width = currentImage.Width;
            int height = currentImage.Height;
            List<LineSortHelper> lineSortHelper = new List<LineSortHelper>();

            List<DirectionLine> auxLines = new List<DirectionLine>();
            List<DirectionLineSegment>[,] segmentCounts = new List<DirectionLineSegment>[height, width];
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    segmentCounts[i, j] = new List<DirectionLineSegment>();
                }
            }

            double startValue = 64.73;
            double step = (99.58 - startValue) / lines.Count;
            for (int i = 0; i < lines.Count; i++)
            {
                NewProject.ProgressValue = startValue + i * step;
                LineSegments segmentedLine = new LineSegments(currentImage, lines[i], 2, 10);

                List<DirectionLineSegment> segments = segmentedLine.GetSegments();

                for (int j = 0; j < segments.Count; ++j)
                {
                    segments[j].MarkSegmentOnMap(segmentCounts, width, height);
                }
            }
            NewProject.ProgressValue = 99.58;

            auxLines = new List<DirectionLine>();
            List<DirectionLineSegment> directionSegments = new List<DirectionLineSegment>();
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (currentImage.Data[i, j, 0] == 0)
                    {
                        if (segmentCounts[i, j].Count > 0)
                        {
                            DirectionLineSegment biggestSegment = segmentCounts[i, j][0];
                            for (int k = 1; k < segmentCounts[i, j].Count; ++k)
                            {
                                if (biggestSegment.Size < segmentCounts[i, j][k].Size)
                                {
                                    biggestSegment = segmentCounts[i, j][k];
                                }
                            }

                            if (!directionSegments.Contains(biggestSegment))
                            {
                                directionSegments.Add(biggestSegment);
                            }
                        }
                    }
                }
            }

            List<DirectionLine> actualLines = new List<DirectionLine>();

            Dictionary<DirectionLine, List<DirectionLineSegment>>[] angleSorted = new Dictionary<DirectionLine, List<DirectionLineSegment>>[tolerance + 1];

            for (int i = 0; i <= tolerance; ++i)
            {
                angleSorted[i] = new Dictionary<DirectionLine, List<DirectionLineSegment>>();
            }

            //ELIMINATE NON PARALEL LINES
            for (int i = 0; i < directionSegments.Count; i++)
            {
                LineSortHelper lineHelper = new LineSortHelper(directionSegments[i].Line);
                if (!angleSorted[lineHelper.OXAngle].Keys.Contains(directionSegments[i].Line))
                {
                    angleSorted[lineHelper.OXAngle].Add(directionSegments[i].Line, new List<DirectionLineSegment>());
                }
                angleSorted[lineHelper.OXAngle][directionSegments[i].Line].Add(directionSegments[i]);
            }

            List<Wall2D> walls = new List<Wall2D>();

            for (int i = 0; i <= tolerance; ++i)
            {
                if (angleSorted[i].Count > 1)
                {
                    List<List<DirectionLineSegment>> segmentsGroup = new List<List<DirectionLineSegment>>();
                    List<DirectionLine> linesGroup = new List<DirectionLine>();

                    foreach (KeyValuePair<DirectionLine, List<DirectionLineSegment>> pair in angleSorted[i])
                    {
                        segmentsGroup.Add(pair.Value);
                        linesGroup.Add(pair.Key);
                    }

                    for (int j = 0; j < segmentsGroup.Count - 1; ++j)
                    {
                        for (int k = j + 1; k < segmentsGroup.Count; ++k)
                        {
                            if (DirectionLine.GetDistanceBetweenLines(linesGroup[j], linesGroup[k]) < 15)
                            {
                                foreach (DirectionLineSegment first in segmentsGroup[j])
                                {
                                    foreach (DirectionLineSegment second in segmentsGroup[k])
                                    {
                                        Wall2D wall = DirectionLineSegment.GetWallBetweenTwoSegments(first, second);
                                        if (wall != null)
                                        {
                                            walls.Add(wall);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < walls.Count - 1; ++i)
            {
                for (int j = i + 1; j < walls.Count; ++j)
                {
                    Wall2D newWall = walls[i].Combine(walls[j]);
                    if (newWall != null)
                    {
                        walls[j] = newWall;
                        walls.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            return walls;
        }
        private class LineSortHelper : IComparable<LineSortHelper>
        {
            public DirectionLine lineSegment { get; private set; }

            public int OXAngle { get; private set; }

            public LineSortHelper(DirectionLine lineSegment)
            {
                this.lineSegment = lineSegment;
                DirectionLine oxLine = new DirectionLine(new Vector2d(0, 0), new Vector2d(1, 0));
                OXAngle = (int)(DirectionLine.GetAngleBetweenLines(lineSegment, oxLine) * tolerance / Math.PI);
            }

            public int CompareTo(LineSortHelper other)
            {
                return OXAngle > other.OXAngle ? 1 : -1;
            }
        }
    }
}
