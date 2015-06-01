using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public struct UV
    {
        public float U { get; set; }
        public float V { get; set; }
        public UV(float u, float v)
            : this()
        {
            U = u;
            V = v;
        }
    }
}
