using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine.Attributes;
using Engine.Saving;

namespace Medical
{
    public partial class MenuItemField : DataField
    {
        [DoNotSave]
        private List<TimelineEntry> timelines = new List<TimelineEntry>();

        public MenuItemField(String name)
            :base(name)
        {

        }

        public void addTimeline(String timeline)
        {
            TimelineEntry entry = new TimelineEntry(timeline);
            timelines.Add(entry);
            onTimelineAdded(entry);
        }

        public void removeTimeline(String timeline)
        {
            TimelineEntry entry = null;
            foreach (TimelineEntry currentEntry in timelines)
            {
                if (currentEntry.Timeline == timeline)
                {
                    entry = currentEntry;
                    break;
                }
            }
            if (entry != null)
            {
                timelines.Remove(entry);
                onTimelineRemoved(entry);
            }
        }

        public DataDrivenNavigationState createNavigationState(String menuTimeline)
        {
            DataDrivenNavigationState navState = new DataDrivenNavigationState(menuTimeline);
            foreach (TimelineEntry entry in timelines)
            {
                navState.addTimeline(entry);
            }
            return navState;
        }

        public override DataControl createControl(Widget parentWidget, DataDrivenTimelineGUI gui)
        {
            return new MenuButtonDataControl(parentWidget, gui, this);
        }

        public override string Type
        {
            get { return "Menu Item"; }
        }

        protected MenuItemField(LoadInfo info)
            :base(info)
        {
            info.RebuildList<TimelineEntry>("Timeline", timelines);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<TimelineEntry>("Timeline", timelines);
        }
    }

    public partial class MenuItemField
    {
        [DoNotSave]
        [DoNotCopy]
        private EditInterface timelinesEditInterface;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            timelinesEditInterface = new EditInterface("Timelines", addReferenceCallback, removeReferenceCallback, validate);
            
            EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
            propertyInfo.addColumn(new EditablePropertyColumn("Timeline", false));
            timelinesEditInterface.setPropertyInfo(propertyInfo);
            editInterface.addSubInterface(timelinesEditInterface);
            foreach (TimelineEntry entry in timelines)
            {
                editInterface.addEditableProperty(entry);
            }
        }

        private void addReferenceCallback(EditUICallback callback)
        {
            addTimeline("");
        }

        private void removeReferenceCallback(EditUICallback callback, EditableProperty property)
        {
            TimelineEntry timeline = property as TimelineEntry;
            if (timeline != null)
            {
                timelines.Remove(timeline);
                onTimelineRemoved(timeline);
            }
        }

        private bool validate(out String message)
        {
            message = null;
            return true;
        }

        private void onTimelineAdded(TimelineEntry entry)
        {
            if (timelinesEditInterface != null)
            {
                timelinesEditInterface.addEditableProperty(entry);
            }
        }

        private void onTimelineRemoved(TimelineEntry entry)
        {
            if (timelinesEditInterface != null)
            {
                timelinesEditInterface.removeEditableProperty(entry);
            }
        }
    }
}
