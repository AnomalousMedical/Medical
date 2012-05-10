using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public abstract class Model : SaveableEditableItem
    {
        public Model(String name)
            :base(name)
        {

        }

        public virtual void reset()
        {

        }

        protected Model(LoadInfo info)
            : base(info)
        {

        }
    }
}
