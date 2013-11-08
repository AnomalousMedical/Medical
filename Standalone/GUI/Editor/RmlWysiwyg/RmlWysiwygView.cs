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
    public class RmlWysiwygView : RmlWysiwygViewBase
    {
        public event Action<RmlWysiwygView, RmlWysiwygComponent> ComponentCreated;
        public event Action<RmlWysiwygView> RequestFocus;

        [DoNotSave]
        private UndoRedoBuffer undoBuffer;

        [DoNotSave]
        private LinkedList<ElementStrategy> customStrategies = new LinkedList<ElementStrategy>();

        public RmlWysiwygView(String name, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, UndoRedoBuffer undoBuffer)
            :base(name)
        {
            RmlFile = name + ".rml";

            this.UICallback = uiCallback;
            this.BrowserProvider = browserProvider;
            this.undoBuffer = undoBuffer;
        }

        [EditableFile("*.rml", "Rml Files")]
        public String RmlFile { get; set; }

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

        public void addCustomStrategy(ElementStrategy strategy)
        {
            customStrategies.AddLast(strategy);
        }

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

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Edit", openFileInViewer));
            base.customizeEditInterface(editInterface);
        }

        public enum CustomQueries
        {
            OpenFileInRmlViewer
        }

        private void openFileInViewer(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.runOneWayCustomQuery(CustomQueries.OpenFileInRmlViewer, RmlFile);
        }

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

        protected RmlWysiwygView(LoadInfo info)
            :base (info)
        {

        }
    }
}
