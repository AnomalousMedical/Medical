using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    [Flags]
    public enum SceneAnatomyControlType
    {
        Pin = 1,
        Lock = 1 << 1
    }

    public interface SceneAnatomyControl
    {
        bool Active { get; set; }

        Vector3 WorldPosition { get; }

        SceneAnatomyControlType Type { get; }
    }
}
