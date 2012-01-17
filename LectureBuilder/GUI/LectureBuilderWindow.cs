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

            LectureCompanion = new LectureCompanion();
        }

        public LectureCompanion LectureCompanion { get; set; }

        void remove_MouseButtonClick(Widget source, EventArgs e)
        {
            if (slides.hasItemSelected())
            {
                uint selectedIndex = slides.getIndexSelected();
                LectureCompanion.deleteSlide(slides.getItemNameAt(selectedIndex), lectureTimelineController);
                slides.removeItemAt(selectedIndex);
            }
        }

        void preview_MouseButtonClick(Widget source, EventArgs e)
        {
            mainTimelineController.ResourceProvider = lectureTimelineController.ResourceProvider.clone();
            LectureCompanion.preview(mainTimelineController);
        }

        void capture_MouseButtonClick(Widget source, EventArgs e)
        {
            LectureCompanion.addSlide(name.OnlyText, lectureTimelineController);
            slides.addItem(name.OnlyText);
        }
    }
}
