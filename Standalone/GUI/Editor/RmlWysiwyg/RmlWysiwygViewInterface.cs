using Engine.Saving;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public abstract class RmlWysiwygViewBase : MyGUIView
    {
        public RmlWysiwygViewBase(String name)
            : base(name)
        {

        }

        internal abstract void _fireRequestFocus();

        public abstract IEnumerable<ElementStrategy> CustomElementStrategies { get; }

        protected RmlWysiwygViewBase(LoadInfo info)
            :base (info)
        {

        }
    }
}
