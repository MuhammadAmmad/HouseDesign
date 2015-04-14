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
        private List<Triangle>[] triangles;
        private List<UV> uvs;
        private List<String> textures;

        public WorldObject(List<Point3d> vertices, List<Triangle> triangles, List<UV> uvs, List<String> textures)
        {
            this.vertices = new List<Point3d>();
            this.vertices.AddRange(vertices);

            this.triangles = new List<Triangle>[textures.Count];

            for (int i = 0; i < textures.Count;i++)
            {
                this.triangles[i] = new List<Triangle>();
            }

            for (int i = 0; i < triangles.Count;i++ )
            {
                int index = triangles[i].TextureIndex;
                this.triangles[index].Add(triangles[i]);
            }


            this.uvs = new List<UV>();
            this.uvs.AddRange(uvs);

            this.textures = new List<String>();
            this.textures.AddRange(textures);

        }

        uint[] tex;

        public void Draw(OpenGL gl)
        {
            for (int j = 0; j < textures.Count;j++ )
            {
                //if (tex == null)
                //{
                    tex = new uint[1];
                    tex[0] = Texture.LoadTexture(textures[j], gl);
                //}

                gl.Enable(OpenGL.GL_TEXTURE_2D);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, tex[0]);



                gl.Begin(OpenGL.GL_TRIANGLES);

                for (int i = 0; i < triangles[j].Count; i++)
                {
                    gl.Color(1f, 1f, 1f);
                    gl.TexCoord(uvs[3 * i].U, uvs[3 * i].V);
                    gl.Vertex(vertices[triangles[j][i].vertex1].X, vertices[triangles[j][i].vertex1].Y, vertices[triangles[j][i].vertex1].Z);
                    gl.TexCoord(uvs[3 * i + 1].U, uvs[3 * i + 1].V);
                    gl.Vertex(vertices[triangles[j][i].vertex2].X, vertices[triangles[j][i].vertex2].Y, vertices[triangles[j][i].vertex2].Z);
                    gl.TexCoord(uvs[3 * i + 2].U, uvs[3 * i + 2].V);
                    gl.Vertex(vertices[triangles[j][i].vertex3].X, vertices[triangles[j][i].vertex3].Y, vertices[triangles[j][i].vertex3].Z);
                }
                gl.End();
            }

                
        }

        public List<String> GetTextures()
        {
            return this.textures;
        }

        public void SetTexture(int index, String newTexturePath)
        {
            this.textures[index] = newTexturePath;
        }
    }
}
