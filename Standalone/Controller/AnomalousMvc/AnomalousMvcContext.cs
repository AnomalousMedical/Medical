using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public partial class AnomalousMvcContext : Saveable
    {
        private ControllerCollection controllers;
        private ViewCollection views;
        private AnomalousMvcCore core;

        //Action queue stuff
        private bool queuedCloseGui = false;
        private String queuedTimeline = null;

        public AnomalousMvcContext()
        {
            controllers = new ControllerCollection();
            views = new ViewCollection();
        }

        public void showView(String view)
        {
            core.showView(views[view], this);
        }

        public void stopTimelines()
        {
            core.stopTimelines();
        }

        public void stopPlayingExample()
        {
            core.stopPlayingExample();
        }

        public void playTimeline(String timelineName)
        {
            core.playTimeline(timelineName, true);
        }

        public void playTimeline(String timelineName, bool allowPlaybackStop)
        {
            core.playTimeline(timelineName, allowPlaybackStop);
        }

        public void runAction(string address)
        {
            queuedCloseGui = false;
            queuedTimeline = null;

            int slashLoc = address.IndexOf('/');
            String controllerName = address.Substring(0, slashLoc);
            ++slashLoc;
            String actionName = address.Substring(slashLoc, address.Length - slashLoc);
            Controller controller = controllers[controllerName];
            controller.runAction(actionName, this);

            if (queuedCloseGui)
            {
                //if (queuedTimeline == null)
                //{
                //    closeAndReturnToMainGUI();
                //}
                //else
                //{
                //    core.
                //    closeAndPlayTimeline(queuedTimeline);
                //}
            }
            else if (queuedTimeline != null)
            {
                playTimeline(queuedTimeline);
            }
        }

        public string getFullPath(string file)
        {
            return core.getFullPath(file);
        }

        public void queueTimeline(string timeline)
        {
            queuedTimeline = timeline;
        }

        public void queueClose()
        {
            throw new NotImplementedException();
        }

        public void applyLayers(EditableLayerState layers)
        {
            core.applyLayers(layers);
        }

        public void applyPresetState(PresetState presetState, float duration)
        {
            core.applyPresetState(presetState, duration);
        }

        public void applyCameraPosition(CameraPosition cameraPosition)
        {
            core.applyCameraPosition(cameraPosition);
        }

        internal void _setCore(AnomalousMvcCore core)
        {
            this.core = core;
        }

        protected AnomalousMvcContext(LoadInfo info)
        {
            controllers = info.GetValue<ControllerCollection>("Controllers");
            views = info.GetValue<ViewCollection>("Views");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Controllers", controllers);
            info.AddValue("Views", views);
        }
    }

    public partial class AnomalousMvcContext
    {
        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("MVC");
                editInterface.addSubInterface(controllers.getEditInterface());
                editInterface.addSubInterface(views.getEditInterface());
            }
            return editInterface;
        }
    }
}
