using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface BoneManipulator
    {
        void setPosition(float position);

        String getUIName();
    }
}
