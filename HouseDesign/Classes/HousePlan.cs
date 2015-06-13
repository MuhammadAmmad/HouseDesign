using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace HouseDesign.Classes
{
    public class HousePlan
    {
        public String Name { get; set; }
        private List<Wall> walls;

        public HousePlan(String name)
        {
            this.Name = name;
            walls = new List<Wall>();
        }

        private void InitializeWalls()
        {
            String fullPath = @"D:\Licenta\HouseDesign\HouseDesign\HousePlans"+"\\"+Name+".hpl";
            try
            {
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        String[] points = line.Split(' ');
                        String[] point1 = points[0].Split(',');
                        Point3d p1 = new Point3d(Convert.ToSingle(point1[0]), Convert.ToSingle(point1[1]), Convert.ToSingle(point1[2]));
                        String[] point2 = points[1].Split(',');
                        Point3d p2 = new Point3d(Convert.ToSingle(point2[0]), Convert.ToSingle(point2[1]), Convert.ToSingle(point2[2]));
                        String[] point3 = points[2].Split(',');
                        Point3d p3 = new Point3d(Convert.ToSingle(point3[0]), Convert.ToSingle(point3[1]), Convert.ToSingle(point3[2]));
                        String[] point4 = points[3].Split(',');
                        Point3d p4 = new Point3d(Convert.ToSingle(point4[0]), Convert.ToSingle(point4[1]), Convert.ToSingle(point4[2]));
                        Wall wall = new Wall(p1, p2, p3, p4);
                        walls.Add(wall);
                        
                    }

                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show("The file could not be read! "+ex.Message);
            }
        }

        public List<Wall> GetWalls()
        {
            if(this.walls.Count==0)
            {
                InitializeWalls();
            }
            return this.walls;
        }

        public void AddWall(Wall wall)
        {
            this.walls.Add(wall);
        }
    }
}
