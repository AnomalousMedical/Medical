using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    class RmlView : View
    {
        public RmlView(String name)
            :base(name)
        {
            RmlFile = name + ".rml";
        }

        [Editable]
        public String RmlFile { get; set; }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("View RML File", openFileInViewer));
        }

        private void openFileInViewer(EditUICallback callback, EditInterfaceCommand command)
        {
            //This uses the other class's custom query so it will need to be replaced when that class is removed
            callback.runCustomQuery(RmlTimelineGUIData.CustomQueries.OpenFileInRmlViewer, null, RmlFile);
        }

        protected RmlView(LoadInfo info)
            :base (info)
        {

        }
    }
}
