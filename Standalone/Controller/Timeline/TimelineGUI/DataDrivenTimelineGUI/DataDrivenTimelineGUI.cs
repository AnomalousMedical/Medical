using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    public class DataDrivenTimelineGUI : GenericTimelineGUI<DataDrivenTimelineGUIData>
    {
        private DataControl topLevelDataControl;
        private DataDrivenExamSection guiSection;

        public DataDrivenTimelineGUI()
            :base("Medical.Controller.Timeline.TimelineGUI.DataDrivenTimelineGUI.DataDrivenTimelineGUI.layout")
        {
            
        }

        public override void Dispose()
        {
            topLevelDataControl.Dispose();
            base.Dispose();
        }

        protected override void onShown()
        {
            Vector2 startPos = new Vector2(0, 0);
            if (NextTimeline == null && PreviousTimeline == null && MenuTimeline == null)
            {
                //Make a close button and exam if this was launched directly.
                Button previewCloseButton = (Button)widget.createWidgetT("Button", "Button", 0, 0, 100, 20, MyGUIPlugin.Align.Default, "");
                previewCloseButton.MouseButtonClick += new MyGUIEvent(previewCloseButton_MouseButtonClick);
                previewCloseButton.Caption = "Close";
                startPos.y = previewCloseButton.Bottom;

                DataDrivenExam exam = new DataDrivenExam(TimelineFile);
                DataDrivenExamController.Instance.CurrentExam = exam;
                DataDrivenExamController.Instance.CurrentSection = exam;
                guiSection = exam;
            }
            else
            {
                guiSection = DataDrivenExamController.Instance.CurrentSection.getSection(TimelineFile);
            }

            topLevelDataControl = GUIData.createControls(widget, this);
            topLevelDataControl.WorkingSize = new Size2(widget.Width, widget.Height);
            topLevelDataControl.Location = startPos;
            topLevelDataControl.layout();
        }

        protected override void closing()
        {
            topLevelDataControl.captureData(guiSection);
        }

        void previewCloseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.closeAndReturnToMainGUI();
            DataDrivenExamController.Instance.saveAndClear();
        }
    }
}
