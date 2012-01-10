using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.GUI
{
    class BrowserBuilderFactory : DataControlFactory
    {
        private Browser browser;
        private TimelineController timelineController;
        private String currentPath;
        private static readonly String[] DELIM = { "." };

        public BrowserBuilderFactory(Browser browser, TimelineController timelineController)
        {
            this.browser = browser;
            this.timelineController = timelineController;
        }

        public void pushColumnLayout()
        {
            
        }

        public void popColumnLayout()
        {
            
        }

        public void addField(BooleanDataField field)
        {
            browser.addNode(currentPath, DELIM, new BrowserNode(field.Name, field));
        }

        public void addField(MenuItemField field)
        {
            String lastCurrentPath = currentPath;
            foreach(TimelineEntry entry in field.Timelines)
            {
                Timeline timeline = timelineController.openTimeline(entry.Timeline);
                if (timeline != null)
                {
                    if (String.IsNullOrEmpty(lastCurrentPath))
                    {
                        currentPath = entry.Name;
                    }
                    else
                    {
                        currentPath = String.Format("{0}.{1}", lastCurrentPath, entry.Name);
                    }
                    DataDrivenTimelineGUIData timelineGUIData = DataDrivenTimelineGUIData.FindDataInTimeline(timeline);
                    if (timelineGUIData != null)
                    {
                        timelineGUIData.createControls(this);
                    }
                }
            }
            currentPath = lastCurrentPath;
        }

        public void addField(MultipleChoiceField field)
        {
            browser.addNode(currentPath, DELIM, new BrowserNode(field.Name, field));
        }

        public void addField(NotesDataField field)
        {
            browser.addNode(currentPath, DELIM, new BrowserNode(field.Name, field));
        }

        public void addField(NumericDataField field)
        {
            browser.addNode(currentPath, DELIM, new BrowserNode(field.Name, field));
        }

        public void addField(PlayExampleDataField field)
        {
            
        }
    }
}
