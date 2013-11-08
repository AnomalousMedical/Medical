using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using Engine.Attributes;
using Engine;

namespace Medical.GUI
{
    public class RawRmlWysiwygView : RmlWysiwygViewBase
    {
        private float scaleFactor = -1.0f;

        public event Action<RawRmlWysiwygView, RmlWysiwygComponent> ComponentCreated;
        public event Action<RawRmlWysiwygView> RequestFocus;

        [DoNotSave]
        private UndoRedoBuffer undoBuffer;

        [DoNotSave]
        private LinkedList<ElementStrategy> customStrategies = new LinkedList<ElementStrategy>();

        public RawRmlWysiwygView(String name, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, UndoRedoBuffer undoBuffer)
            :base(name)
        {
            this.UICallback = uiCallback;
            this.BrowserProvider = browserProvider;
            this.undoBuffer = undoBuffer;
        }

        public void addCustomStrategy(ElementStrategy strategy)
        {
            customStrategies.AddLast(strategy);
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

        public Action<String> UndoRedoCallback { get; set; }

        public UndoRedoBuffer UndoBuffer
        {
            get
            {
                return undoBuffer;
            }
        }

        public override IEnumerable<ElementStrategy> CustomElementStrategies
        {
            get
            {
                return customStrategies;
            }
        }

        public MedicalUICallback UICallback { get; private set; }

        public RmlWysiwygBrowserProvider BrowserProvider { get; private set; }

        internal void _fireComponentCreated(RmlWysiwygComponent component)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, component);
            }
        }

        internal override void _fireRequestFocus()
        {
            if (RequestFocus != null)
            {
                RequestFocus.Invoke(this);
            }
        }

        protected RawRmlWysiwygView(LoadInfo info)
            :base (info)
        {

        }
    }
}
