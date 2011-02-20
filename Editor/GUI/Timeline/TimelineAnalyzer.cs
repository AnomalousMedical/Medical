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
        private TimelineList timelineList;
        private TimelineProperties timelineProperties;
        private bool allowTimelineChanges = true;
        private ActionManager actionManager;

        public TimelineAnalyzer(TimelineController timelineController, TimelineProperties timelineProperties)
            :base("Medical.GUI.Timeline.TimelineAnalyzer.layout")
        {
            this.timelineController = timelineController;
            this.timelineProperties = timelineProperties;

            timelineController.EditingTimelineChanged += new SingleArgumentEvent<TimelineController, Timeline>(timelineController_EditingTimelineChanged);

            actionManager = new ActionManager(window.findWidget("LastQuery") as StaticText);
            timelineList = new TimelineList(window.findWidget("TimelineList") as MultiList, actionManager);

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;
            MenuItem analyzeMenu = menuBar.addItem("Analyze", MenuItemType.Popup);
            MenuCtrl analyzeMenuCtrl = analyzeMenu.createItemChild();

            MenuItem open = analyzeMenuCtrl.addItem("Open");
            open.MouseButtonClick += new MyGUIEvent(open_MouseButtonClick);
            analyzeMenuCtrl.addItem("", MenuItemType.Separator);
            MenuItem findReferences = analyzeMenuCtrl.addItem("Find All References");
            findReferences.MouseButtonClick += new MyGUIEvent(actionMenuItemClick);
            MenuItem listTargets = analyzeMenuCtrl.addItem("List All Targets");
            listTargets.MouseButtonClick += new MyGUIEvent(actionMenuItemClick);
            MenuItem dumpInfo = analyzeMenuCtrl.addItem("Dump Info to Log");
            dumpInfo.MouseButtonClick += new MyGUIEvent(dumpInfo_MouseButtonClick);
            analyzeMenuCtrl.addItem("", MenuItemType.Separator);
            MenuItem reset = analyzeMenuCtrl.addItem("Reset to Current");
            reset.MouseButtonClick += new MyGUIEvent(actionMenuItemClick);

            //Setup actions
            actionManager.addAction(reset, this.reset, "Starting Point Reset");
            actionManager.addAction(listTargets, this.listTargets, "List targets");
            actionManager.addAction(findReferences, this.findReferences, "Find references");
            actionManager.addAction(timelineController, this.reset, "Set by Timeline Editor");
        }

        void dumpInfo_MouseButtonClick(Widget source, EventArgs e)
        {
            if (timelineList.HasSelection)
            {
                String tlFile = timelineList.SelectedTimeline;
                Log.Debug("-----------------------------------------------------------");
                Timeline tl = timelineController.openTimeline(tlFile);
                Log.Debug("Dumping post actions for timeline \"{0}\".", tl.SourceFile);
                tl.dumpPostActionsToLog();
                Log.Debug("-----------------------------------------------------------");
                Log.Debug("");
            }
        }

        #region Action Functions

        String reset()
        {
            timelineList.removeAllItems();
            addTimeline(timelineController.EditingTimeline, "Starting Point");
            return timelineController.EditingTimeline.SourceFile;
        }

        String listTargets()
        {
            String file = null;
            if (timelineList.HasSelection)
            {
                TimelineStaticInfo info = new EndsWithStaticInfo(".tl");

                String tlFile = timelineList.SelectedTimeline;
                Timeline targetListTl = timelineController.openTimeline(tlFile);
                targetListTl.findFileReference(info);
                if (info.HasMatches)
                {
                    timelineList.removeAllItems();
                    foreach (TimelineMatchInfo matchInfo in info.Matches)
                    {
                        string infoStr = String.Format("{0} : {1}", matchInfo.ActionType.Name, matchInfo.Comment);
                        String fileName = Path.GetFileName(matchInfo.File);
                        timelineList.addItem(fileName, infoStr);
                    }
                    file = targetListTl.SourceFile;
                }
                else
                {
                    MessageBox.show("No targets found.", "No Matches", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
                }
            }
            return file;
        }

        String findReferences()
        {
            String ret = null;
            if (timelineList.HasSelection)
            {
                bool foundNoMatches = true;
                String tlFile = timelineList.SelectedTimeline;
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
                            ret = tlFile;
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
            return ret;
        }

        #endregion

        void open_MouseButtonClick(Widget source, EventArgs e)
        {
            if (timelineList.HasSelection)
            {
                allowTimelineChanges = false;
                timelineProperties.openTimelineFile(timelineList.SelectedTimeline);
                allowTimelineChanges = true;
            }
        }

        void actionMenuItemClick(Widget source, EventArgs e)
        {
            actionManager.executeAction(source);
        }

        void timelineController_EditingTimelineChanged(TimelineController source, Timeline arg)
        {
            if (allowTimelineChanges)
            {
                actionManager.executeAction(source);
            }
        }

        void addTimeline(Timeline timeline, String info)
        {
            if (timeline != null && timeline.SourceFile != null)
            {
                String fileName = Path.GetFileName(timeline.SourceFile);
                timelineList.addItem(fileName, info);
            }
        }

        public override void deserialize(Engine.ConfigFile configFile)
        {
            base.deserialize(configFile);
            window_WindowChangedCoord(null, null);
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            timelineList.resizeColumns();
        }

        class ActionManager
        {
            private StaticText lastQueryText;
            private string lastQueryFile = null;

            /// <summary>
            /// Action function. Returns the name of the timeline file that was manipulated.
            /// </summary>
            /// <returns>The name </returns>
            public delegate String ActionFunction();
            private class ActionFunctionInfo
            {
                public ActionFunctionInfo(ActionFunction function, String actionText)
                {
                    Function = function;
                    ActionText = actionText;
                }

                public String ActionText { get; set; }

                public ActionFunction Function { get; set; }
            }

            private Dictionary<Object, ActionFunctionInfo> actions = new Dictionary<object, ActionFunctionInfo>();

            public ActionManager(StaticText lastQueryText)
            {
                this.lastQueryText = lastQueryText;
            }

            public void addAction(Object actionKey, ActionFunction function, String actionInfo)
            {
                actions.Add(actionKey, new ActionFunctionInfo(function, actionInfo));
            }

            public void executeAction(Object actionKey)
            {
                ActionFunctionInfo info;
                if(actions.TryGetValue(actionKey, out info))
                {
                    String returnFile = Path.GetFileName(info.Function());
                    if (returnFile != null)
                    {
                        lastQueryFile = returnFile;
                        lastQueryText.Caption = String.Format("{0} :   {1}", lastQueryFile, info.ActionText);
                    }
                }
            }

            public String LastQueryFile
            {
                get
                {
                    return lastQueryFile;
                }
            }

            public bool HasLastQueryFile
            {
                get
                {
                    return lastQueryFile != null;
                }
            }
        }

        class TimelineList
        {
            private MultiList timelineList;
            private ActionManager actionManager;

            public TimelineList(MultiList timelineList, ActionManager actionManager)
            {
                this.timelineList = timelineList;
                this.actionManager = actionManager;
                timelineList.addColumn("Timeline", timelineList.Width / 3);
                timelineList.addColumn("Info", timelineList.Width - timelineList.getColumnWidthAt(0));
            }

            public String SelectedTimeline
            {
                get
                {
                    if (timelineList.hasItemSelected())
                    {
                        return timelineList.getItemNameAt(timelineList.getIndexSelected());
                    }
                    else
                    {
                        return actionManager.LastQueryFile;
                    }
                }
            }

            public bool HasSelection
            {
                get
                {
                    return timelineList.hasItemSelected() || actionManager.HasLastQueryFile;
                }
            }

            public void removeAllItems()
            {
                timelineList.removeAllItems();
            }

            public void addItem(String timelineColumn, String infoColumn)
            {
                timelineList.addItem(timelineColumn);
                uint newIndex = timelineList.getItemCount() - 1;
                timelineList.setSubItemNameAt(1, newIndex, infoColumn);
            }

            public void resizeColumns()
            {
                int width = timelineList.Width / 3;
                timelineList.setColumnWidthAt(0, width);
                timelineList.setColumnWidthAt(1, timelineList.Width - width);
            }
        }
    }
}
