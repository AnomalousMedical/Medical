using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Controller.AnomalousMvc
{
    public class RmlView : View
    {
        public RmlView(String name)
            :base(name)
        {
            RmlFile = name + ".rml";
        }

        public String RmlFile { get; set; }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addEditableProperty(new BrowseableEditableProperty("RmlFile", new PropertyMemberWrapper(this.GetType().GetProperty("RmlFile")), this, BrowserWindowController.RmlSearchPattern));
            editInterface.addCommand(new EditInterfaceCommand("View RML File", openFileInViewer));
        }

        public enum CustomQueries
        {
            OpenFileInRmlViewer
        }

        private void openFileInViewer(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.runCustomQuery(CustomQueries.OpenFileInRmlViewer, null, RmlFile);
        }

        protected RmlView(LoadInfo info)
            :base (info)
        {

        }
    }
}
