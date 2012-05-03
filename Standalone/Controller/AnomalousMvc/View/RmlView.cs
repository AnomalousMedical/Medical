using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    class RmlView : View
    {
        public RmlView(String name)
            :base(name)
        {
            RmlFile = name + ".rml";
        }

        [Editable]
        public String RmlFile { get; set; }

        protected RmlView(LoadInfo info)
            :base (info)
        {

        }
    }
}
