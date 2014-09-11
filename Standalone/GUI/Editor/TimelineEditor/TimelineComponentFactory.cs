using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Editing;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    class TimelineComponentFactory : ViewHostComponentFactory
    {
        SaveableClipboard clipboard;

        public TimelineComponentFactory(SaveableClipboard clipboard)
        {
            this.clipboard = clipboard;
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is TimelineEditorView)
            {
                TimelineEditorView editorView = (TimelineEditorView)view;
                TimelineEditorComponent timelineEditor = new TimelineEditorComponent(viewHost, editorView, clipboard);
                timelineEditor.CurrentTimeline = editorView.Timeline;
                editorView.fireComponentCreated(timelineEditor);
                return timelineEditor;
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
