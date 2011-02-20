using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Logging;

namespace Medical.GUI
{
    class TimelineAnalyzer : Dialog
    {
        private TimelineController timelineController;
        private MultiList timelineList;
        private TimelineProperties timelineProperties;
        private bool allowTimelineChanges = true;
        private StaticText lastQuery;

        public TimelineAnalyzer(TimelineController timelineController, TimelineProperties timelineProperties)
            :base("Medical.GUI.Timeline.TimelineAnalyzer.layout")
        {
            this.timelineController = timelineController;
            this.timelineProperties = timelineProperties;
            timelineController.EditingTimelineChanged += new SingleArgumentEvent<TimelineController, Timeline>(timelineController_EditingTimelineChanged);

            timelineList = window.findWidget("TimelineList") as MultiList;
            timelineList.addColumn("Timeline", timelineList.Width / 3);
            timelineList.addColumn("Info", timelineList.Width - timelineList.getColumnWidthAt(0));

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;
            MenuItem analyzeMenu = menuBar.addItem("Analyze", MenuItemType.Popup);
            MenuCtrl analyzeMenuCtrl = analyzeMenu.createItemChild();

            MenuItem open = analyzeMenuCtrl.addItem("Open");
            open.MouseButtonClick += new MyGUIEvent(open_MouseButtonClick);
            analyzeMenuCtrl.addItem("", MenuItemType.Separator);
            MenuItem findReferences = analyzeMenuCtrl.addItem("Find All References");
            findReferences.MouseButtonClick += new MyGUIEvent(findReferences_MouseButtonClick);
            MenuItem listTargets = analyzeMenuCtrl.addItem("List All Targets");
            listTargets.MouseButtonClick += new MyGUIEvent(listTargets_MouseButtonClick);
            MenuItem dumpInfo = analyzeMenuCtrl.addItem("Dump Info to Log");
            dumpInfo.MouseButtonClick += new MyGUIEvent(dumpInfo_MouseButtonClick);
            analyzeMenuCtrl.addItem("", MenuItemType.Separator);
            MenuItem reset = analyzeMenuCtrl.addItem("Reset to Current");
            reset.MouseButtonClick += new MyGUIEvent(reset_MouseButtonClick);

            lastQuery = window.findWidget("LastQuery") as StaticText;
        }

        void dumpInfo_MouseButtonClick(Widget source, EventArgs e)
        {
            String tlFile = timelineList.getItemNameAt(timelineList.getIndexSelected());
            Log.Debug("-----------------------------------------------------------");
            Timeline tl = timelineController.openTimeline(tlFile);
            Log.Debug("Dumping post actions for timeline \"{0}\".", tl.SourceFile);
            tl.dumpPostActionsToLog();
            Log.Debug("-----------------------------------------------------------");
            Log.Debug("");
        }

        void reset_MouseButtonClick(Widget source, EventArgs e)
        {
            timelineList.removeAllItems();
            addTimeline(timelineController.EditingTimeline, "Starting Point");
            setLastQuery("Starting point reset", timelineController.EditingTimeline.SourceFile);

        }

        void open_MouseButtonClick(Widget source, EventArgs e)
        {
            if (timelineList.hasItemSelected())
            {
                allowTimelineChanges = false;
                String file = timelineList.getItemNameAt(timelineList.getIndexSelected());
                timelineProperties.openTimelineFile(file);
                allowTimelineChanges = true;
            }
        }

        void listTargets_MouseButtonClick(Widget source, EventArgs e)
        {
            if (timelineList.hasItemSelected())
            {
                TimelineStaticInfo info = new EndsWithStaticInfo(".tl");

                String tlFile = timelineList.getItemNameAt(timelineList.getIndexSelected());
                Timeline targetListTl = timelineController.openTimeline(tlFile);
                targetListTl.findFileReference(info);
                if (info.HasMatches)
                {
                    timelineList.removeAllItems();
                    foreach (TimelineMatchInfo matchInfo in info.Matches)
                    {
                        string infoStr = String.Format("{0} : {1}", matchInfo.ActionType.Name, matchInfo.Comment);
                        String fileName = Path.GetFileName(matchInfo.File);
                        timelineList.addItem(fileName);
                        uint newIndex = timelineList.getItemCount() - 1;
                        timelineList.setSubItemNameAt(1, newIndex, infoStr);
                        checkSelection();
                    }
                    setLastQuery("List targets", targetListTl.SourceFile);
                }
                else
                {
                    MessageBox.show("No targets found.", "No Matches", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
                }
            }
        }

        void findReferences_MouseButtonClick(Widget source, EventArgs e)
        {
            if (timelineList.hasItemSelected())
            {
                bool foundNoMatches = true;
                String tlFile = timelineList.getItemNameAt(timelineList.getIndexSelected());
                TimelineStaticInfo info = new ExactMatchStaticInfo(tlFile);

                String[] files = timelineController.listResourceFiles("*.tl");
                foreach (String file in files)
                {
                    Timeline tl = timelineController.openTimeline(file);
                    tl.findFileReference(info);
                    if (info.HasMatches)
                    {
                        if (foundNoMatches)
                        {
                            setLastQuery("Find references", tlFile);
                            timelineList.removeAllItems();
                            foundNoMatches = false;
                        }
                        foreach (TimelineMatchInfo matchInfo in info.Matches)
                        {
                            string infoStr = String.Format("{0} : {1}", matchInfo.ActionType.Name, matchInfo.Comment);
                            addTimeline(tl, infoStr);
                        }
                    }
                    info.clearMatches();
                }

                if (foundNoMatches)
                {
                    MessageBox.show("No references found.", "No Matches", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
                }
            }
        }

        void timelineController_EditingTimelineChanged(TimelineController source, Timeline arg)
        {
            if (allowTimelineChanges)
            {
                timelineList.removeAllItems();
                addTimeline(arg, "Starting Point");
                setLastQuery("Edit timeline changed", arg.SourceFile);
            }
        }

        void addTimeline(Timeline timeline, String info)
        {
            if (timeline != null && timeline.SourceFile != null)
            {
                String fileName = Path.GetFileName(timeline.SourceFile);
                timelineList.addItem(fileName);
                uint newIndex = timelineList.getItemCount() - 1;
                timelineList.setSubItemNameAt(1, newIndex, info);
                checkSelection();
            }
        }

        void checkSelection()
        {
            if (!timelineList.hasItemSelected() && timelineList.getItemCount() == 1)
            {
                timelineList.setIndexSelected(0);
            }
        }

        public override void deserialize(Engine.ConfigFile configFile)
        {
            base.deserialize(configFile);
            window_WindowChangedCoord(null, null);
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            int width = timelineList.Width / 3;
            timelineList.setColumnWidthAt(0, width);
            timelineList.setColumnWidthAt(1, timelineList.Width - width);
        }

        void setLastQuery(String query, String tl)
        {
            lastQuery.Caption = String.Format("{0} on {1}", query, tl);
        }
    }
}
