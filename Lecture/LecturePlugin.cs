﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.ObjectManagement;
using MyGUIPlugin;

namespace Medical
{
    class LecturePlugin : AtlasPlugin
    {
        private CloneWindowDialog cloneWindowDialog;
        private StandaloneController standaloneController;

        public LecturePlugin()
        {

        }

        public void Dispose()
        {
            cloneWindowDialog.Dispose();
        }

        public void initialize(StandaloneController standaloneController)
        {
            Gui.Instance.load("Medical.Resources.LectureIcons.xml");

            this.standaloneController = standaloneController;

            //Create dialogs
            cloneWindowDialog = new CloneWindowDialog();

            //Taskbar
            standaloneController.TaskController.addTask(new CloneWindowTask(standaloneController, cloneWindowDialog));
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        public void sceneRevealed()
        {
            
        }

        public long PluginId
        {
            get
            {
                return -1;
            }
        }

        public String PluginName
        {
            get
            {
                return "Lecture Plugin";
            }
        }
    }
}
