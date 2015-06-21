using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    interface ICollision
    {
        float GetMaxTD(Point3d d);
    }
}
