using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;
using Medical.Controller.AnomalousMvc;
using Engine.Attributes;
using Engine;

namespace Medical.GUI.AnomalousMvc
{
    public class RawRmlView : MyGUIView
    {
        private float scaleFactor = -1.0f;

        public event Action<RawRmlView, RmlWidgetComponent> ComponentCreated;

        [DoNotSave]
        private CreateCustomEventControllerDelegate createCustomEventController;

        public RawRmlView(String name)
            : base(name)
        {
            Rml = null;
        }

        [Editable]
        public String Rml { get; set; }

        [Editable]
        public String FakePath { get; set; }

        [Editable]
        public float ScaleFactor
        {
            get
            {
                return scaleFactor;
            }
            set
            {
                scaleFactor = value;
            }
        }

        public float FinalScaleFactor
        {
            get
            {
                float scale = ScaleHelper.ScaleFactor;
                if (scaleFactor > 0)
                {
                    scale *= scaleFactor;
                }
                return scale;
            }
        }

        public CreateCustomEventControllerDelegate CreateCustomEventController
        {
            get
            {
                return createCustomEventController;
            }
            set
            {
                createCustomEventController = value;
            }
        }

        internal RocketEventController createRocketEventController(AnomalousMvcContext mvcContext, ViewHost viewHost)
        {
            if (createCustomEventController != null)
            {
                return createCustomEventController(mvcContext, viewHost);
            }
            return new RmlMvcEventController(mvcContext, viewHost);
        }

        internal void _fireComponentCreated(RmlWidgetComponent component)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, component);
            }
        }

        public RawRmlView(LoadInfo info)
            : base(info)
        {
            
        }
    }
}
