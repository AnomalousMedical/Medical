using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    class TimelineAnalyzer : Dialog
    {
        private TimelineController timelineController;
        private MultiList timelineList;
        //private PopupMenu popupMenu;

        public TimelineAnalyzer(TimelineController timelineController)
            :base("Medical.GUI.Timeline.TimelineAnalyzer.layout")
        {
            this.timelineController = timelineController;
            timelineController.EditingTimelineChanged += new SingleArgumentEvent<TimelineController, Timeline>(timelineController_EditingTimelineChanged);

            timelineList = window.findWidget("TimelineList") as MultiList;
            timelineList.addColumn("Timeline", timelineList.Width);

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;
            MenuItem analyzeMenu = menuBar.addItem("Analyze", MenuItemType.Popup);
            MenuCtrl analyzeMenuCtrl = analyzeMenu.createItemChild();

            MenuItem open = analyzeMenuCtrl.addItem("Open");
            open.MouseButtonClick += new MyGUIEvent(open_MouseButtonClick);
            MenuItem findReferences = analyzeMenuCtrl.addItem("Find All References");
            findReferences.MouseButtonClick += new MyGUIEvent(findReferences_MouseButtonClick);
            MenuItem listTargets = analyzeMenuCtrl.addItem("List All Targets");
            listTargets.MouseButtonClick += new MyGUIEvent(listTargets_MouseButtonClick);
            analyzeMenuCtrl.addItem("", MenuItemType.Separator);
            MenuItem reset = analyzeMenuCtrl.addItem("Reset to Current");
            reset.MouseButtonClick += new MyGUIEvent(reset_MouseButtonClick);
        }

        void reset_MouseButtonClick(Widget source, EventArgs e)
        {
            timelineList.removeAllItems();
            addTimeline(timelineController.EditingTimeline);
        }

        void open_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void listTargets_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void findReferences_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void timelineController_EditingTimelineChanged(TimelineController source, Timeline arg)
        {
            timelineList.removeAllItems();
            addTimeline(arg);
        }

        void addTimeline(Timeline timeline)
        {
            if (timeline != null && timeline.SourceFile != null)
            {
                String fileName = Path.GetFileName(timeline.SourceFile);
                timelineList.addItem(fileName);
            }
        }

        public override void deserialize(Engine.ConfigFile configFile)
        {
            base.deserialize(configFile);
            window_WindowChangedCoord(null, null);
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            timelineList.setColumnWidthAt(0, timelineList.Width);
        }
    }
}
