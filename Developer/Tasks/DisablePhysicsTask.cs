using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using BulletPlugin;
using Engine.ObjectManagement;

namespace Developer
{
    class DisablePhysicsTask : Task
    {
        private BulletScene bulletScene = null;

        public DisablePhysicsTask(int weight)
            : base("Developer.DisablePhysicsTask", "Disable Physics", "Developer.DisablePhysics", "Developer")
        {
            this.Weight = weight;
        }

        public void sceneChanged(SimScene scene)
        {
            bulletScene = scene.getDefaultSubScene().getSimElementManager<BulletScene>();
        }

        public override void clicked(TaskPositioner positioner)
        {
            if (bulletScene != null)
            {
                ShowOnTaskbar = bulletScene.Active = !bulletScene.Active;
                if (!bulletScene.Active)
                {
                    fireItemClosed();
                }
            }
        }

        public override bool Active
        {
            get { return bulletScene != null ? bulletScene.Active : false; }
        }
    }
}
