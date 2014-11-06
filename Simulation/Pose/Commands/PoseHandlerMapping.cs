using Engine;
using Engine.Editing;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    class PoseHandlerMapping : Saveable
    {
        public PoseHandlerMapping()
        {
            PoseHandlerName = "PoseHandler";
        }

        [Editable]
        public String SimObjectName { get; set; }

        [Editable]
        public String PoseHandlerName { get; set; }

        protected PoseHandlerMapping(LoadInfo info)
        {
            SimObjectName = info.GetString("SimObjectName");
            PoseHandlerName = info.GetString("PoseHandlerName");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("SimObjectName", SimObjectName);
            info.AddValue("PoseHandlerName", PoseHandlerName);
        }
    }
}
