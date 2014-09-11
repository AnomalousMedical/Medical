using BulletPlugin;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class is a bit of a hack to allow Bullet to sleep. The teeth will register themselves here,
    /// the MuscleBehavior will poll the IsSleepy function in its update function to see if it should stop
    /// applying its forces. When this happens because all the teeth want deactivation the muscle object will
    /// also deactivate and bullet will go to sleep. This will have huge battery life and heat improvements
    /// for portable computers as the Bullet calculations are very expensive and are not needed most of the time
    /// since the mouth is just sitting closed.
    /// 
    /// Don't make this public if it can be avoided. Just have the various mandible manipulating classes call wakeUp
    /// or directly activate their actors when appropriate.
    /// </summary>
    public static class SleepyActorRepository
    {
        private static List<RigidBody> sleepers = new List<RigidBody>();
        private static bool sleeping = false;

        public static void SceneLoaded(SimScene scene)
        {
            var bulletScene = scene.getDefaultSubScene().getSimElementManager<BulletScene>();
            if (bulletScene != null)
            {
                bulletScene.OnRigidBodyAdded += bulletScene_OnRigidBodyAdded;
                bulletScene.OnRigidBodyRemoved += bulletScene_OnRigidBodyRemoved;
            }
        }

        public static void SceneUnloading(SimScene scene)
        {
            var bulletScene = scene.getDefaultSubScene().getSimElementManager<BulletScene>();
            if (bulletScene != null)
            {
                bulletScene.OnRigidBodyAdded -= bulletScene_OnRigidBodyAdded;
                bulletScene.OnRigidBodyRemoved -= bulletScene_OnRigidBodyRemoved;
            }
        }

        /// <summary>
        /// Add a sleeper to this repository.
        /// </summary>
        /// <param name="sleeper"></param>
        internal static void addSleeper(RigidBody sleeper)
        {
            sleepers.Add(sleeper);
        }

        /// <summary>
        /// Remove a sleeper from this repository.
        /// </summary>
        /// <param name="sleeper"></param>
        internal static void removeSleeper(RigidBody sleeper)
        {
            sleepers.Remove(sleeper);
        }

        /// <summary>
        /// Wake up all the regisered sleepers, you can call this a bunch of times if needed, it will track
        /// internally if it really needs to wake stuff up.
        /// </summary>
        internal static void wakeUp()
        {
            if (sleeping)
            {
                //Logging.Log.Debug("Woke up sleepers");
                foreach (var sleeper in sleepers)
                {
                    sleeper.activate(false);
                }
                sleeping = false;
            }
        }

        /// <summary>
        /// Returns true if all objects in this repository are not in the ActivationState.ActiveTag status. This really
        /// needs to be polled by something, most likely classes that that apply forces to the physics scene without
        /// really thinking about the other objects. Bullet is pretty good about putting stuff to sleep that it totally
        /// controls.
        /// </summary>
        internal static bool IsSleepy
        {
            get
            {
                //Find the first thing that is not set with the active tag, if this is null everything is not the active tag status.
                //Note that there are lots of states besides active, but by checking for active we can quickly fail this loop in the
                //event that stuff is active, which will be most of the time if we are already cpu heavy aka simulating.
                //For anomalous medical this is realisticly 32 teeth so it probably doesn't really matter either way.
                sleeping = sleepers.FirstOrDefault(s => s.getActivationState() == ActivationState.ActiveTag) == null;
                return sleeping;
            }
        }

        private static void bulletScene_OnRigidBodyAdded(BulletScene bulletScene, RigidBody rigidBody)
        {
            addSleeper(rigidBody);
            wakeUp();
        }

        private static void bulletScene_OnRigidBodyRemoved(BulletScene bulletScene, RigidBody rigidBody)
        {
            removeSleeper(rigidBody);
            wakeUp();
        }
    }
}
