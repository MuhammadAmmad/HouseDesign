using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public static class Texture
    { 
        public static uint LoadTexture(string filename, OpenGL gl)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            uint[] id = new uint[1];
            gl.GenTextures(1, id);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, id[0]);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, bmp_data.Width, bmp_data.Height, 0,
                OpenGL.GL_BGRA_EXT, OpenGL.GL_UNSIGNED_BYTE, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, (int)OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, (int)OpenGL.GL_LINEAR);

            return id[0];
        }
    }
}
