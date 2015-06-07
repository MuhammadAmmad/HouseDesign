using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Camera: WorldObject
    {
        public Camera()
        {

        }

        protected override void DrawObject(OpenGL gl)
        {

        }

        public override Point3d Forward
        {
            get
            {
                return new Point3d(0.0f, 0.0f, 1.0f).RotateX(-Rotate.X).RotateY(Rotate.Y);

            }
        }

        public override Point3d Top
        {
            get
            {
                return new Point3d(0.0f, 1.0f, 0.0f).RotateX(-Rotate.X).RotateY(Rotate.Y);

            }
        }

        public override Point3d Right
        {
            get
            {
                return new Point3d(1.0f, 0.0f, 0.0f).RotateX(-Rotate.X).RotateY(Rotate.Y);

            }
        }

        public void Perspective(OpenGL gl)
        {
            gl.Rotate(-Rotate.Z, 0, 0, 1);
            gl.Rotate(-Rotate.X, 1, 0, 0);
            gl.Rotate(-180 - Rotate.Y, 0, 1, 0);

            gl.Translate(-Translate.X, -Translate.Y, -Translate.Z);
        }

        public void RotateAroundPoint(Point3d p, float angle)
        {
            float oldX = Translate.X;
            float oldZ = Translate.Z;
            oldX = oldX - p.X;
            oldZ = oldZ - p.Z;
            Translate = new Point3d(Convert.ToSingle(oldX * Math.Cos(angle) - oldZ * Math.Sin(angle))+p.X, Translate.Y, 
                Convert.ToSingle(oldX * Math.Sin(angle) + oldZ * Math.Cos(angle))+p.Z);

            Point3d forward = new Point3d(p.X - Translate.X, 0, p.Z - Translate.Z);
            Point3d oz = new Point3d(0, 0, 1);
            float yangle = Point3d.GetAngleBetween2Vectors(forward, oz);
            if(forward.X < 0 )
            {
                yangle = Convert.ToSingle(Math.PI*2-yangle);
            }
            Rotate = new Point3d(Rotate.X,ConvertToDegrees(yangle),Rotate.Z);
        }

        public float ConvertToDegrees(float radians)
        {
            return Convert.ToSingle(180 * radians / Math.PI);
        }
    }
}
