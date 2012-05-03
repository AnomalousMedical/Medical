using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public abstract class View : SaveableEditableItem
    {
        public View(String name)
            :base(name)
        {
            
        }

        protected View(LoadInfo info)
            :base (info)
        {

        }
    }
}
