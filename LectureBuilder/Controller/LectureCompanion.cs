using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical;

namespace LectureBuilder
{
    class LectureCompanion : Saveable
    {
        private List<String> slides = new List<String>();

        public LectureCompanion()
        {
            
        }

        public void addSlide(String name, TimelineController timelineController)
        {
            Timeline timeline = new Timeline();
            timelineController.setAsTimelineController(timeline);
            
            MoveCameraAction moveCamera = new MoveCameraAction();
            timeline.addAction(moveCamera);
            moveCamera.capture();

            ChangeMedicalStateAction medicalState = new ChangeMedicalStateAction();
            timeline.addAction(medicalState);
            medicalState.capture();

            LayerChangeAction layers = new LayerChangeAction();
            timeline.addAction(layers);
            layers.capture();

            MusclePositionAction musclePosition = new MusclePositionAction();
            timeline.addAction(musclePosition);
            musclePosition.capture();

            slides.Add(name);
            timelineController.saveTimeline(timeline, name);
        }

        public void deleteSlide(String name, TimelineController timelineController)
        {
            timelineController.deleteFile(name);
        }

        public void preview(TimelineController timelineController)
        {
            Timeline timeline = new Timeline();

            DataDrivenTimelineGUIData dataDrivenGUI = new DataDrivenTimelineGUIData();
            DataFieldCollection dataFields = dataDrivenGUI.DataFields;
            foreach (String name in slides)
            {
                dataFields.addDataField(new PlayExampleDataField(name, name));
            }
            dataDrivenGUI.AllowSubmit = false;
            dataDrivenGUI.CancelButtonText = "Close";

            ShowTimelineGUIAction showTimelineGUI = new ShowTimelineGUIAction();
            showTimelineGUI.GUIData = dataDrivenGUI;
            showTimelineGUI.GUIName = "DataDrivenGUI";

            timeline.addPreAction(showTimelineGUI);

            timelineController.saveTimeline(timeline, "Startup.tl");
            timeline.SourceFile = "Startup.tl";
            timelineController.startPlayback(timeline);
        }

        protected LectureCompanion(LoadInfo info)
        {

        }

        public void getInfo(SaveInfo info)
        {

        }
    }
}
