using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public class WorldObject
    {
        protected List<Point3d> vertices;
        protected List<List<Triangle>> triangles;
        protected List<UV> uvs;
        protected List<String> textures;
        public Point3d Rotate { get; set; }
        public Point3d Translate { get; set; }
        public Point3d Scale { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }


        public Point3d Forward
        {
            get
            {
                return new  Point3d(0.0f, 0.0f, 1.0f).RotateY(Rotate.Y).RotateX(Rotate.X);
            
            }
        }

        public Point3d Top
        {
            get
            {
                return new Point3d(0.0f, 1.0f, 0.0f).RotateY(Rotate.Y).RotateX(Rotate.X);

            }
        }

        public Point3d Right
        {
            get
            {
                return new Point3d(1.0f, 0.0f, 0.0f).RotateY(Rotate.Y).RotateX(Rotate.X);

            }
        }

        public WorldObject()
        {
            this.vertices = new List<Point3d>();
            this.triangles = new List<List<Triangle>>();
            this.uvs = new List<UV>();
            this.textures = new List<String>();
            Scale = new Point3d(1, 1, 1);
        }
        public WorldObject(List<Point3d> vertices, List<Triangle> triangles, List<UV> uvs, List<String> textures, float width, float height):this()
        {
            this.Width = width;
            this.Height = height;
            this.vertices.AddRange(vertices);
            

            for (int i = 0; i < textures.Count;i++)
            {
                this.triangles.Add( new List<Triangle>());
            }

            for (int i = 0; i < triangles.Count;i++ )
            {
                int index = triangles[i].TextureIndex;
                this.triangles[index].Add(triangles[i]);
            }            
            this.uvs.AddRange(uvs);
            
            this.textures.AddRange(textures);

        }

        uint[] tex;


        public void Draw(OpenGL gl)
        {
            ModifyPerspective(gl);
            DrawObject(gl);
            ModifyPerspectiveBack(gl);
        }
        protected virtual void DrawObject(OpenGL gl)
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

        protected void ModifyPerspective(OpenGL gl)
        {
            gl.Translate(Translate.X, Translate.Y, Translate.Z);

            gl.Rotate(Rotate.Y, 0, 1, 0);
            gl.Rotate(Rotate.X, 1, 0, 0);
            gl.Rotate(Rotate.Z, 0, 0, 1);

            gl.Scale(Scale.X, Scale.Y, Scale.Z);
        }

        protected void ModifyPerspectiveBack(OpenGL gl)
        {
            gl.Scale(1 / Scale.X, 1 / Scale.Y, 1 / Scale.Z);

            gl.Rotate(-Rotate.Z, 0, 0, 1);
            gl.Rotate(-Rotate.X, 1, 0, 0);
            gl.Rotate(-Rotate.Y, 0, 1, 0);

            gl.Translate(-Translate.X, -Translate.Y, -Translate.Z);
        }

    }
}
