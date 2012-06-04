using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    class StoredBorderContainerDock
    {
        public StoredBorderContainerDock(IntSize2 size)
        {
            this.Size = size;
        }

        public IntSize2 Size { get; set; }
    }
}
