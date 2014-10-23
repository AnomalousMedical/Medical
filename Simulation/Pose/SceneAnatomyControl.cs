using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public interface SceneAnatomyControl
    {
        bool Active { get; set; }

        Vector3 WorldPosition { get; }
    }
}
