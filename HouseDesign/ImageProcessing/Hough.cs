using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.ImageProcessing
{
    public class Hough
    {
        private int[,] houghMatrix;
        private double[] sinValues;
        private double[] cosValues;
        int numberOfLines;
        int numberOfColumns;
        const double radianConverter = Math.PI / 180.0;
        int width;
        int height;
        Image<Gray, byte> image;
        
        public Hough(Image<Gray, byte> image)
        {
            width = image.Width;
            height = image.Height;
            this.image = image;
            numberOfLines = (int)Math.Sqrt(width * width + height * height);
            numberOfColumns = 270;
            InitializeTrigonometricValues();
            InitializeHoughMatrix();
            //ApplyLocalMaximum(2, 2);
        }

        private void InitializeTrigonometricValues()
        {
            sinValues = new double[numberOfColumns];
            cosValues = new double[numberOfColumns];

            for(int i=0;i<numberOfColumns;i++)
            {
                sinValues[i] = Math.Sin((i - 90) * radianConverter);
                cosValues[i] = Math.Cos((i - 90) * radianConverter);
            }
        }

        private void InitializeHoughMatrix()
        {
            houghMatrix = new int[numberOfLines, numberOfColumns];

            double startValue = 10.38;
            double step = (64.73 - startValue) / numberOfLines;
            for(int r=0;r<numberOfLines;r++)
            {
                NewProject.ProgressValue = startValue + r * step;
                for(int t=-90;t<180;t++)
                {
                    if((t<-45) || (t>45 && t<135))
                    {
                        for(int x=0;x<height;x++)
                        {
                            int y = (int)((r - x * cosValues[t + 90]) * 1.0 / sinValues[t + 90]);
                            if(y>0 && y<width)
                            {
                                if (image.Data[x, y, 0] == 0)
                                {
                                    houghMatrix[r, t + 90]++;
                                }
                            }                            
                        }
                    }
                    else
                    {
                        for(int y=0;y<width;y++)
                        {
                            int x = (int)((r - y*sinValues[t + 90])*1.0 / cosValues[t + 90]);
                            if(x>0 && x<height)
                            {
                                if (image.Data[x, y, 0] == 0)
                                {
                                    houghMatrix[r, t + 90]++;
                                }
                            }                            
                        }
                    }
                }
            }
        }

        private void ApplyLocalMaximum(int maskXDimension, int maskYDimension)
        {
            int[,] auxMatrix=new int[numberOfLines, numberOfColumns];
            for(int y=maskYDimension;y<numberOfLines-maskYDimension;y++)
            {
                for(int x=maskXDimension;x<numberOfColumns-maskXDimension;x++)
                {
                    bool isLocalMaximum=true;
                    for(int i=y-maskYDimension;i<y+maskYDimension+1 && isLocalMaximum;i++)
                    {
                        for(int j=x-maskXDimension;j<x+maskXDimension+1;j++)
                        {
                            if(houghMatrix[i, j]>houghMatrix[y, x])
                            {
                                isLocalMaximum = false;
                                break;
                            }
                        }
                    }

                    if(isLocalMaximum)
                    {
                        auxMatrix[y, x] = houghMatrix[y, x];
                    }
                }
            }

            houghMatrix = auxMatrix;
        }

        public List<DirectionLine> GetLines(int thresold)
        {
            List<DirectionLine> lines = new List<DirectionLine>();
            double default1=0, default2=100;
            double x1, x2, y1, y2;

            for (int i = 0; i < numberOfLines;i++ )
            {
                for(int j=0;j<numberOfColumns;j++)
                {
                    if(houghMatrix[i, j]>thresold)
                    {
                        if((j<45) || (j>135 && j<225))
                        {
                            y1=default1;
                            y2=default2;
                            x1 = (i - y1 * cosValues[j]) / sinValues[j];
                            x2 = (i - y2 * cosValues[j]) / sinValues[j];
                        }
                        else
                        {
                            x1 = default1;
                            x2 = default2;
                            y1 = (i - x1 * sinValues[j]) / cosValues[j];
                            y2 = (i - x2 * sinValues[j]) / cosValues[j];
                        }
                       
                        DirectionLine line = new DirectionLine(new Vector2d(x1, y1), new Vector2d(x2 - x1, y2 - y1));
                        lines.Add(line);
                    }
                }
            }
            
            return lines;
        }

        public Image<Gray, byte> GetHoughImage()
        {
            Image<Gray, byte> result = new Image<Gray, byte>(numberOfColumns, numberOfLines);
            int max=0;
            for(int i=0;i<numberOfLines; i++)
            {
                for(int j=0;j<numberOfColumns;j++)
                {
                    if(houghMatrix[i, j]>max)
                    {
                        max=houghMatrix[i, j];
                    }
                }
            }
            for(int i=0;i<numberOfLines; i++)
            {
                for(int j=0;j<numberOfColumns;j++)
                {
                    result.Data[i, j, 0] =(byte) (houghMatrix[i, j]*255.0/max);
                }
            }

            return result;
        }
    }
}
