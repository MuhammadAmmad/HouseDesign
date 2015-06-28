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

        public void AddWall(WorldObject wall)
        {
            this.walls.Add(wall);
        }
        public void AddHouseObject(WorldObject houseObject)
        {
            //houseObject.Rotate = new Point3d(0, 360, 0);
            this.houseObjects.Add(houseObject);
        }
        public void ClearHouseObjects()
        {
            houseObjects.Clear();
        }
        public void ClearWalls()
        {
            walls.Clear();
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
        }

        public bool IsEmpty()
        {
            return this.houseObjects.Count + this.walls.Count==0;
        }
        public WorldObject GetCollisionObject(Point3d point, Point3d direction)
        {
            for(int i=0;i<houseObjects.Count;i++)
            {
                if(houseObjects[i].CheckCollision(point, direction, false).Count>0)
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

        public bool CheckCurrentObjectCollisions(WorldObject currentObject, Point3d d, out float td)
        {
            bool collision = false;
            td = 1;

            for(int i=0;i<houseObjects.Count;i++)
            {
                if(houseObjects[i]!=currentObject)
                {
                    float currentTD;
                    if(currentObject.CheckObjectCollision(houseObjects[i], d, out currentTD))
                    {
                        if(currentTD < td)
                            td = currentTD;
                        collision = true;
                    }
                }
            }

            for (int i = 0; i < walls.Count;i++ )
            {
                float currentTD;
                if (currentObject.CheckObjectCollision(walls[i], d, out currentTD))
                {
                    if (currentTD < td)
                        td = currentTD;
                    collision = true;
                }
            }

            return collision;
        }

        public void ConvertHouseObjectPrices(Currency lastCurrency, Currency currentCurrency)
        {
            for (int i = 0; i < houseObjects.Count; i++)
            {
                houseObjects[i].Price = CurrencyHelper.FromCurrencyToCurrency(lastCurrency, houseObjects[i].Price, currentCurrency);
            }
        }

        public List<WorldObject> GetSortedHouseObjects()
        {
            houseObjects.Sort();

            return houseObjects;
        }
    }
}
