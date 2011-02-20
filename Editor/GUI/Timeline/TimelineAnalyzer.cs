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
        private ActionButtonManager actionButtonManager = new ActionButtonManager();

        private Button open;

        public TimelineAnalyzer(TimelineController timelineController, TimelineProperties timelineProperties)
            :base("Medical.GUI.Timeline.TimelineAnalyzer.layout")
        {
            this.timelineController = timelineController;
            this.timelineProperties = timelineProperties;

            timelineController.EditingTimelineChanged += new SingleArgumentEvent<TimelineController, Timeline>(timelineController_EditingTimelineChanged);

            actionManager = new ActionManager(window.findWidget("LastQuery") as StaticText, window.findWidget("BackButton") as Button, window.findWidget("ForwardButton") as Button);
            timelineList = new TimelineList(window.findWidget("TimelineList") as MultiList, actionManager);
            timelineList.TimelineSelected += new TimelineList.TimelineSelectedDelegate(timelineList_TimelineSelected);

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            open = window.findWidget("Open") as Button;
            open.MouseButtonClick += new MyGUIEvent(open_MouseButtonClick);
            actionButtonManager.addButton(open);

            Button findReferences = window.findWidget("Find All References") as Button;
            findReferences.MouseButtonClick += new MyGUIEvent(actionMenuItemClick);
            actionButtonManager.addButton(findReferences);

            Button listTargets = window.findWidget("List All Targets") as Button;
            listTargets.MouseButtonClick += new MyGUIEvent(actionMenuItemClick);
            actionButtonManager.addButton(listTargets);

            Button dumpInfo = window.findWidget("Dump Info to Log") as Button;
            dumpInfo.MouseButtonClick += new MyGUIEvent(dumpInfo_MouseButtonClick);

            Button reset = window.findWidget("Reset to Current") as Button;
            reset.MouseButtonClick += new MyGUIEvent(actionMenuItemClick);

            //Setup actions
            actionManager.addAction(reset, this.reset, this.resetUndo, "Starting Point Reset");
            actionManager.addAction(listTargets, this.listTargets, this.listTargetsUndo, "List targets");
            actionManager.addAction(findReferences, this.findReferences, this.FindReferencesUndo, "Find references");
            actionManager.addAction(timelineController, this.reset, this.resetUndo, "Set by Timeline Editor");

            actionButtonManager.ActiveButton = listTargets;
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

        void resetUndo(String file)
        {
            timelineList.removeAllItems();
            addTimeline(timelineController.openTimeline(file), "Starting Point");
        }

        String listTargets()
        {
            return doListTargets(timelineList.SelectedTimeline);
        }

        void listTargetsUndo(String file)
        {
            doListTargets(file);
        }

        String doListTargets(String tlFile)
        {
            String file = null;
            if (timelineList.HasSelection)
            {
                TimelineStaticInfo info = new EndsWithStaticInfo(".tl");

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
            return doFindReferences(timelineList.SelectedTimeline);
        }

        void FindReferencesUndo(String file)
        {
            doFindReferences(file);
        }

        String doFindReferences(String tlFile)
        {
            String ret = null;
            if (timelineList.HasSelection)
            {
                bool foundNoMatches = true;
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
            doTimelineOpen(timelineList.SelectedTimeline);
        }

        void timelineList_TimelineSelected(string timeline)
        {
            if (actionButtonManager.ActiveButton == open)
            {
                doTimelineOpen(timeline);
            }
            else
            {
                actionManager.executeAction(actionButtonManager.ActiveButton);
            }
        }

        private void doTimelineOpen(string timeline)
        {
            if (timelineList.HasSelection)
            {
                allowTimelineChanges = false;
                timelineProperties.openTimelineFile(timeline);
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

            /// <summary>
            /// Action function. Returns the name of the timeline file that was manipulated.
            /// </summary>
            /// <returns>The name </returns>
            public delegate String ActionFunction();
            public delegate void ActionFunctionUndo(String file);
            private class ActionFunctionInfo
            {
                public ActionFunctionInfo(ActionFunction function, ActionFunctionUndo undoFunction, String actionText)
                {
                    Function = function;
                    UndoFunction = undoFunction;
                    ActionText = actionText;
                }

                public ActionFunctionInfo(ActionFunctionUndo undoFunction, String actionText, String file)
                {
                    UndoFunction = undoFunction;
                    ActionText = actionText;
                    File = file;
                }

                public String ActionText { get; set; }

                public ActionFunction Function { get; set; }

                public ActionFunctionUndo UndoFunction { get; set; }

                public String File { get; set; }
            }

            private Dictionary<Object, ActionFunctionInfo> actions = new Dictionary<object, ActionFunctionInfo>();
            private List<ActionFunctionInfo> history = new List<ActionFunctionInfo>();
            private int historyIndex = 0;

            public ActionManager(StaticText lastQueryText, Button backButton, Button forwardButton)
            {
                this.lastQueryText = lastQueryText;
                backButton.MouseButtonClick += new MyGUIEvent(backButton_MouseButtonClick);
                forwardButton.MouseButtonClick += new MyGUIEvent(forwardButton_MouseButtonClick);
            }

            public void addAction(Object actionKey, ActionFunction function, ActionFunctionUndo undoFunction, String actionInfo)
            {
                actions.Add(actionKey, new ActionFunctionInfo(function, undoFunction, actionInfo));
            }

            public void executeAction(Object actionKey)
            {
                ActionFunctionInfo info;
                if(actions.TryGetValue(actionKey, out info))
                {
                    String returnFile = Path.GetFileName(info.Function());
                    if (returnFile != null)
                    {
                        //Chop off any indexes before this point.
                        int removeIndex = historyIndex + 1;
                        if(removeIndex < history.Count)
                        {
                            history.RemoveRange(removeIndex, history.Count - removeIndex);
                        }

                        //Add the new action to the history
                        history.Add(new ActionFunctionInfo(info.UndoFunction, info.ActionText, returnFile));
                        historyIndex = history.Count - 1;

                        lastQueryText.Caption = String.Format("{0} :   {1}", returnFile, info.ActionText);
                    }
                }
            }

            void forwardButton_MouseButtonClick(Widget source, EventArgs e)
            {
                if (historyIndex + 1 < history.Count)
                {
                    ActionFunctionInfo info = history[++historyIndex];
                    info.UndoFunction(info.File);
                    lastQueryText.Caption = String.Format("{0} :   {1}", info.File, info.ActionText);
                }
            }

            void backButton_MouseButtonClick(Widget source, EventArgs e)
            {
                if (historyIndex - 1 > -1)
                {
                    ActionFunctionInfo info = history[--historyIndex];
                    info.UndoFunction(info.File);
                    lastQueryText.Caption = String.Format("{0} :   {1}", info.File, info.ActionText);
                }
            }

            public String LastQueryFile
            {
                get
                {
                    return history[historyIndex].File;
                }
            }

            public bool HasLastQueryFile
            {
                get
                {
                    return history.Count > 0;
                }
            }
        }

        class ActionButtonManager
        {
            private Button activeButton = null;

            public ActionButtonManager()
            {

            }

            public void addButton(Button button)
            {
                button.MouseButtonReleased += new MyGUIEvent(button_MouseButtonReleased);
            }

            void button_MouseButtonReleased(Widget source, EventArgs e)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                if (me.Button == Engine.Platform.MouseButtonCode.MB_BUTTON1)
                {
                    ActiveButton = (Button)source;
                }
            }

            public Button ActiveButton
            {
                get
                {
                    return activeButton;
                }
                set
                {
                    if (activeButton != null)
                    {
                        activeButton.StateCheck = false;
                    }
                    activeButton = value;
                    if (activeButton != null)
                    {
                        activeButton.StateCheck = true;
                    }
                }
            }
        }

        class TimelineList
        {
            private MultiList timelineList;
            private ActionManager actionManager;
            public delegate void TimelineSelectedDelegate(String timeline);
            public event TimelineSelectedDelegate TimelineSelected;

            public TimelineList(MultiList timelineList, ActionManager actionManager)
            {
                this.timelineList = timelineList;
                this.actionManager = actionManager;
                timelineList.addColumn("Timeline", timelineList.Width / 3);
                timelineList.addColumn("Info", timelineList.Width - timelineList.getColumnWidthAt(0));
                timelineList.SortOnChanges = false;
                timelineList.ListSelectAccept += new MyGUIEvent(timelineList_ListSelectAccept);
            }

            void timelineList_ListSelectAccept(Widget source, EventArgs e)
            {
                if (TimelineSelected != null)
                {
                    TimelineSelected.Invoke(SelectedTimeline);
                }
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
