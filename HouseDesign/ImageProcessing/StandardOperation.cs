using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.ImageProcessing
{
    public static class StandardOperation
    {

        public static Image<Gray, byte> Binarize(Image<Gray, byte> image, double thresold)
        {
            int height = image.Height;
            int width = image.Width;
            Image<Gray, byte> binaryImage = new Image<Gray, byte>(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (image.Data[i, j, 0] < thresold)
                    {
                        binaryImage.Data[i, j, 0] = 0;
                    }
                    else
                    {
                        binaryImage.Data[i, j, 0] = 255;
                    }
                }
            }

            return binaryImage;
        }

        public static void Invert(Image<Gray, byte> image)
        {
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    image.Data[i, j, 0] = (byte)(255 - image.Data[i, j, 0]);
                }
            }
        }

        public static Image<Gray, byte> GetImageWithExtraPixels(Image<Gray, byte> image, BinaryColor color, int maskLength)
        {
            int width = image.Width;
            int height = image.Height;
            Image<Gray, byte> result = new Image<Gray, byte>(width + (maskLength / 2) * 2, height + (maskLength / 2) * 2);

            for (int i = 0; i < maskLength / 2; i++)
            {
                for (int j = 0; j < width + maskLength / 2; j++)
                {
                    if (color == BinaryColor.Black)
                    {
                        result.Data[i, j, 0] = 0;
                        result.Data[height + maskLength / 2 + i, j, 0] = 0;
                    }
                    else
                    {
                        result.Data[i, j, 0] = 255;
                        result.Data[height + maskLength / 2 + i, j, 0] = 255;
                    }
                }
            }

            for (int j = 0; j < maskLength / 2; j++)
            {
                for (int i = 0; i < height + maskLength / 2; i++)
                {
                    if (color == BinaryColor.Black)
                    {
                        result.Data[i, j, 0] = 0;
                        result.Data[i, width + maskLength / 2 + j, 0] = 0;
                    }
                    else
                    {
                        result.Data[i, j, 0] = 255;
                        result.Data[i, width + maskLength / 2 + j, 0] = 255;
                    }
                }
            }


            for (int i = maskLength / 2; i < height + maskLength / 2; i++)
            {
                for (int j = maskLength / 2; j < width + maskLength / 2; j++)
                {
                    result.Data[i, j, 0] = image.Data[i - maskLength / 2, j - maskLength / 2, 0];
                }
            }

            return result;
        }

       

        public static double GetDistanceBetweenPoints(System.Windows.Point p1, System.Windows.Point p2)
        {
            return Math.Sqrt((p2.Y - p1.Y) * (p2.Y - p1.Y) + (p2.X - p1.X) * (p2.X - p1.X));
        }

        public enum BinaryColor
        {
            White,
            Black,
        };
    }
}
