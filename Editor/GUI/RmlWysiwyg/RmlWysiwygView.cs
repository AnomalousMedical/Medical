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

namespace Medical.GUI
{
    public class RmlWysiwygView : MyGUIView
    {
        public event Action<RmlWysiwygView, RmlWysiwygComponent> ComponentCreated;

        public RmlWysiwygView(String name)
            :base(name)
        {
            RmlFile = name + ".rml";
        }

        [EditableFile("*.rml", "Rml Files")]
        public String RmlFile { get; set; }

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

        protected RmlWysiwygView(LoadInfo info)
            :base (info)
        {

        }
    }
}
