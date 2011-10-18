using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using MyGUIPlugin;

namespace Medical
{
    public class DataDrivenTimelineGUIData : AbstractTimelineGUIData
    {
        private DataFieldCollection dataFields;

        public DataDrivenTimelineGUIData()
        {
            dataFields = new DataFieldCollection();
            SubmitButtonText = "Submit";
        }

        public DataControl createControls(Widget parent, DataDrivenTimelineGUI gui)
        {
            return dataFields.createControls(parent, gui);
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(dataFields.getEditInterface());
        }

        [Editable]
        public String SubmitButtonText { get; set; }

        protected DataDrivenTimelineGUIData(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
