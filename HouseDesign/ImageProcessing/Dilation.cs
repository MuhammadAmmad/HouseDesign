using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.ImageProcessing
{
    public static class Dilation
    {
        public static Image<Gray, byte> GetDilation(Image<Gray, byte> image, int maskLength)
        {
            int height = image.Height;
            int width = image.Width;

            Image<Gray, byte> result = new Image<Gray, byte>(width, height);

            for (int i = maskLength / 2; i < height - maskLength / 2; i++)
            {
                for (int j = maskLength / 2; j < width - maskLength / 2; j++)
                {
                    if (image.Data[i, j, 0] == 0)
                    {
                        if (CheckPixel(image, i, j, maskLength / 2, BinaryColor.White) == true)
                        {
                            result.Data[i, j, 0] = 255;
                        }
                        else
                        {
                            result.Data[i, j, 0] = 0;
                        }
                    }
                    else
                    {
                        result.Data[i, j, 0] = 255;
                    }
                }
            }

            return result;
        }

        private static bool CheckPixel(Image<Gray, byte> image, int i, int j, int length, BinaryColor color)
        {
            int value;
            if (color == BinaryColor.White)
            {
                value = 255;
            }
            else
            {
                value = 0;
            }
            for (int k = i - length; k <= i + length; k++)
            {
                for (int p = j - length; p <= j + length; p++)
                {
                    if (image.Data[k, p, 0] == value)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        public enum BinaryColor
        {
            White,
            Black,
        };
    }
}
