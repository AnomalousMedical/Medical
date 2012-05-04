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
        private bool queuedCloseView = false;
        private String queuedTimeline = null;
        private String queuedShowView = null;

        public AnomalousMvcContext()
        {
            controllers = new ControllerCollection();
            views = new ViewCollection();
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
            queuedCloseView = false;
            queuedTimeline = null;
            queuedShowView = null;

            int slashLoc = address.IndexOf('/');
            String controllerName = address.Substring(0, slashLoc);
            ++slashLoc;
            String actionName = address.Substring(slashLoc, address.Length - slashLoc);
            Controller controller = controllers[controllerName];
            controller.runAction(actionName, this);

            if (queuedCloseView)
            {
                if (queuedTimeline == null)
                {
                    core.closeView();
                    if (queuedShowView == null)
                    {
                        //No new timeline and no new view, shutdown.
                        core.returnToMainGui();
                    }
                }
                else
                {
                    core.closeView();
                    playTimeline(queuedTimeline);
                }
            }
            else if (queuedTimeline != null)
            {
                playTimeline(queuedTimeline);
            }
            if (queuedShowView != null)
            {
                if (!queuedCloseView)
                {
                    core.closeView();
                }
                core.showView(views[queuedShowView], this);
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

        public void queueShowView(String view)
        {
            queuedShowView = view;
        }

        public void queueClose()
        {
            queuedCloseView = true;
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
                editInterface.addSubInterface(views.getEditInterface("Views"));
                editInterface.addSubInterface(controllers.getEditInterface("Controllers"));
            }
            return editInterface;
        }
    }
}
