using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using Medical.GUI;

namespace Developer
{
    class DeveloperAtlasPlugin : AtlasPlugin
    {
        private ButtonCreator buttonCreator;
        private DummyTimeline dummyTimeline;

        public DeveloperAtlasPlugin(StandaloneController standaloneController)
        {

        }

        public void Dispose()
        {
            if (buttonCreator != null)
            {
                buttonCreator.Dispose();
                dummyTimeline.Dispose();
            }
        }

        public void createMenuBar(NativeMenuBar menu)
        {

        }

        public void initialize(StandaloneController standaloneController)
        {
            //Find the prototypes with reflection. Could put a manual list here too.
            standaloneController.TimelineGUIFactory.findPrototypes(GetType().Assembly);

            buttonCreator = new ButtonCreator();
            standaloneController.GUIManager.addManagedDialog(buttonCreator);

            dummyTimeline = new DummyTimeline();
            standaloneController.GUIManager.addManagedDialog(dummyTimeline);

            standaloneController.TaskController.addTask(new MDIDialogOpenTask(buttonCreator, "Developer.ButtonCreator", "Button Creator", "", TaskMenuCategories.Developer));
            standaloneController.TaskController.addTask(new MDIDialogOpenTask(dummyTimeline, "Developer.DummyTimeline", "Dummy Timeline", "", TaskMenuCategories.Developer));
        }

        public void sceneLoaded(SimScene scene)
        {

        }

        public void sceneRevealed()
        {

        }

        public void sceneUnloading(SimScene scene)
        {

        }

        public void setMainInterfaceEnabled(bool enabled)
        {

        }
    }
}
