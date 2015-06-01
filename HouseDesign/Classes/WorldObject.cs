﻿using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HouseDesign.Classes
{
    [Serializable]
    public class WorldObject
    {
        public delegate void Point3DEventHandler(object sender, EventArgs e);        

        public event Point3DEventHandler Translating;
        public event Point3DEventHandler Scaling;
        public event Point3DEventHandler Rotating;
        protected List<Point3d> vertices;
        protected List<List<Triangle>> triangles;
        protected List<UV> uvs;
        protected List<String> textures;

        private Point3d translate;
        private Point3d scale;
        private Point3d rotate;

        private float height;
        private float width;
        private float length;
        public Point3d Translate 
        { 
            get
            {
                return translate;
            }
            set
            {
                translate = value;
                if(Translating!=null)
                {
                    Translating(this, new EventArgs());
                }                
            }
        }
        public Point3d Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                if(Scaling!=null)
                {
                    Scaling(this, new EventArgs());
                }               
            }
        }

        public Point3d Rotate{ 
            get 
            {
                return rotate;
            } 
            set 
            {
                rotate = value;
                if(Rotating!=null)
                {
                    Rotating(this, new EventArgs());
                }
            } 
        }
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        public float Height { 
            get 
            {
                return height;
            } 
            set 
            {
                height = value;
            } 
        }
        public float Length { 
            get 
            {
                return length;
            } 
            set
            {
                length = value;
            }
        }

        private OpenGL currentGL;

        private BoundingBox boundingBox;

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
            InitializeBoundingBoxAndDimensions();
        }

        uint[] tex = null;

        public void InitializeBoundingBoxAndDimensions()
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

            boundingBox = new BoundingBox(minX, maxX, minY, maxY, minZ, maxZ, this);
            height = maxY - minY;
            length = maxZ - minZ;
            width = maxX - minX;
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

                //if(boundingBox!=null)
                //{
                //    Point3d[] vertices=boundingBox.GetPoints();
                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[0].X, vertices[0].Y, vertices[0].Z);
                //    gl.TexCoord(1, 0);
                //    gl.Vertex(vertices[1].X, vertices[1].Y, vertices[1].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[5].X, vertices[5].Y, vertices[5].Z);                    
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[5].X, vertices[5].Y, vertices[5].Z);                   
                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[0].X, vertices[0].Y, vertices[0].Z);
                //    gl.TexCoord(0, 1);
                //    gl.Vertex(vertices[4].X, vertices[4].Y, vertices[4].Z);

                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[1].X, vertices[1].Y, vertices[1].Z);
                //    gl.TexCoord(1, 0);
                //    gl.Vertex(vertices[2].X, vertices[2].Y, vertices[2].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[6].X, vertices[6].Y, vertices[6].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[6].X, vertices[6].Y, vertices[6].Z);
                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[1].X, vertices[1].Y, vertices[1].Z);
                //    gl.TexCoord(0, 1);
                //    gl.Vertex(vertices[5].X, vertices[5].Y, vertices[5].Z);

                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[2].X, vertices[2].Y, vertices[2].Z);
                //    gl.TexCoord(1, 0);
                //    gl.Vertex(vertices[3].X, vertices[3].Y, vertices[3].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[7].X, vertices[7].Y, vertices[7].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[7].X, vertices[7].Y, vertices[7].Z);
                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[2].X, vertices[2].Y, vertices[2].Z);
                //    gl.TexCoord(0, 1);
                //    gl.Vertex(vertices[6].X, vertices[6].Y, vertices[6].Z);

                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[3].X, vertices[3].Y, vertices[3].Z);
                //    gl.TexCoord(1, 0);
                //    gl.Vertex(vertices[0].X, vertices[0].Y, vertices[0].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[4].X, vertices[4].Y, vertices[4].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[4].X, vertices[4].Y, vertices[4].Z);
                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[3].X, vertices[3].Y, vertices[3].Z);
                //    gl.TexCoord(0, 1);
                //    gl.Vertex(vertices[7].X, vertices[7].Y, vertices[7].Z);

                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[4].X, vertices[4].Y, vertices[4].Z);
                //    gl.TexCoord(1, 0);
                //    gl.Vertex(vertices[5].X, vertices[5].Y, vertices[5].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[6].X, vertices[6].Y, vertices[6].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[6].X, vertices[6].Y, vertices[6].Z);
                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[4].X, vertices[4].Y, vertices[4].Z);
                //    gl.TexCoord(0, 1);
                //    gl.Vertex(vertices[7].X, vertices[7].Y, vertices[7].Z);

                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[0].X, vertices[0].Y, vertices[0].Z);
                //    gl.TexCoord(1, 0);
                //    gl.Vertex(vertices[1].X, vertices[1].Y, vertices[1].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[2].X, vertices[2].Y, vertices[2].Z);
                //    gl.TexCoord(1, 1);
                //    gl.Vertex(vertices[2].X, vertices[2].Y, vertices[2].Z);
                //    gl.TexCoord(0, 0);
                //    gl.Vertex(vertices[0].X, vertices[0].Y, vertices[0].Z);
                //    gl.TexCoord(0, 1);
                //    gl.Vertex(vertices[3].X, vertices[3].Y, vertices[3].Z);
                //}
                


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

        public bool CheckCollision(Point3d point, Point3d direction, bool isObjectCollision)
        {
            if(direction.X==0)
            {
                direction.X += 0.0000000001f;
            }
            if(direction.Y==0)
            {
                direction.Y += 0.0000000001f;
            }
            Point3d[] vertices = boundingBox.ActualPoints;
            Point3d result;
            if (CheckParticularCollision(point, direction, vertices[0], vertices[4] - vertices[0], vertices[1] - vertices[0],
                out result, isObjectCollision))
            {
                return true;
            }
            if (CheckParticularCollision(point, direction, vertices[1], vertices[5] - vertices[1], vertices[2] - vertices[1], 
                out result, isObjectCollision))
            {
                return true;
            }
            if (CheckParticularCollision(point, direction, vertices[2], vertices[6] - vertices[2], vertices[3] - vertices[2],
                out result, isObjectCollision))
            {
                return true;
            }
            if (CheckParticularCollision(point, direction, vertices[3], vertices[7] - vertices[3], vertices[0] - vertices[3],
                out result, isObjectCollision))
            {
                return true;
            }
            if (CheckParticularCollision(point, direction, vertices[7], vertices[4] - vertices[7], vertices[6] - vertices[7],
                out result, isObjectCollision))
            {
                return true;
            }
            if (CheckParticularCollision(point, direction, vertices[3], vertices[0] - vertices[3], vertices[2] - vertices[3],
                out result, isObjectCollision))
            {
                return true;
            }


            //MessageBox.Show("TYYYYYYYYYYYYYYYYYYYYyy");
            return false;
        }

        private bool CheckParticularCollision(Point3d p1, Point3d v1, Point3d p2, Point3d v2, Point3d v3, out Point3d result, bool isObjectCollision)
        {
            float[,] system=new float[3,4];
            system[0, 0] = v1.X;
            system[0, 1] = -v2.X;
            system[0, 2] = -v3.X;
            system[0, 3] = p2.X - p1.X;

            system[1, 0] = v1.Y;
            system[1, 1] = -v2.Y;
            system[1, 2] = -v3.Y;
            system[1, 3] = p2.Y - p1.Y;

            system[2, 0] = v1.Z;
            system[2, 1] = -v2.Z;
            system[2, 2] = -v3.Z;
            system[2, 3] = p2.Z - p1.Z;

            float aLID = system[0, 0] * system[2, 3] - system[2, 0] * system[0, 3];
            float aFEB = system[0, 0] * system[1, 1] - system[1, 0] * system[0, 1];
            float aHED = system[0, 0] * system[1, 3] - system[1, 0] * system[0, 3];
            float aJIB = system[0, 0] * system[2, 1] - system[2, 0] * system[0, 1];
            float aKIC = system[0, 0] * system[2, 2] - system[2, 0] * system[0, 2];
            float aGEC = system[0, 0] * system[1, 2] - system[1, 0] * system[0, 2];

            float t3 = (aLID * aFEB - aHED * aJIB) / (aKIC * aFEB - aGEC * aJIB);
            float t2 = (aHED - t3 * aGEC) / aFEB;
            float t1=(system[0,3]- system[0,2]*t3- system[0,1]*t2)/system[0,0];
            //MessageBox.Show("t2 " + t2);
            //MessageBox.Show("t3 " + t3);

            if (t2 >= 0 && t2 <= 1 && t3 >= 0 && t3 <= 1)
            {
                if(isObjectCollision)
                {
                    if(t1<0 || t1>1)
                    {
                        result = new Point3d(0, 0, 0);
                        return false;
                    }
                }
                result = p2 + v2 * t2 + v3 * t3;
                return true;
            }
            result = new Point3d(0, 0, 0);
            return false;
        }

        public bool CheckObjectCollision(WorldObject obj)
        {
            Point3d[] vertices = obj.boundingBox.ActualPoints;
            if(CheckCollision(vertices[0], vertices[1]-vertices[0], true))
            {
                return true;
            }
            if(CheckCollision(vertices[1], vertices[2]-vertices[1], true))
            {
                return true;
            }
            if(CheckCollision(vertices[2], vertices[3]-vertices[2], true))
            {
                return true;
            }
            if(CheckCollision(vertices[3], vertices[0]-vertices[3], true))
            {
                return true;
            }
            if(CheckCollision(vertices[4], vertices[5]-vertices[4], true))
            {
                return true;
            }
            if(CheckCollision(vertices[5], vertices[6]-vertices[5], true))
            {
                return true;
            }
            if(CheckCollision(vertices[6], vertices[7]-vertices[6], true))
            {
                return true;
            }
            if(CheckCollision(vertices[7], vertices[4]-vertices[7], true))
            {
                return true;
            }
            if(CheckCollision(vertices[4], vertices[0]-vertices[4], true))
            {
                return true;
            }
            if(CheckCollision(vertices[5], vertices[1]-vertices[5], true))
            {
                return true;
            }
            if(CheckCollision(vertices[6], vertices[2]-vertices[6], true))
            {
                return true;
            }
            if(CheckCollision(vertices[7], vertices[3]-vertices[7], true))
            {
                return true;
            }

            return false;
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
