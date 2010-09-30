using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class OpenNewSceneAction : TimelineInstantAction
    {
        public OpenNewSceneAction()
        {

        }

        public OpenNewSceneAction(String scene)
        {
            this.Scene = scene;
        }

        public override void doAction()
        {
            TimelineController.openNewScene(Scene);
        }

        public String Scene { get; set; }
    }
}
