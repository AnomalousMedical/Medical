using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public interface TaskPositioner
    {
        IntVector2 findGoodWindowPosition(int width, int height);
    }
}
