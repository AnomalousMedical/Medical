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

        /// <summary>
        /// This property when specified will allow you to customize the rml that is put
        /// in as a placeholder. Its possible to edit that rml, so if you structure the document
        /// you return correctly the user could just edit it to make their changes.
        /// </summary>
        public RmlWysiwygComponent.GetMissingRmlDelegate GetMissingRmlCallback { get; set; }

        internal abstract void _fireRequestFocus();

        public abstract IEnumerable<ElementStrategy> CustomElementStrategies { get; }

        protected RmlWysiwygViewBase(LoadInfo info)
            :base (info)
        {

        }
    }
}
