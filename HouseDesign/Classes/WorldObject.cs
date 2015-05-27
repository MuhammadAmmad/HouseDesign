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

        private OpenGL currentGL;

        public Decimal Price { get; set; }
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
        public WorldObject(List<Point3d> vertices, List<Triangle> triangles, List<UV> uvs, List<String> textures):this()
        {
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
            InitializeBoundingBox();
        }

        uint[] tex = null;

        public void InitializeBoundingBox()
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            for(int i=0;i<vertices.Count;i++)
            {
                CheckValues(ref minX, vertices[i], Criteria.less, Axis.OX);
                CheckValues(ref maxX, vertices[i], Criteria.greater, Axis.OX);
                CheckValues(ref minY, vertices[i], Criteria.less, Axis.OY);
                CheckValues(ref maxY, vertices[i], Criteria.greater, Axis.OY);
                CheckValues(ref minZ, vertices[i], Criteria.less, Axis.OZ);
                CheckValues(ref maxZ, vertices[i], Criteria.greater, Axis.OZ);
            }
        }

        public void CheckValues(ref float currentValue, Point3d currentVertex, Criteria criteria, Axis axis)
        {
            if(axis==Axis.OX)
            {
                if(criteria==Criteria.less)
                {
                    if(currentVertex.X<currentValue)
                    {
                        currentValue = currentVertex.X;
                    }
                }
                else
                {
                    if (currentVertex.X > currentValue)
                    {
                        currentValue = currentVertex.X;
                    }
                }
            }
            else
            {
                if(axis==Axis.OY)
                {
                    if (criteria == Criteria.less)
                    {
                        if (currentVertex.Y < currentValue)
                        {
                            currentValue = currentVertex.Y;
                        }
                    }
                    else
                    {
                        if (currentVertex.Y > currentValue)
                        {
                            currentValue = currentVertex.Y;
                        }
                    }
                }
                else
                {
                    if (criteria == Criteria.less)
                    {
                        if (currentVertex.Z < currentValue)
                        {
                            currentValue = currentVertex.Z;
                        }
                    }
                    else
                    {
                        if (currentVertex.Z > currentValue)
                        {
                            currentValue = currentVertex.Z;
                        }
                    }
                }
            }
        }
        public void InitializeTextures(OpenGL gl)
        {
            if(tex!=null)
            {
                currentGL.DeleteTextures(textures.Count, tex);
            }
            currentGL = gl;
            tex = new uint[textures.Count];
            for(int i=0;i<textures.Count;i++)
            {
                tex[i] = Texture.LoadTexture(textures[i], currentGL);
            }
        }

        public void Draw(OpenGL gl)
        {
            ModifyPerspective(gl);
            DrawObject(gl);
            ModifyPerspectiveBack(gl);
        }
        protected virtual void DrawObject(OpenGL gl)
        {
            if(gl!=currentGL)
            {
                InitializeTextures(gl);
            }
            for (int j = 0; j < textures.Count;j++ )
            {
                //if (tex == null)
                //{
                    
                //}


                gl.BindTexture(OpenGL.GL_TEXTURE_2D, tex[j]);



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
            //gl.DeleteTextures((int)tex[0], tex);
                
        }

        public List<String> GetTextures()
        {
            return this.textures;
        }

        public void SetTexture(int index, String newTexturePath, OpenGL gl)
        {
            this.textures[index] = newTexturePath;
            InitializeTextures(gl);
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

        public double GetArea(Point3d a, Point3d b, Point3d c)
        {
            double area = Math.Abs((a.X*(b.Y-c.Y)+b.X*(c.Y-a.Y)+c.X*(a.Y-b.Y))/2);

            return area;
        }

        public double getTotalAreaPerTexture(int textureIndex)
        {
            double area = 0;
            for(int i=0;i<triangles[textureIndex].Count;i++)
            {
                Point3d a = vertices[triangles[textureIndex][i].vertex1];
                Point3d b = vertices[triangles[textureIndex][i].vertex2];
                Point3d c = vertices[triangles[textureIndex][i].vertex3];
                area += GetArea(a, b, c);
            }
            return area;
        }

        public enum Criteria
        {
            less,
            greater
        };

        public enum Axis
        {
            OX,
            OY,
            OZ
        };

    }
}
