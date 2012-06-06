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
        public RmlView(String name)
            :base(name)
        {
            RmlFile = name + ".rml";
        }

        [EditableFile(BrowserWindowController.RmlSearchPattern, "Rml Files")]
        public String RmlFile { get; set; }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("View RML File", openFileInViewer));
            editInterface.addCommand(new EditInterfaceCommand("Edit RML File", editFile));
            base.customizeEditInterface(editInterface);
        }

        public enum CustomQueries
        {
            OpenFileInRmlViewer,
            EditWithSystemEditor
        }

        private void openFileInViewer(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.runOneWayCustomQuery(CustomQueries.OpenFileInRmlViewer, RmlFile);
        }

        private void editFile(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.runOneWayCustomQuery(CustomQueries.EditWithSystemEditor, RmlFile);
        }

        protected RmlView(LoadInfo info)
            :base (info)
        {

        }
    }
}
