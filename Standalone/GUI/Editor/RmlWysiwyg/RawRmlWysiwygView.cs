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

        private float zoomLevel = 1.0f;
        [Editable]
        public float ZoomLevel
        {
            get
            {
                return zoomLevel;
            }
            set
            {
                zoomLevel = value;
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
