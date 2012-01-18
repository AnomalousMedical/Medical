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
        public delegate void SlideEvent(String name);

        public event SlideEvent SlideAdded;
        public event SlideEvent SlideRemoved;
        public event SlideEvent SlideMoved;

        private List<String> slides = new List<String>();

        public LectureCompanion(TimelineController timelineController)
        {
            Timeline closeTimeline = new Timeline();
            closeTimeline.addAction(new MusclePositionAction(timelineController.MovementSequenceController.NeutralMovementState));
            closeTimeline.addAction(new ChangeMedicalStateAction(timelineController.MedicalStateController.NormalState, 0.0f));
            timelineController.saveTimeline(closeTimeline, "Close.tl");
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

            if (!slides.Contains(name))
            {
                slides.Add(name);
                if (SlideAdded != null)
                {
                    SlideAdded.Invoke(name);
                }
            }
            timelineController.saveTimeline(timeline, name);
        }

        public void deleteSlide(String name, TimelineController timelineController)
        {
            if (SlideRemoved != null)
            {
                SlideRemoved.Invoke(name);
            }
            timelineController.deleteFile(name);
            slides.Remove(name);
        }

        public void moveSlideUp(String name)
        {
            int oldIndex = slides.IndexOf(name);
            if (oldIndex != -1 && oldIndex > 0)
            {
                int newIndex = oldIndex - 1;
                slides.Insert(newIndex, name);
                slides.RemoveAt(oldIndex);
                if (SlideMoved != null)
                {
                    SlideMoved.Invoke(name);
                }
            }
        }

        public void moveSlideDown(String name)
        {

        }

        public bool hasSlide(String name)
        {
            return slides.Contains(name);
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
            dataDrivenGUI.PlayTimelineOnCancel = true;
            dataDrivenGUI.CancelTimeline = "Close.tl";

            ShowTimelineGUIAction showTimelineGUI = new ShowTimelineGUIAction();
            showTimelineGUI.GUIData = dataDrivenGUI;
            showTimelineGUI.GUIName = "DataDrivenGUI";

            timeline.addPreAction(showTimelineGUI);
            timeline.Fullscreen = false;

            timelineController.saveTimeline(timeline, "Startup.tl");
            timeline.SourceFile = "Startup.tl";
            timelineController.startPlayback(timeline);
        }

        public int SlideCount
        {
            get
            {
                return slides.Count;
            }
        }

        public IEnumerable<String> SlideNames
        {
            get
            {
                return slides;
            }
        }

        protected LectureCompanion(LoadInfo info)
        {
            info.RebuildList<String>("Slide", slides);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<String>("Slide", slides);
        }
    }
}
