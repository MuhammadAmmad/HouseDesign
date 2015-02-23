using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public class WorldObject
    {
        private List<Point3d> vertices;
        private List<Triangle> triangles;
        private List<UV> uvs;

        public WorldObject(List<Point3d> vertices, List<Triangle> triangles, List<UV> uvs)
        {
            this.vertices = new List<Point3d>();
            this.vertices.AddRange(vertices);

            this.triangles = new List<Triangle>();
            this.triangles.AddRange(triangles);

            this.uvs = new List<UV>();
            this.uvs.AddRange(uvs);

        }

        uint[] tex;

        public void Draw(OpenGL gl)
        {

            if (tex == null)
            {
                tex = new uint[1];
                tex[0] = Texture.LoadTexture(@"D:\Licenta\HouseDesign\HouseDesign\Assets\red.jpg", gl);
            }

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, tex[0]);
            
            
            
            
            gl.Begin(OpenGL.GL_TRIANGLES);

            for (int i = 0; i < triangles.Count;i++)
            {
                gl.TexCoord(uvs[3 * i].U, uvs[3 * i].V); 
                gl.Vertex(vertices[triangles[i].vertex1].X, vertices[triangles[i].vertex1].Y, vertices[triangles[i].vertex1].Z);
                gl.TexCoord(uvs[3 * i + 1].U, uvs[3 * i + 1].V); 
                gl.Vertex(vertices[triangles[i].vertex2].X, vertices[triangles[i].vertex2].Y, vertices[triangles[i].vertex2].Z);
                gl.TexCoord(uvs[3 * i + 2].U, uvs[3 * i +2].V);
                gl.Vertex(vertices[triangles[i].vertex3].X, vertices[triangles[i].vertex3].Y, vertices[triangles[i].vertex3].Z);
            }
            gl.End();
        }
    }
}
