using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    public interface IObjectImporter
    {
        WorldObject Import(string fileName);
    }
}
