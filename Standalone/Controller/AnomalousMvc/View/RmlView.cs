using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    public class RmlView : View
    {
        public RmlView(String name)
            :base(name)
        {
            RmlFile = name + ".rml";
        }

        [EditableFile(BrowserWindowController.RmlSearchPattern)]
        public String RmlFile { get; set; }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("View RML File", openFileInViewer));
            editInterface.addCommand(new EditInterfaceCommand("Edit RML File", editFile));
        }

        public enum CustomQueries
        {
            OpenFileInRmlViewer,
            EditWithSystemEditor
        }

        private void openFileInViewer(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.runCustomQuery(CustomQueries.OpenFileInRmlViewer, null, RmlFile);
        }

        private void editFile(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.runCustomQuery(CustomQueries.EditWithSystemEditor, null, RmlFile);
        }

        protected RmlView(LoadInfo info)
            :base (info)
        {

        }
    }
}
