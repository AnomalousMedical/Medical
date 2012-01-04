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
            PlayTimelineOnSubmit = false;
            CancelButtonText = "Cancel";
            PlayTimelineOnCancel = false;
            AllowSubmit = true;
        }

        public void createControls(DataControlFactory factory)
        {
            dataFields.createControls(factory);
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(dataFields.getEditInterface());
        }

        [Editable]
        public bool AllowSubmit { get; set; }

        [Editable]
        public String SubmitButtonText { get; set; }

        [Editable]
        public bool PlayTimelineOnSubmit { get; set; }

        [Editable]
        public String SubmitTimeline { get; set; }

        [Editable]
        public String CancelButtonText { get; set; }

        [Editable]
        public bool PlayTimelineOnCancel { get; set; }

        [Editable]
        public String CancelTimeline { get; set; }

        [Editable]
        public String PrettyName { get; set; }

        protected DataDrivenTimelineGUIData(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
