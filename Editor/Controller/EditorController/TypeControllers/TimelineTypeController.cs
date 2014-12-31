﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using System.Xml;
using MyGUIPlugin;
using Logging;
using Medical.Controller.AnomalousMvc;
using Engine.Platform;

namespace Medical
{
    public class TimelineTypeController : SaveableTypeController<Timeline>
    {
        public const String Icon = "EditorFileIcon/.tl";

        public TimelineTypeController(EditorController editorController)
            :base(".tl", editorController)
        {
            
        }

        public override ProjectItemTemplate createItemTemplate()
        {
            return new ProjectItemTemplateDelegate("Timeline", Icon, "File", delegate(String path, String fileName, EditorController editorController)
            {
                String filePath = Path.Combine(path, fileName);
                createTimelineFileSafely(filePath);
            });
        }

        public String createTimelineFileSafely(String filePath)
        {
            filePath = Path.ChangeExtension(filePath, ".tl");
            if (EditorController.ResourceProvider.exists(filePath))
            {
                MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                {
                    if (overrideResult == MessageBoxStyle.Yes)
                    {
                        createNewTimeline(filePath);
                    }
                });
            }
            else
            {
                createNewTimeline(filePath);
            }
            return filePath;
        }

        void createNewTimeline(String filePath)
        {
            Timeline timeline = new Timeline();
            creatingNewFile(filePath);
            saveObject(filePath, timeline);
            openEditor(filePath);
        }

        public void saveTimeline(Timeline timeline, String filename)
        {
            try
            {
                saveObject(filename, timeline);
                EditorController.NotificationManager.showNotification(String.Format("{0} saved.", filename), Icon, 2);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving your timeline to\n'{0}'\nPlease make sure that destination is valid.", filename), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Log.Error("Could not save timeline. {0}", ex.Message);
            }
        }
    }
}
