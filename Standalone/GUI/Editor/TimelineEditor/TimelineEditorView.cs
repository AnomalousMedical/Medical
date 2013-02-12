﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    public class TimelineEditorView : MyGUIView
    {
        public TimelineEditorView(String name, Timeline timeline, TimelineController timelineController, EditorController editorController, PropEditController propEditController)
            :base(name)
        {
            ViewLocation = Controller.AnomalousMvc.ViewLocations.Bottom;
            IsWindow = true;
            this.Timeline = timeline;
            this.TimelineController = timelineController;
            this.EditorController = editorController;
            this.PropEditController = propEditController;
        }

        public Timeline Timeline { get; set; }

        public TimelineController TimelineController { get; set; }

        public EditorController EditorController { get; set; }

        public PropEditController PropEditController { get; set; }

        protected TimelineEditorView(LoadInfo info)
            :base(info)
        {

        }
    }
}