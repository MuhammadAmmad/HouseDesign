using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public class Camera: WorldObject
    {
        public Camera()
        {

        }

        protected override void DrawObject(OpenGL gl)
        {

        }

        public void Perspective(OpenGL gl)
        {
            gl.Rotate(-Rotate.Z, 0, 0, 1);
            gl.Rotate(-Rotate.X, 1, 0, 0);
            gl.Rotate(-180 - Rotate.Y, 0, 1, 0);

            gl.Translate(-Translate.X, -Translate.Y, -Translate.Z);
        }
    }
}
