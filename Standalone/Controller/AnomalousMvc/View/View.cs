using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.Controller.AnomalousMvc
{
    public abstract class View : SaveableEditableItem
    {
        public View(String name)
            :base(name)
        {
            ViewLocation = ViewLocations.Left;
        }

        [Editable]
        public ViewLocations ViewLocation { get; set; }

        protected View(LoadInfo info)
            :base (info)
        {

        }
    }
}
