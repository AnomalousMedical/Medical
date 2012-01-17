using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical;
using MyGUIPlugin;
using System.IO;

namespace LectureBuilder
{
    class LectureBuilderWindow : MDIDialog
    {
        private TimelineController lectureTimelineController;
        private TimelineController mainTimelineController;

        private LectureCompanion lecComp;

        private Edit name;
        private MultiList slides;

        public LectureBuilderWindow(TimelineController lectureTimelineController, TimelineController mainTimelineController)
            : base("LectureBuilder.GUI.LectureBuilderWindow.layout")
        {
            this.lectureTimelineController = lectureTimelineController;
            this.mainTimelineController = mainTimelineController;

            String outputPath = Path.Combine(MedicalConfig.UserDocRoot, "TEMP_LectureCompanion");
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            lectureTimelineController.ResourceProvider = new FilesystemTimelineResourceProvider(outputPath);

            Button capture = (Button)window.findWidget("Capture");
            capture.MouseButtonClick += new MyGUIEvent(capture_MouseButtonClick);

            Button preview = (Button)window.findWidget("Preview");
            preview.MouseButtonClick += new MyGUIEvent(preview_MouseButtonClick);

            Button remove = (Button)window.findWidget("Remove");
            remove.MouseButtonClick += new MyGUIEvent(remove_MouseButtonClick);

            name = (Edit)window.findWidget("Name");

            slides = (MultiList)window.findWidget("Slides");
            slides.addColumn("Name", slides.ClientCoord.width);

            LectureCompanion = new LectureCompanion(lectureTimelineController);
        }

        public LectureCompanion LectureCompanion
        {
            get
            {
                return lecComp;
            }
            set
            {
                if (lecComp != null)
                {
                    lecComp.SlideAdded -= lecComp_SlideAdded;
                    lecComp.SlideRemoved -= lecComp_SlideRemoved;
                }
                lecComp = value;
                if (lecComp != null)
                {
                    lecComp.SlideAdded += lecComp_SlideAdded;
                    lecComp.SlideRemoved += lecComp_SlideRemoved;
                }
            }
        }

        void lecComp_SlideRemoved(string name)
        {
            uint index;
            if (slides.findSubItemWith(0, name, out index))
            {
                slides.removeItemAt(index);
            }
        }

        void lecComp_SlideAdded(string name)
        {
            slides.addItem(name);
        }

        void remove_MouseButtonClick(Widget source, EventArgs e)
        {
            if (slides.hasItemSelected())
            {
                uint selectedIndex = slides.getIndexSelected();
                LectureCompanion.deleteSlide(slides.getItemNameAt(selectedIndex), lectureTimelineController);
            }
        }

        void preview_MouseButtonClick(Widget source, EventArgs e)
        {
            mainTimelineController.ResourceProvider = lectureTimelineController.ResourceProvider.clone();
            LectureCompanion.preview(mainTimelineController);
        }

        void capture_MouseButtonClick(Widget source, EventArgs e)
        {
            String slideName = name.OnlyText;
            if (!String.IsNullOrEmpty(slideName))
            {
                if (LectureCompanion.hasSlide(slideName))
                {
                    MessageBox.show(String.Format("Are you sure you want to overwrite the slide named '{0}'.", slideName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            LectureCompanion.addSlide(slideName, lectureTimelineController);
                        }
                    });
                }
                else
                {
                    LectureCompanion.addSlide(slideName, lectureTimelineController);
                }
            }
            else
            {
                MessageBox.show("Please enter a name for this slide.", "No Name", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
            }
        }
    }
}
