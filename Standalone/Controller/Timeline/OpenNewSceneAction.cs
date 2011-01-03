using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    public class OpenNewSceneAction : TimelineInstantAction
    {
        public static readonly String Name = "Change Scene";

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

        #region Saveable

        protected static readonly String SCENE = "Scene";

        protected OpenNewSceneAction(LoadInfo info)
            :base(info)
        {
            Scene = info.GetString(SCENE, "");
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(SCENE, Scene);
        }

        #endregion
    }
}
