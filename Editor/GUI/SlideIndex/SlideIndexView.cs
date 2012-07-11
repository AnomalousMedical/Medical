using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Medical.GUI.AnomalousMvc;
using Medical.Presentation;

namespace Medical.GUI
{
    class SlideIndexView : MyGUIView
    {
        public event Action<SlideIndexView, SlideIndex> ComponentCreated;

        public SlideIndexView(String name, PresentationIndex presentation)
            :base(name)
        {
            this.Presentation = presentation;
        }

        public PresentationIndex Presentation { get; set; }

        /// <summary>
        /// This is used by the factory to fire when one of these components has
        /// been created. DO NOT call from anywhere else.
        /// </summary>
        /// <param name="component">The TextEditorComponent that was created.</param>
        internal void _fireComponentCreated(SlideIndex component)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, component);
            }
        }

        protected SlideIndexView(LoadInfo info)
            :base(info)
        {

        }
    }
}
