using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using Engine.Saving;
using Engine.Editing;
using Engine;

namespace Medical
{
    /// <summary>
    /// A TimelineInstantAction to show a GUI.
    /// </summary>
    public class ShowTimelineGUIAction : TimelineInstantAction
    {
        TimelineGUI gui;
        private TimelineController timelineControllerAfterDoAction; //This will be stored in doAction because it will go away when the timeline stops
        private String guiName;
        private TimelineGUIData guiData;
        private EditInterface editInterface;

        public ShowTimelineGUIAction()
        {

        }

        public void stopTimelines()
        {
            if (timelineControllerAfterDoAction.Playing)
            {
                timelineControllerAfterDoAction.stopPlayback();
            }
            timelineControllerAfterDoAction._fireMultiTimelineStopEvent();
        }

        public void playTimeline(String timelineName)
        {
            this.playTimeline(timelineName, true);
        }

        public void playTimeline(String timelineName, bool allowPlaybackStop)
        {
            Timeline timeline = timelineControllerAfterDoAction.openTimeline(timelineName);
            if (timeline != null)
            {
                timeline.AutoFireMultiTimelineStopped = allowPlaybackStop;
                if (timelineControllerAfterDoAction.Playing)
                {
                    timelineControllerAfterDoAction.queueTimeline(timeline);
                    timelineControllerAfterDoAction.stopPlayback();
                }
                else
                {
                    timelineControllerAfterDoAction.startPlayback(timeline);
                }
            }
            else
            {
                Log.Warning("ShowGUIAction playback: Error loading timeline '{0}'", timelineName);
                stopTimelines();
            }
        }

        public override void doAction()
        {
            Timeline.AutoFireMultiTimelineStopped = false;
            gui = TimelineController.GUIFactory.getGUI(GUIName);
            if (gui != null)
            {
                timelineControllerAfterDoAction = TimelineController;
                gui.initialize(this);
                gui.show(TimelineController.GUIManager);
            }
            else
            {
                TimelineController._fireMultiTimelineStopEvent();
            }
        }

        public override void dumpToLog()
        {
            Log.Debug("ShowTimelineGUIAction GUI: '{0}'", GUIName);
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        public String GUIName
        {
            get
            {
                return guiName;
            }
            set
            {
                guiName = value;
            }
        }

        public TimelineGUIData GUIData
        {
            get
            {
                return guiData;
            }
            set
            {
                if (editInterface != null && guiData != null)
                {
                    editInterface.removeSubInterface(guiData.getEditInterface());
                }
                guiData = value;
                if (editInterface != null && guiData != null)
                {
                    editInterface.addSubInterface(guiData.getEditInterface());
                }
            }
        }

        public enum CustomEditQueries
        {
            ChangeGUIType,
            GetGUIData,
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            this.editInterface = editInterface;
            base.customizeEditInterface(editInterface);
            if (guiData != null)
            {
                editInterface.addSubInterface(guiData.getEditInterface());
            }
            editInterface.addCommand(new EditInterfaceCommand("Change GUI Type", changeGUIType));
            editInterface.addEditableProperty(new ShowTimelineGUIActionGUINameProperty(this));
        }

        private void changeGUIType(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.runCustomQuery(CustomEditQueries.ChangeGUIType, delegate(Object result, ref String errorMessage)
            {
                GUIName = result.ToString();
                callback.runCustomQuery(CustomEditQueries.GetGUIData, setGUIData, guiName);
                return true;
            });
        }

        private bool setGUIData(Object guiData, ref String errorMessage)
        {
            GUIData = (TimelineGUIData)guiData;
            return true;
        }

        #region Saving

        protected ShowTimelineGUIAction(LoadInfo info)
            : base(info)
        {
            guiName = info.GetString("GUIName", null);
            guiData = info.GetValue<TimelineGUIData>("GUIData", null);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("GUIName", guiName);
            info.AddValue("GUIData", guiData);
        }

        #endregion
    }

    /// <summary>
    /// This class shows the name of the chosen UI on the edit sheet as read only.
    /// </summary>
    class ShowTimelineGUIActionGUINameProperty : EditableProperty
    {
        private ShowTimelineGUIAction action;

        public ShowTimelineGUIActionGUINameProperty(ShowTimelineGUIAction action)
        {
            this.action = action;
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }

        public Type getPropertyType(int column)
        {
            return typeof(String);
        }

        public string getValue(int column)
        {
            switch (column)
            {
                case 0:
                    return "GUIName";
                case 1:
                    return action.GUIName;
            }
            return null;
        }

        public void setValueStr(int column, string value)
        {
            
        }
    }
}
