using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    class StoredBorderContainer
    {
        public StoredBorderContainer()
        {

        }

        public StoredBorderContainerDock Left { get; set; }

        public StoredBorderContainerDock Right { get; set; }

        public StoredBorderContainerDock Top { get; set; }

        public StoredBorderContainerDock Bottom { get; set; }

        public StoredFloatingWindows Floating { get; set; }
    }
}
