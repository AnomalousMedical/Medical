using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    public class RmlView : MyGUIView
    {
        public event Action<RmlView, RmlWidgetComponent> ComponentCreated;

        public RmlView(String name)
            :base(name)
        {
            RmlFile = name + ".rml";
        }

        [EditableFile(BrowserWindowController.RmlSearchPattern, "Rml Files")]
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

        internal void _fireComponentCreated(RmlWidgetComponent component)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, component);
            }
        }

        protected RmlView(LoadInfo info)
            :base (info)
        {

        }
    }
}
