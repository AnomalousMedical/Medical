using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical;
using System.IO;
using Engine.Saving.XMLSaver;
using System.Xml;

namespace LectureBuilder
{
    class LectureCompanion
    {
        private static XmlSaver xmlSaver = new XmlSaver();

        private readonly String CLOSE_TIMELINE = "LectureCompanion_Close.tl";
        private readonly String STARTUP_TIMELINE = "LectureCompanion_Startup.tl";

        public delegate void SlideEvent(String name);

        public event SlideEvent SlideAdded;
        public event SlideEvent SlideRemoved;
        public event SlideEvent SlideMoved;

        private List<String> slides = new List<String>();
        private DDAtlasPlugin plugin;

        public LectureCompanion(TimelineController timelineController, String projectDirectory)
        {
            String timelinesDirectory = Path.Combine(projectDirectory, "Timelines");
            if (Directory.Exists(projectDirectory))
            {
                Directory.Delete(projectDirectory, true);
            }
            if (!Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
                Directory.CreateDirectory(timelinesDirectory);
            }
            timelineController.ResourceProvider = new FilesystemTimelineResourceProvider(timelinesDirectory);

            plugin = new DDAtlasPlugin();
            using (XmlTextWriter xmlWriter = new XmlTextWriter(Path.Combine(projectDirectory, "Plugin.ddp"), Encoding.Default))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlSaver.saveObject(plugin, xmlWriter);
            }

            Timeline closeTimeline = new Timeline();
            
            MusclePositionAction musclePosition = new MusclePositionAction(timelineController.MovementSequenceController.NeutralMovementState);
            musclePosition.Duration = 0.5f;
            closeTimeline.addAction(musclePosition);
            
            ChangeMedicalStateAction changeState = new ChangeMedicalStateAction(timelineController.MedicalStateController.NormalState, 0.0f);
            changeState.Duration = 0.5f;
            closeTimeline.addAction(changeState);
            
            timelineController.saveTimeline(closeTimeline, CLOSE_TIMELINE);
        }

        public void addSlide(String name, TimelineController timelineController)
        {
            if (name.StartsWith("LectureCompanion_"))
            {
                throw new LectureCompanionException("A slide name cannot start with 'LectureCompanion_'. Those names are reserved.\nPlease try another.");
            }

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

        public Timeline createStartupTimeline(TimelineController timelineController)
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
            dataDrivenGUI.CancelTimeline = CLOSE_TIMELINE;

            ShowTimelineGUIAction showTimelineGUI = new ShowTimelineGUIAction();
            showTimelineGUI.GUIData = dataDrivenGUI;
            showTimelineGUI.GUIName = "DataDrivenGUI";

            timeline.addPreAction(showTimelineGUI);
            timeline.Fullscreen = false;

            timelineController.saveTimeline(timeline, STARTUP_TIMELINE);
            timeline.SourceFile = STARTUP_TIMELINE;
            return timeline;
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
    }
}
