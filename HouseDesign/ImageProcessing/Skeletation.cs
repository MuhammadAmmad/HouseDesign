using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.ImageProcessing
{
    public static class Skeletation
    {
        private static List<Point2d> points;
        private static int[,,] matrixes=new int[8,3,3]{ 
                                                { {0,0,-1},
                                                  {0,1,1},
                                                  {-1,1,-1}},

                                                { {1,-1,0},
                                                  {1,1,0},
                                                  {1,-1,0}},

                                                { {-1,1,-1},
                                                  {0,1,1},
                                                  {0,0,-1}},

                                                  { {1,1,1},
                                                  {-1,1,-1},
                                                  {0,0,0}},

                                                  { {0,0,0},
                                                  {-1,1,-1},
                                                  {1,1,1}},

                                                  { {-1,1,-1},
                                                  {1,1,0},
                                                  {-1,0,0}},

                                                  { {0,-1,1},
                                                  {0,1,1},
                                                  {0,-1,1}},

                                                  { {-1,0,0},
                                                  {1,1,0},
                                                  {-1,1,-1}},
        
                                            };

        public static Image<Gray, byte> GetProcessedImage(Image<Gray, byte> image, byte value)
        {
            int width=image.Width;
            int height=image.Height;

            byte reverseValue = (byte)(255 - value);
            Image<Gray, byte> result = new Image<Gray, byte>(width, height);
            points = new List<Point2d>();

            for (int i = 1; i < height-1;i++ )
            {
                for(int j=1; j<width-1;j++)
                {
                    if(image.Data[i, j, 0]==value)
                    {
                        points.Add(new Point2d(j, i));
                    }
                }
            }

            bool testing = true;

            List<Point2d> removedPoints = new List<Point2d>();

            while (testing)
            {
                testing = false;

                for (int i = 0; i < 8; ++i)
                {
                    for (int j = 0; j < points.Count; ++j)
                    {
                        if(TestMask(image, value, i, points[j]))
                        {
                            removedPoints.Add(points[j]);
                            points.RemoveAt(j);
                            j--;
                            testing = true;
                        }
                    }

                    for (int j = 0; j < removedPoints.Count; ++j)
                    {
                        image.Data[removedPoints[j].Y, removedPoints[j].X, 0] = reverseValue;
                    }
                    removedPoints.Clear();
                }
            }

            for (int i = 0; i < points.Count;++i )
            {
                result.Data[points[i].Y, points[i].X, 0] = 255;
            }

            return result;
        }

        private static bool TestMask(Image<Gray, byte> image, byte value, int i, Point2d testPoint)
        {
            for (int k = 0; k < 3; ++k)
            {
                for (int l = 0; l < 3; ++l)
                {
                    if (matrixes[i, k, l] != -1)
                    {
                        if (matrixes[i, k, l] == 1)
                        {
                            if (image.Data[testPoint.Y - 1 + k, testPoint.X - 1 + l, 0] != value)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (image.Data[testPoint.Y - 1 + k, testPoint.X - 1 + l, 0] == value)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
