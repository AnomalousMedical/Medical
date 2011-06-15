using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using ExamExample.GUI;

namespace ExamExample
{
    class ExamExampleAtlasPlugin : AtlasPlugin
    {
        public ExamExampleAtlasPlugin(StandaloneController standaloneController)
        {

        }

        public void Dispose()
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            //Add the ExampleGUIPrototype to the TimelineGUIFactory.
            standaloneController.TimelineGUIFactory.addPrototype(new ExampleGUIPrototype());
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
