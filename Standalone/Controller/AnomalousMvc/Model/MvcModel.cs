using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public abstract class MvcModel : SaveableEditableItem
    {
        public MvcModel(String name)
            :base(name)
        {

        }

        public virtual void reset()
        {

        }

        protected MvcModel(LoadInfo info)
            : base(info)
        {

        }
    }
}
