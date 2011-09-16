using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Engine.Attributes;

namespace Medical.GUI
{
    public partial class TimelineWizardPanelData : AbstractTimelineGUIData
    {
        [DoNotSave]
        private List<TimelineEntry> timelines = new List<TimelineEntry>();

        private bool attachToScrollView = true;

        public TimelineWizardPanelData()
        {

        }

        public void addTimeline(String name, String timeline, String imageKey)
        {
            TimelineEntry entry = new TimelineEntry(name, timeline, imageKey);
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

        [Editable]
        public bool AttachToScrollView
        {
            get
            {
                return attachToScrollView;
            }
            set
            {
                attachToScrollView = value;
            }
        }

        public IEnumerable<TimelineEntry> Timelines
        {
            get
            {
                return timelines;
            }
        }

        public bool HasTimelineLinks
        {
            get
            {
                return timelines.Count > 0;
            }
        }

        protected TimelineWizardPanelData(LoadInfo info)
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

    partial class TimelineWizardPanelData
    {
        [DoNotSave]
        [DoNotCopy]
        private EditInterface timelinesEditInterface;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            timelinesEditInterface = new EditInterface("Timelines", addReferenceCallback, removeReferenceCallback, validate);

            EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
            propertyInfo.addColumn(new EditablePropertyColumn("Name", false));
            propertyInfo.addColumn(new EditablePropertyColumn("Timeline", false));
            propertyInfo.addColumn(new EditablePropertyColumn("Icon", false));
            timelinesEditInterface.setPropertyInfo(propertyInfo);
            editInterface.addSubInterface(timelinesEditInterface);
            foreach (TimelineEntry entry in timelines)
            {
                timelinesEditInterface.addEditableProperty(entry);
            }
        }

        private void addReferenceCallback(EditUICallback callback)
        {
            addTimeline("", "", "");
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
