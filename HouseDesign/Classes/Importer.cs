using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public class Importer
    {
        private float x, y, z;
        operation nextSet;
        List<Point3d> vertices;
        List<Triangle> triangles;
        private int vertex1, vertex2, vertex3;
        operationTriangle nextVertex;
        List<UV> uvs;
        operationUV nextUV;
        float u, v;


        public Importer()
        {
            vertices = new List<Point3d>();
            triangles = new List<Triangle>();
            uvs = new List<UV>();
        }

        delegate void operation(float value);
        delegate void operationTriangle(int value);
        delegate void operationUV(float value);
        private void SetX(float value)
        {
            x = value;
            nextSet = SetY;
        }

        private void SetY(float value)
        {
            y = value;
            nextSet = SetZ;
        }

        private void SetZ(float value)
        {
            z = value;
            vertices.Add(new Point3d(x, y, z));
            nextSet = SetX;

        }

        private void SetVertex1(int value)
        {
            vertex1 = value;
            nextVertex = SetVertex2;
        }

        private void SetVertex2(int value)
        {
            vertex2 = value;
            nextVertex = SetVertex3;
        }

        private void SetVertex3(int value)
        {
            vertex3 = value * -1 - 1;
            triangles.Add(new Triangle(vertex1, vertex2, vertex3));
            nextVertex = SetVertex1;
        }

        private void SetUV1(float value)
        {
            u = value;
            nextUV = SetUV2;

        }
        private void SetUV2(float value)
        {
            v = 1-value;
            uvs.Add(new UV(u, v));
            nextUV = SetUV1;

        }

        public WorldObject Import(string fileName)
        {
            //try
            //{
            using (StreamReader sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    if (line.Contains("Vertices:"))
                    {
                        nextSet = SetX;
                        line = sr.ReadLine();
                        line = line.Split(':')[1];
                        do
                        {

                            String[] linePoints = line.Split(',');
                            for (int i = 0; i < linePoints.Length; i++)
                            {
                                if (linePoints[i].Length > 0)
                                {
                                    nextSet(float.Parse(linePoints[i].Replace('.', ',')));
                                }

                            }

                        }
                        while ((line = sr.ReadLine()).Contains("}") == false);
                    }

                    if (line.Contains("PolygonVertexIndex:"))
                    {
                        nextVertex = SetVertex1;
                        line = sr.ReadLine();
                        line = line.Split(':')[1];
                        do
                        {
                            String[] lineTriangles = line.Split(',');
                            for (int i = 0; i < lineTriangles.Length; i++)
                            {
                                if (lineTriangles[i].Length > 0)
                                {
                                    nextVertex(Convert.ToInt16(lineTriangles[i]));
                                }

                            }

                        }
                        while ((line = sr.ReadLine()).Contains("}") == false);
                    }

                    if (line.Contains("LayerElementUV:"))
                    {
                        line = sr.ReadLine();
                        while(! line.Contains("UV:"))
                        {
                            line = sr.ReadLine();
                        }

                        nextUV = SetUV1;
                        line = sr.ReadLine();
                        line = line.Split(':')[1];
                        do
                        {

                            String[] lineUVs = line.Split(',');
                            for (int i = 0; i < lineUVs.Length; i++)
                            {
                                if (lineUVs[i].Length > 0)
                                {
                                    nextUV(float.Parse(lineUVs[i].Replace('.', ',')));
                                }

                            }

                        }
                        while ((line = sr.ReadLine()).Contains("}") == false);
                    break;
                    }
                   

                }

            }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("The file could not be read:");
            //    Console.WriteLine(e.Message);
            //}

            return new WorldObject(vertices, triangles, uvs);

        }
    }
}
