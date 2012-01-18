using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical;
using MyGUIPlugin;
using System.IO;
using Logging;

namespace LectureBuilder
{
    class LectureBuilderWindow : MDIDialog
    {
        private TimelineController lectureTimelineController;
        private TimelineController mainTimelineController;

        private LectureCompanion lecComp;
        private String currentProject = null;

        private Edit name;
        private ScrollView slideScroller;
        private ButtonGrid slides;
        private Button capture;
        private Button preview;
        private Button remove;
        private Button moveUp;
        private Button moveDown;

        public LectureBuilderWindow(TimelineController lectureTimelineController, TimelineController mainTimelineController)
            : base("LectureBuilder.GUI.LectureBuilderWindow.layout")
        {
            this.lectureTimelineController = lectureTimelineController;
            this.mainTimelineController = mainTimelineController;

            capture = (Button)window.findWidget("Capture");
            capture.MouseButtonClick += new MyGUIEvent(capture_MouseButtonClick);

            preview = (Button)window.findWidget("Preview");
            preview.MouseButtonClick += new MyGUIEvent(preview_MouseButtonClick);

            remove = (Button)window.findWidget("Remove");
            remove.MouseButtonClick += new MyGUIEvent(remove_MouseButtonClick);

            moveUp = (Button)window.findWidget("MoveUp");
            moveUp.MouseButtonClick += new MyGUIEvent(moveUp_MouseButtonClick);

            moveDown = (Button)window.findWidget("MoveDown");
            moveDown.MouseButtonClick += new MyGUIEvent(moveDown_MouseButtonClick);

            name = (Edit)window.findWidget("Name");

            slideScroller = (ScrollView)window.findWidget("SlideScroller");
            slides = new ButtonGrid(slideScroller, new ButtonGridListLayout());
            slides.ItemActivated += new EventHandler(slides_ItemActivated);

            MenuBar menuBar = (MenuBar)window.findWidget("MenuBar");
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            PopupMenu fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            fileMenu.addItem("New", MenuItemType.Normal, "New");
            fileMenu.addItem("Open", MenuItemType.Normal, "Open");

            setInterfaceEnabled(false);
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
                    lecComp.SlideMoved -= lecComp_SlideMoved;
                }
                lecComp = value;
                if (lecComp != null)
                {
                    lecComp.SlideAdded += lecComp_SlideAdded;
                    lecComp.SlideRemoved += lecComp_SlideRemoved;
                    lecComp.SlideMoved += lecComp_SlideMoved;
                }
            }
        }

        void lecComp_SlideMoved(string name)
        {
            slides.clear();
            foreach (String slideName in LectureCompanion.SlideNames)
            {
                slides.addItem("", slideName);
            }
        }

        void lecComp_SlideRemoved(string name)
        {
            ButtonGridItem item = slides.findItemByCaption(name);
            if (item != null)
            {
                slides.removeItem(item);
            }
        }

        void lecComp_SlideAdded(string name)
        {
            slides.addItem("", name);
        }

        void remove_MouseButtonClick(Widget source, EventArgs e)
        {
            ButtonGridItem item = slides.SelectedItem;
            if (item != null)
            {
                LectureCompanion.deleteSlide(item.Caption, lectureTimelineController);
            }
        }

        void preview_MouseButtonClick(Widget source, EventArgs e)
        {
            mainTimelineController.ResourceProvider = lectureTimelineController.ResourceProvider.clone();
            mainTimelineController.startPlayback(LectureCompanion.createStartupTimeline(mainTimelineController));
        }

        void moveDown_MouseButtonClick(Widget source, EventArgs e)
        {
            //ButtonGridItem item = slides.SelectedItem;
            //if (item != null)
            //{
            //    uint selectedIndex = slides.getIndexSelected();
            //    LectureCompanion.moveSlideDown(slides.getItemNameAt(selectedIndex));
            //}
        }

        void moveUp_MouseButtonClick(Widget source, EventArgs e)
        {
            //if (slides.hasItemSelected())
            //{
            //    uint selectedIndex = slides.getIndexSelected();
            //    LectureCompanion.moveSlideUp(slides.getItemNameAt(selectedIndex));
            //}
        }

        void slides_ItemActivated(object sender, EventArgs e)
        {
            String selectedName = slides.SelectedItem.Caption;
            Timeline tl = lectureTimelineController.openTimeline(selectedName);
            if (tl != null)
            {
                lectureTimelineController.startPlayback(tl);
            }
            name.OnlyText = selectedName;
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

        void fileMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            switch (mcae.Item.ItemId)
            {
                case "New":
                    newClicked();
                    break;
                case "Open":
                    break;
            }
        }

        void newClicked()
        {
            try
            {
                createNewLectureCompanion("Untitled");
                setInterfaceEnabled(true);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving your lecture companion project to\n'{0}'\nPlease make sure that destination is valid.", currentProject), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                setInterfaceEnabled(false);
                Log.Error("Could not create lecture companion. {0}", ex.Message);
            }
        }

        void createNewLectureCompanion(String name)
        {
            currentProject = Path.Combine(MedicalConfig.UserDocRoot, "Lecture Companions", name);
            if (Directory.Exists(currentProject))
            {
                Directory.Delete(currentProject, true);
            }
            if (!Directory.Exists(currentProject))
            {
                Directory.CreateDirectory(currentProject);
            }
            lectureTimelineController.ResourceProvider = new FilesystemTimelineResourceProvider(currentProject);

            LectureCompanion = new LectureCompanion(lectureTimelineController);
        }

        void setInterfaceEnabled(bool enabled)
        {
            name.Enabled = enabled;
            capture.Enabled = enabled;
            preview.Enabled = enabled;
            remove.Enabled = enabled;
            moveUp.Enabled = enabled;
            moveDown.Enabled = enabled;
        }
    }
}
