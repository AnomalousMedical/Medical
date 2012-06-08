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
        TimelineController timelineController;
        EditorController editorController;
        SaveableClipboard clipboard;
        EditorPlugin editorPlugin;

        public TimelineComponentFactory(TimelineController timelineController, EditorController editorController, SaveableClipboard clipboard, EditorPlugin editorPlugin)
        {
            this.timelineController = timelineController;
            this.editorController = editorController;
            this.clipboard = clipboard;
            this.editorPlugin = editorPlugin;
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is TimelineEditorView)
            {
                TimelineEditorView editorView = (TimelineEditorView)view;
                TimelineEditorComponent timelineEditor = new TimelineEditorComponent(viewHost, timelineController, editorController, clipboard, editorPlugin);
                timelineEditor.CurrentTimeline = editorView.Timeline;
                editorView._fireComponentCreated(timelineEditor);
                return timelineEditor;
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
