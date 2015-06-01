using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Scene
    {
        public Camera MainCamera { get; set; }
        private List<WorldObject> walls;
        private List<WorldObject> houseObjects;

        public Scene()
        {
            this.MainCamera = new Camera();
            this.walls = new List<WorldObject>();
            this.houseObjects = new List<WorldObject>();
        }

        public void AddHouseObject(WorldObject houseObject)
        {
            this.houseObjects.Add(houseObject);
        }

        public void AddWall(WorldObject wall)
        {
            this.walls.Add(wall);
        }

        public void Render(OpenGL gl)
        {
            MainCamera.Perspective(gl);

            for(int i=0;i<walls.Count;i++)
            {
                walls[i].Draw(gl);
            }

            for (int i = 0; i < houseObjects.Count;i++)
            {
                houseObjects[i].Draw(gl);
            }

           // mainCamera.SetRotate0
        }

        public bool IsEmpty()
        {
            return this.houseObjects.Count + this.walls.Count==0;
        }

        public void ClearHouseObjects()
        {
            houseObjects.Clear();
        }

        public void ClearWalls()
        {
            walls.Clear();
        }

        public WorldObject GetCollisionObject(Point3d point, Point3d direction)
        {
            for(int i=0;i<houseObjects.Count;i++)
            {
                if(houseObjects[i].CheckCollision(point, direction, false))
                {
                    return houseObjects[i];
                }
            }

            return null;
        }

        public void DeleteHouseObject(WorldObject sceneObject)
        {
            houseObjects.Remove(sceneObject);
        }
    }
}
