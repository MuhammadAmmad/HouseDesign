using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class WallObject: WorldObject
    {
        private float height;

        public WallObject(Wall wall, float height): base()
        {
            //Scale = new Point3d(0.05f, 0.05f, 0.05f);
            Point3d p1 = wall.P1 * 0.05f;
            Point3d p2 = wall.P2 * 0.05f;
            Point3d p3 = wall.P3 * 0.05f;
            Point3d p4 = wall.P4 * 0.05f;

            this.height = height * 0.05f;

            vertices.Add(p1);
            vertices.Add(p2);
            vertices.Add(p3);
            vertices.Add(p4);
            vertices.Add(new Point3d(p1.X, this.height, p1.Z));
            vertices.Add(new Point3d(p2.X, this.height, p2.Z));
            vertices.Add(new Point3d(p3.X, this.height, p3.Z));
            vertices.Add(new Point3d(p4.X, this.height, p4.Z));

            triangles.Add(new List<Triangle>());

            triangles[0].Add(new Triangle(0, 1, 2));
            triangles[0].Add(new Triangle(0, 3, 2));
            triangles[0].Add(new Triangle(4, 5, 6));
            triangles[0].Add(new Triangle(4, 7, 6));
            triangles[0].Add(new Triangle(1, 2, 6));
            triangles[0].Add(new Triangle(1, 5, 6));
            triangles[0].Add(new Triangle(0, 3, 7));
            triangles[0].Add(new Triangle(0, 4, 7));
            triangles[0].Add(new Triangle(0, 1, 5));
            triangles[0].Add(new Triangle(0, 4, 5));
            triangles[0].Add(new Triangle(3, 2, 6));
            triangles[0].Add(new Triangle(3, 7, 6));

            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(0, 0));
            uvs.Add(new UV(0, 1));
            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(1, 1));
            uvs.Add(new UV(0, 1));
            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(0, 0));
            uvs.Add(new UV(0, 1));
            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(1, 1));
            uvs.Add(new UV(0, 1));

            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(0, 0));
            uvs.Add(new UV(0, 1));
            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(1, 1));
            uvs.Add(new UV(0, 1));
            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(0, 0));
            uvs.Add(new UV(0, 1));
            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(1, 1));
            uvs.Add(new UV(0, 1));

            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(0, 0));
            uvs.Add(new UV(0, 1));
            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(1, 1));
            uvs.Add(new UV(0, 1));
            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(0, 0));
            uvs.Add(new UV(0, 1));
            uvs.Add(new UV(1, 0));
            uvs.Add(new UV(1, 1));
            uvs.Add(new UV(0, 1));

            textures.Add(@"D:\Licenta\HouseDesign\HouseDesign\Assets\interiorWallTexture.jpg");
            InitializeBoundingBoxAndDimensions();
        }
    }
}
