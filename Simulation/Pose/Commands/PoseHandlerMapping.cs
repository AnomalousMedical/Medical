using Engine;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    class PoseHandlerMapping
    {
        [Editable]
        public String SimObjectName { get; set; }

        [Editable]
        public String PoseHandlerName { get; set; }
    }
}
