using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class RmlView : View
    {
        public RmlView(String name)
            :base(name)
        {

        }

        public override string Type
        {
            get
            {
                return "Rml View";
            }
        }

        protected RmlView(LoadInfo info)
            :base (info)
        {

        }
    }
}
