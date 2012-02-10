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
        public static DataDrivenTimelineGUIData FindDataInTimeline(Timeline timeline)
        {
            foreach (TimelineInstantAction action in timeline.PreActions)
            {
                if (action is ShowTimelineGUIAction)
                {
                    ShowTimelineGUIAction actionShowGUI = (ShowTimelineGUIAction)action;
                    if (actionShowGUI.GUIData is DataDrivenTimelineGUIData)
                    {
                        return (DataDrivenTimelineGUIData)actionShowGUI.GUIData;
                    }
                }
            }
            foreach (TimelineInstantAction action in timeline.PostActions)
            {
                if (action is ShowTimelineGUIAction)
                {
                    ShowTimelineGUIAction actionShowGUI = (ShowTimelineGUIAction)action;
                    if (actionShowGUI.GUIData is DataDrivenTimelineGUIData)
                    {
                        return (DataDrivenTimelineGUIData)actionShowGUI.GUIData;
                    }
                }
            }
            return null;
        }

        private DataFieldCollection dataFields;

        public DataDrivenTimelineGUIData()
        {
            dataFields = new DataFieldCollection();
            SubmitButtonText = "Submit";
            PlayTimelineOnSubmit = false;
            CancelButtonText = "Cancel";
            PlayTimelineOnCancel = false;
            AllowSubmit = true;
            PreviousButtonText = "Previous";
            NextButtonText = "Next";
            FinishButtonText = "Finish";
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
        public String LayoutFile { get; set; }

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

        [Editable]
        public String PreviousButtonText { get; set; }

        [Editable]
        public String NextButtonText { get; set; }

        [Editable]
        public String FinishButtonText { get; set; }

        public DataFieldCollection DataFields
        {
            get
            {
                return dataFields;
            }
        }

        protected DataDrivenTimelineGUIData(LoadInfo info)
            :base(info)
        {
            if (PreviousButtonText == null)
            {
                PreviousButtonText = "Previous";
            }
            if (NextButtonText == null)
            {
                NextButtonText = "Next";
            }
            if (FinishButtonText == null)
            {
                FinishButtonText = "Finish";
            }
        }
    }
}
