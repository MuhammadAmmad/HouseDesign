using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISIP_FrameworkGUI.Classes
{
    public interface ILineCrossable
    {
        void AtLinePoint(int x, int y);
        void SetLine(int r, int t);
    }
}
