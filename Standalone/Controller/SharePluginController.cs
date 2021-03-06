﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public enum PluginCreationTool
    {
        Unknown = 0,
        SmartLectureTools = 1,
        EditorTools = 2
    }

    public class SharePluginController
    {
        Action<ResourceProvider, PluginCreationTool> shareCallback;

        public SharePluginController(Action<ResourceProvider, PluginCreationTool> shareCallback)
        {
            this.shareCallback = shareCallback;
        }

        public void sharePlugin(ResourceProvider source, PluginCreationTool tool)
        {
            shareCallback(source, tool);
        }

        public string Name { get; set; }

        public string IconName { get; set; }

        public string Category { get; set; }
    }
}
