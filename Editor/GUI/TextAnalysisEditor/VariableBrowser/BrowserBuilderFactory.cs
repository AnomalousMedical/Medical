using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.GUI
{
    /// <summary>
    /// Use the mechansim to create guis from the data to build an object
    /// browser that will allow the user to choose variables from a menu.
    /// </summary>
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
            browser.addNode(currentPath, DELIM, new BrowserNode(field.Name, new DataFieldInfo(currentPath, field.Name)));
        }

        public void addField(MenuItemField field)
        {
            String lastCurrentPath = currentPath;
            String basePath = null;
            if (String.IsNullOrEmpty(lastCurrentPath))
            {
                basePath = String.Format("{0}.{{0}}", field.Name);
            }
            else
            {
                basePath = String.Format("{0}.{1}.{{0}}", lastCurrentPath, field.Name);
            }
            foreach(TimelineEntry entry in field.Timelines)
            {
                Timeline timeline = timelineController.openTimeline(entry.Timeline);
                if (timeline != null)
                {
                    currentPath = String.Format(basePath, entry.Name);
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
            browser.addNode(currentPath, DELIM, new BrowserNode(field.Name, new DataFieldInfo(currentPath, field.Name)));
        }

        public void addField(NotesDataField field)
        {
            browser.addNode(currentPath, DELIM, new BrowserNode(field.Name, new DataFieldInfo(currentPath, field.Name)));
        }

        public void addField(NumericDataField field)
        {
            browser.addNode(currentPath, DELIM, new BrowserNode(field.Name, new DataFieldInfo(currentPath, field.Name)));
        }

        public void addField(PlayExampleDataField field)
        {
            
        }

        public void addField(StaticTextDataField field)
        {

        }

        public void addField(CloseGUIPlayTimelineField closeGUIPlayTimelineField)
        {
            
        }

        public void addField(DoActionsDataField doActionsDataField)
        {

        }
    }
}
