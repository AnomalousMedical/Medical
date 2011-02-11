using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Engine;
using Logging;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;

namespace Medical.GUI
{
    class TimelineProperties : Dialog
    {
        private const String PROJECT_EXTENSION = ".tlp";
        private const String PROJECT_WILDCARD = "Timeline Projects (*.tlp)|*.tlp";

        private Timeline currentTimeline;
        private TimelineController timelineController;
        private String currentTimelineFile;
        private TimelineDataProperties dataProperties;
        private TrackFilter actionFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Dictionary<TimelineAction, TimelineActionData> actionDataBindings = new Dictionary<TimelineAction, TimelineActionData>();
        private TimelineAction copySourceAction;
        private CopySaver copySaver = new CopySaver();
        private TimelineActionFactory actionFactory;

        //Menus
        private ShowMenuButton fileMenuButton;
        private PopupMenu fileMenu;
        private ShowMenuButton editMenuButton;
        private PopupMenu editMenu;
        private ShowMenuButton otherActionsMenuButton;
        private PopupMenu otherActionsMenu;
        private MenuItem testActions;
        private ShowMenuButton analyzeMenuButton;
        private PopupMenu analyzeMenu;


        //Dialogs
        private ChangeSceneEditor changeSceneEditor;
        private NewProjectDialog newProjectDialog;
        private TimelineFileBrowserDialog fileBrowserDialog;
        private SaveTimelineDialog saveTimelineDialog;
        private FinishActionEditor finishActionEditor;
        private TimelineIndexEditor timelineIndexEditor;

        private Button playButton;
        private Button rewindButton;
        private Button fastForwardButton;

        private const int START_COLUMN_WIDTH = 100;

        //File Menu
        MenuItem newTimelineItem;
        MenuItem openTimelineItem;
        MenuItem saveTimelineItem;
        MenuItem saveTimelineAsItem;

        public TimelineProperties(TimelineController timelineController, EditorGUIPlugin editorGUI)
            :base("Medical.GUI.Timeline.TimelineProperties.layout")
        {
            this.timelineController = timelineController;
            timelineController.TimelinePlaybackStarted += new EventHandler(timelineController_TimelinePlaybackStarted);
            timelineController.TimelinePlaybackStopped += new EventHandler(timelineController_TimelinePlaybackStopped);
            timelineController.TimeTicked += new TimeTickEvent(timelineController_TimeTicked);
            timelineController.ResourceLocationChanged += new EventHandler(timelineController_ResourceLocationChanged);

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;

            //File Menu
            MenuItem fileMenuItem = menuBar.addItem("File");
            fileMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            fileMenu.Visible = false;
            MenuItem newProject = fileMenu.addItem("New Project");
            newProject.MouseButtonClick += new MyGUIEvent(newProject_MouseButtonClick);
            MenuItem openProject = fileMenu.addItem("Open Project");
            openProject.MouseButtonClick += new MyGUIEvent(openProject_MouseButtonClick);
            fileMenu.addItem("", MenuItemType.Separator);
            newTimelineItem = fileMenu.addItem("New Timeline");
            newTimelineItem.MouseButtonClick += new MyGUIEvent(newTimeline_MouseButtonClick);
            openTimelineItem = fileMenu.addItem("Open Timeline");
            openTimelineItem.MouseButtonClick += new MyGUIEvent(openTimeline_MouseButtonClick);
            saveTimelineItem = fileMenu.addItem("Save Timeline");
            saveTimelineItem.MouseButtonClick += new MyGUIEvent(saveTimeline_MouseButtonClick);
            saveTimelineAsItem = fileMenu.addItem("Save Timeline As");
            saveTimelineAsItem.MouseButtonClick += new MyGUIEvent(saveTimelineAs_MouseButtonClick);
            fileMenuButton = new ShowMenuButton(fileMenuItem, fileMenu);

            //Edit button
            MenuItem editMenuItem = menuBar.addItem("Edit");
            editMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            editMenu.Visible = false;
            MenuItem copy = editMenu.addItem("Copy");
            copy.MouseButtonClick += new MyGUIEvent(copy_MouseButtonClick);
            MenuItem paste = editMenu.addItem("Paste");
            paste.MouseButtonClick += new MyGUIEvent(paste_MouseButtonClick);
            editMenuButton = new ShowMenuButton(editMenuItem, editMenu);

            //Other Actions Menu
            MenuItem actionsMenuItem = menuBar.addItem("Actions");
            otherActionsMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            otherActionsMenu.Visible = false;
            MenuItem editTimelineIndex = otherActionsMenu.addItem("Edit Timeline Index");
            editTimelineIndex.MouseButtonClick += new MyGUIEvent(editTimelineIndex_MouseButtonClick);
            MenuItem changeScene = otherActionsMenu.addItem("Change Scene");
            changeScene.MouseButtonClick += new MyGUIEvent(changeScene_MouseButtonClick);
            MenuItem finishAction = otherActionsMenu.addItem("Finish Action");
            finishAction.MouseButtonClick += new MyGUIEvent(finishAction_MouseButtonClick);
            MenuItem reverseSidesAction = otherActionsMenu.addItem("Reverse Sides");
            reverseSidesAction.MouseButtonClick += new MyGUIEvent(reverseSidesAction_MouseButtonClick);
            otherActionsMenu.addItem("", MenuItemType.Separator);
            testActions = otherActionsMenu.addItem("Enable Other Actions");
            testActions.StateCheck = false;
            testActions.MouseButtonClick += new MyGUIEvent(testActions_MouseButtonClick);
            otherActionsMenuButton = new ShowMenuButton(actionsMenuItem, otherActionsMenu);

            //Analyze Menu
            MenuItem analyzeMenuItem = menuBar.addItem("Analyze");
            analyzeMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            analyzeMenu.Visible = false;
            MenuItem dumpPostActions = analyzeMenu.addItem("Dump PostActions to Log");
            dumpPostActions.MouseButtonClick += new MyGUIEvent(dumpPostActions_MouseButtonClick);
            analyzeMenuButton = new ShowMenuButton(analyzeMenuItem, analyzeMenu);
           
            //Remove action button
            Button removeActionButton = window.findWidget("RemoveAction") as Button;
            removeActionButton.MouseButtonClick += new MyGUIEvent(removeActionButton_MouseButtonClick);

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            fastForwardButton = window.findWidget("FastForward") as Button;
            fastForwardButton.MouseButtonClick += new MyGUIEvent(fastForwardButton_MouseButtonClick);

            rewindButton = window.findWidget("Rewind") as Button;
            rewindButton.MouseButtonClick += new MyGUIEvent(rewindButton_MouseButtonClick);

            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);

            //Properties
            ScrollView propertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            dataProperties = new TimelineDataProperties(propertiesScrollView, timelineView);
            dataProperties.Visible = false;

            //Track filter
            ScrollView trackFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            actionFilter = new TrackFilter(trackFilterScrollView, timelineView);
            actionFilter.AddTrackItem += new AddTrackItemCallback(actionFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            //Dialogs
            changeSceneEditor = new ChangeSceneEditor();
            newProjectDialog = new NewProjectDialog(PROJECT_EXTENSION);
            newProjectDialog.ProjectCreated += new EventHandler(newProjectDialog_ProjectCreated);
            fileBrowserDialog = new TimelineFileBrowserDialog(timelineController);
            timelineController.FileBrowser = fileBrowserDialog;
            saveTimelineDialog = new SaveTimelineDialog();
            saveTimelineDialog.SaveFile += new EventHandler(saveTimelineDialog_SaveFile);
            finishActionEditor = new FinishActionEditor(timelineController, fileBrowserDialog);
            timelineIndexEditor = new TimelineIndexEditor(fileBrowserDialog);
            timelineIndexEditor.SaveIndexData += new EventHandler(timelineIndexEditor_SaveIndexData);

            //Add tracks to timeline.
            actionFactory = new TimelineActionFactory(propertiesScrollView, editorGUI);
            foreach (TimelineActionFactoryData actionProp in actionFactory.ActionProperties)
            {
                timelineView.addTrack(actionProp.TypeName, actionProp.Color);

                if (actionProp.Panel != null)
                {
                    dataProperties.addPanel(actionProp.TypeName, actionProp.Panel);
                }
            }

            setEnabled(false);

            createNewTimeline();
        }

        public override void Dispose()
        {
            actionFactory.Dispose();
            finishActionEditor.Dispose();
            newProjectDialog.Dispose();
            timelineController.FileBrowser = null;
            fileBrowserDialog.Dispose();
            saveTimelineDialog.Dispose();
            changeSceneEditor.Dispose();
            actionFilter.Dispose();
            timelineView.Dispose();
            Gui.Instance.destroyWidget(fileMenu);
            Gui.Instance.destroyWidget(editMenu);
            Gui.Instance.destroyWidget(otherActionsMenu);
            Gui.Instance.destroyWidget(analyzeMenu);
            base.Dispose();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            
        }

        public void setCurrentTimeline(Timeline timeline)
        {
            String filename = timeline.SourceFile;

            timelineController.EditingTimeline = timeline;
            if (currentTimeline != null)
            {
                currentTimeline.ActionAdded -= currentTimeline_ActionAdded;
                currentTimeline.ActionRemoved -= currentTimeline_ActionRemoved;
            }
            changeTimelineFile(filename);
            currentTimeline = timeline;
            timelineView.removeAllData();
            foreach (TimelineAction action in currentTimeline.Actions)
            {
                addActionToTimeline(action);
            }
            currentTimeline.ActionAdded += currentTimeline_ActionAdded;
            currentTimeline.ActionRemoved += currentTimeline_ActionRemoved;
            changeSceneEditor.CurrentTimeline = currentTimeline;
            finishActionEditor.CurrentTimeline = currentTimeline;
        }

        private void changeTimelineFile(String filename)
        {
            currentTimelineFile = filename;
            updateWindowCaption();
        }

        private void updateWindowCaption()
        {
            if (timelineController.ResourceProvider != null)
            {
                if (currentTimelineFile != null)
                {
                    window.Caption = String.Format("Timeline - {0} - {1}", Path.GetFileName(currentTimelineFile), timelineController.ResourceProvider.BackingLocation);
                }
                else
                {
                    window.Caption = String.Format("Timeline - {0}", timelineController.ResourceProvider.BackingLocation);
                }
            }
            else
            {
                window.Caption = "Timeline";
            }
        }

        private void createProject(string projectName)
        {
            using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(projectName))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    XmlTextWriter xmlWriter = new XmlTextWriter(memStream, Encoding.Default);
                    xmlWriter.Formatting = Formatting.Indented;
                    TimelineIndex index = new TimelineIndex();
                    XmlSaver xmlSaver = new XmlSaver();
                    xmlSaver.saveObject(index, xmlWriter);
                    xmlWriter.Flush();
                    memStream.Seek(0, SeekOrigin.Begin);
                    ionicZip.AddEntry(TimelineController.INDEX_FILE_NAME, memStream);
                    ionicZip.Save();
                }
            }
            timelineController.ResourceProvider = new TimelineZipResources(projectName);
        }

        public void createNewTimeline()
        {
            Timeline timeline = new Timeline();
            setCurrentTimeline(timeline);
        }

        #region Action Management

        void removeActionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            currentTimeline.removeAction(((TimelineActionData)timelineView.CurrentData).Action);
        }

        void actionFilter_AddTrackItem(string name)
        {
            TimelineAction action = actionFactory.createAction(name);
            action.StartTime = timelineView.MarkerTime;
            currentTimeline.addAction(action);
            action.capture();
            timelineView.CurrentData = actionDataBindings[action];
        }

        void currentTimeline_ActionAdded(object sender, TimelineActionEventArgs e)
        {
            addActionToTimeline(e.Action);
        }

        private void addActionToTimeline(TimelineAction action)
        {
            TimelineActionData data = new TimelineActionData(action);
            actionDataBindings.Add(action, data);
            timelineView.addData(data);
        }

        void currentTimeline_ActionRemoved(object sender, TimelineActionEventArgs e)
        {
            timelineView.removeData(actionDataBindings[e.Action]);
        }

        #endregion

        #region File Menu

        /// <summary>
        /// Create a new project. You can optionally delete the old project.
        /// </summary>
        /// <param name="filename">The file name of the new project.</param>
        /// <param name="deleteOld">True to delete any existing project first.</param>
        void createNewProject(String filename, bool deleteOld)
        {
            try
            {
                if (deleteOld)
                {
                    File.Delete(filename);
                }
                createProject(filename);
                updateWindowCaption();
            }
            catch (Exception ex)
            {
                String errorMessage = String.Format("Error creating new project {0}.", ex.Message);
                Log.Error(errorMessage);
                MessageBox.show(errorMessage, "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        void newProject_MouseButtonClick(Widget source, EventArgs e)
        {
            newProjectDialog.open(true);
            newProjectDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            newProjectDialog.ensureVisible();
            fileMenu.setVisibleSmooth(false);
        }

        void newProjectDialog_ProjectCreated(object sender, EventArgs e)
        {
            String newProjectName = newProjectDialog.FullProjectName;
            if (File.Exists(newProjectName))
            {
                MessageBox.show(String.Format("A project named {0} already exists. Would you like to overwrite it?", newProjectName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, overwriteNewProject);
            }
            else
            {
                createNewProject(newProjectName, false);
            }
        }

        void overwriteNewProject(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                createNewProject(newProjectDialog.FullProjectName, true);
            }
        }

        void openProject_MouseButtonClick(Widget source, EventArgs e)
        {
            fileMenu.setVisibleSmooth(false);
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a timeline.", newProjectDialog.ProjectLocation, "", PROJECT_WILDCARD, false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    timelineController.ResourceProvider = new TimelineZipResources(fileDialog.Path);
                    updateWindowCaption();
                }
            }
        }

        void saveTimelineAs_MouseButtonClick(Widget source, EventArgs e)
        {
            saveTimelineDialog.open(true);
            saveTimelineDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            saveTimelineDialog.ensureVisible();
            fileMenu.setVisibleSmooth(false);
        }

        void saveTimeline(Timeline timeline, String filename)
        {
            timelineController.saveTimeline(timeline, filename);
            changeTimelineFile(filename);
        }

        void saveTimelineDialog_SaveFile(object sender, EventArgs e)
        {
            String filename = saveTimelineDialog.Filename;
            if (timelineController.resourceExists(filename))
            {
                MessageBox.show(String.Format("The file {0} already exists. Would you like to overwrite it?", filename), "Overwrite?", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, overwriteTimelineResult);
            }
            else
            {
                saveTimeline(currentTimeline, filename);
            }
        }

        void overwriteTimelineResult(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                saveTimeline(currentTimeline, saveTimelineDialog.Filename);
            }
        }

        void saveTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            if (currentTimelineFile != null)
            {
                timelineController.saveTimeline(currentTimeline, currentTimelineFile);
                fileMenu.setVisibleSmooth(false);
            }
            else
            {
                saveTimelineAs_MouseButtonClick(source, e);
            }
        }

        void openTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            fileBrowserDialog.promptForFile("*.tl", openTimelineDialog_OpenFile);
            fileBrowserDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            fileBrowserDialog.ensureVisible();
            fileMenu.setVisibleSmooth(false);
        }

        void openTimelineDialog_OpenFile(String filename)
        {
            try
            {
                setCurrentTimeline(timelineController.openTimeline(filename));
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error loading timeline {0}.\n{1}", filename, ex.Message), "Load Timeline Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        void newTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            createNewTimeline();
            fileMenu.setVisibleSmooth(false);
        }

        void timelineController_ResourceLocationChanged(object sender, EventArgs e)
        {
            setEnabled(timelineController.ResourceProvider != null);
        }

        private void setEnabled(bool enabled)
        {
            newTimelineItem.Enabled = enabled;
            openTimelineItem.Enabled = enabled;
            saveTimelineItem.Enabled = enabled;
            saveTimelineAsItem.Enabled = enabled;
            actionFilter.Enabled = enabled;
            otherActionsMenuButton.Enabled = enabled;
            editMenuButton.Enabled = enabled;
            playButton.Enabled = enabled;
            timelineView.Enabled = enabled;
            fastForwardButton.Enabled = enabled;
            rewindButton.Enabled = enabled;
            analyzeMenuButton.Enabled = enabled;
        }

        #endregion

        #region Edit Menu

        void paste_MouseButtonClick(Widget source, EventArgs e)
        {
            if (copySourceAction != null)
            {
                TimelineAction copiedAction = copySaver.copy<TimelineAction>(copySourceAction);
                copiedAction.StartTime = timelineView.MarkerTime;
                currentTimeline.addAction(copiedAction);
                timelineView.CurrentData = actionDataBindings[copiedAction];
            }
            editMenu.setVisibleSmooth(false);
        }

        void copy_MouseButtonClick(Widget source, EventArgs e)
        {
            TimelineActionData currentData = timelineView.CurrentData as TimelineActionData;
            if (currentData != null)
            {
                copySourceAction = copySaver.copy<TimelineAction>(currentData.Action);
            }
            editMenu.setVisibleSmooth(false);
        }

        #endregion Edit Menu

        #region Other Actions Menu

        void editTimelineIndex_MouseButtonClick(Widget source, EventArgs e)
        {
            timelineIndexEditor.setData(timelineController.CurrentTimelineIndex);
            timelineIndexEditor.open(true);
            timelineIndexEditor.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            timelineIndexEditor.ensureVisible();
        }

        void timelineIndexEditor_SaveIndexData(object sender, EventArgs e)
        {
            TimelineIndex index = timelineIndexEditor.createIndex();
            timelineController.saveIndex(index);
        }

        void testActions_MouseButtonClick(Widget source, EventArgs e)
        {
            testActions.StateCheck = !testActions.StateCheck;
        }

        void changeScene_MouseButtonClick(Widget source, EventArgs e)
        {
            changeSceneEditor.open(true);
            changeSceneEditor.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            changeSceneEditor.ensureVisible();
        }

        void reverseSidesAction_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Reversing sides will attempt to help you make a timeline that works on the opposite side.\nIt can only reverse things on the x-axis meaning it will reverse stuff left to right.\n\nThe only things that can be reversed are:\n* Camera translation and look at.\n* Prop translation (rotations need to be fixed manually).\n* Movement sequence keyframes.", "Reverse", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
            timelineController.EditingTimeline.reverseSides();
        }

        void finishAction_MouseButtonClick(Widget source, EventArgs e)
        {
            finishActionEditor.open(true);
            finishActionEditor.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            finishActionEditor.ensureVisible();
        }

        #endregion

        #region AnalyzeMenu

        void dumpPostActions_MouseButtonClick(Widget source, EventArgs e)
        {
            Log.Debug("");
            Log.Debug("");
            Log.Debug("Dumping Timeline Post Actions");
            String[] files = timelineController.listResourceFiles("*.tl");
            foreach(String file in files)
            {
                Log.Debug("-----------------------------------------------------------");
                Timeline tl = timelineController.openTimeline(file);
                Log.Debug("Dumping post actions for timeline \"{0}\".", tl.SourceFile);
                tl.dumpPostActionsToLog();
                analyzeMenu.setVisibleSmooth(false);
                Log.Debug("-----------------------------------------------------------");
                Log.Debug("");
            }
            Log.Debug("");
            Log.Debug("");
        }

        #endregion

        #region Playback

        void rewindButton_MouseButtonClick(Widget source, EventArgs e)
        {
            timelineView.MarkerTime = 0.0f;
        }

        void fastForwardButton_MouseButtonClick(Widget source, EventArgs e)
        {
            timelineView.MarkerTime += 10.0f;
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback(false);
            }
            else if (currentTimeline != null)
            {
                timelineView.CurrentData = null;
                timelineController.startPlayback(currentTimeline, timelineView.MarkerTime, testActions.StateCheck);
            }
        }

        void timelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            playButton.Caption = "Play";
            playButton.StaticImage.setItemResource("Timeline/PlayIcon");
            rewindButton.Enabled = true;
            fastForwardButton.Enabled = true;
        }

        void timelineController_TimelinePlaybackStarted(object sender, EventArgs e)
        {
            playButton.Caption = "Stop";
            playButton.StaticImage.setItemResource("Timeline/StopIcon");
            rewindButton.Enabled = false;
            fastForwardButton.Enabled = false;
            if (timelineController.ActiveTimeline != currentTimeline)
            {
                setCurrentTimeline(timelineController.ActiveTimeline);
            }
        }

        void timelineController_TimeTicked(float currentTime)
        {
            timelineView.MarkerTime = currentTime;
        }

        #endregion
    }
}
