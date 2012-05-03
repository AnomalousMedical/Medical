using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class ViewCollection : SaveableEditableItemCollection<View>
    {
        public ViewCollection()
        {
            
        }

        public override void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<View> itemEdits)
        {
            addItemCreation("Add Rml View", delegate(String name)
            {
                return new RmlView(name);
            });
        }

        public override String EditInterfaceName
        {
            get
            {
                return "Views";
            }
        }

        protected ViewCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
