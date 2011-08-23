using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using MyGUIPlugin;

namespace Medical
{
    class DataDrivenTimelineGUIData : AbstractTimelineGUIData
    {
        private DataFieldCollection dataFields;

        public DataDrivenTimelineGUIData()
        {
            dataFields = new DataFieldCollection();
        }

        public DataControl createControls(Widget parent)
        {
            return dataFields.createControls(parent);
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(dataFields.getEditInterface());
        }

        protected DataDrivenTimelineGUIData(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
