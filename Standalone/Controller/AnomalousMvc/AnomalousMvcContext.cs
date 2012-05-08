using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.Editing;
using Engine.Saving;
using Medical.Editor;
using Engine.Attributes;
using Logging;
using Medical.Model;

namespace Medical.Controller.AnomalousMvc
{
    public partial class AnomalousMvcContext : Saveable
    {
        private ControllerCollection controllers;
        private ViewCollection views;
        private AnomalousMvcCore core;
        private ResourceProvider resourceProvider;

        //State recording stuff
        private ModelMemory modelMemory = new ModelMemory();

        //Action queue stuff
        private bool queuedCloseView = false;
        private String queuedTimeline = null;
        private String queuedShowView = null;
        private bool allowShutdown = true;

        public AnomalousMvcContext()
        {
            controllers = new ControllerCollection();
            views = new ViewCollection();
            StartupAction = "Common/Start";
            ShutdownAction = "Common/Shutdown";
        }

        public void stopPlayingTimeline()
        {
            core.stopPlayingTimeline();
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

            doRunAction(address);

            if (queuedCloseView)
            {
                core.closeView();
            }
            if (queuedTimeline != null)
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

            checkShutdownConditions();
        }

        public string getFullPath(string file)
        {
            return resourceProvider.getFullFilePath(file);
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

        public void setResourceProvider(ResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
        }

        public void saveCamera(String name)
        {
            CameraPosition cameraPosition = core.getCurrentCameraPosition();
            if (cameraPosition != null)
            {
                modelMemory.add(name, cameraPosition);
            }
            else
            {
                Log.Warning("Problem saving camera position for '{0}'. A position could not be generated.", name);
            }
        }

        public void restoreCamera(String name)
        {
            CameraPosition cameraPos = modelMemory.get<CameraPosition>(name);
            if (cameraPos != null)
            {
                core.applyCameraPosition(cameraPos);
            }
            else
            {
                Log.Warning("A camera named '{0}' cannot be found to restore.", name);
            }
        }

        public void saveLayers(String name)
        {
            LayerState layerState = new LayerState(name);
            layerState.captureState();
            modelMemory.add(name, layerState);
        }

        public void restoreLayers(String name)
        {
            LayerState layers = modelMemory.get<LayerState>(name);
            if (layers != null)
            {
                core.applyLayers(layers);
            }
            else
            {
                Log.Warning("A layer state named '{0}' cannot be found to restore.", name);
            }
        }

        public void saveMedicalState(String name)
        {
            MedicalState medicalState = core.generateMedicalState();
            modelMemory.add(name, medicalState);
        }

        public void restoreMedicalState(String name)
        {
            MedicalState medicalState = modelMemory.get<MedicalState>(name);
            if (medicalState != null)
            {
                core.applyMedicalState(medicalState);
            }
            else
            {
                Log.Warning("A medical state named '{0}' cannot be found to restore.", name);
            }
        }

        public void createMedicalState()
        {
            WizardStateInfo stateInfo = modelMemory.get<WizardStateInfo>(WizardStateInfo.ModelMemoryName);
            if (stateInfo == null)
            {
                stateInfo = new WizardStateInfo();
            }
            core.createMedicalState(stateInfo);
        }

        public void addModel(String name, Object model)
        {
            modelMemory.add(name, model);
        }

        [EditableAction]
        public String StartupAction { get; set; }

        [EditableAction]
        public String ShutdownAction { get; set; }

        public ControllerCollection Controllers
        {
            get
            {
                return controllers;
            }
        }

        public ViewCollection Views
        {
            get
            {
                return views;
            }
        }

        internal void _setCore(AnomalousMvcCore core)
        {
            this.core = core;
        }

        protected AnomalousMvcContext(LoadInfo info)
        {
            controllers = info.GetValue<ControllerCollection>("Controllers");
            views = info.GetValue<ViewCollection>("Views");
            StartupAction = info.GetString("StartupAction");
            ShutdownAction = info.GetString("ShutdownAction");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("StartupAction", StartupAction);
            info.AddValue("ShutdownAction", ShutdownAction);
            info.AddValue("Controllers", controllers);
            info.AddValue("Views", views);
        }

        public void hideMainInterface(bool showSharedGui)
        {
            core.hideMainInterface(showSharedGui);
        }

        public void showMainInterface()
        {
            core.showMainInterface();
        }

        public void shutdown()
        {
            checkShutdownConditions();
        }

        public bool AllowShutdown
        {
            get
            {
                return allowShutdown;
            }
            set
            {
                allowShutdown = value;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return core.MeasurementGrid;
            }
        }

        /// <summary>
        /// This is a special method to run the last action for a context. It
        /// does not allow views or timelines to be queued, but it can do a few
        /// things on shutdown.
        /// </summary>
        /// <param name="address"></param>
        internal void runFinalAction(String address)
        {
            doRunAction(address);
        }

        private void checkShutdownConditions()
        {
            if (allowShutdown)
            {
                //Check for shutdown conditions
                //No views are open
                if (!core.HasOpenViews)
                {
                    if (core.PlayingTimeline)
                    {
                        //Reevaluate again when the timeline stops playing
                        core.TimelineStopped += core_TimelineStopped;
                    }
                    else
                    {
                        //Not timelines playing and no views showing, shutdown
                        core.shutdownContext(this);
                    }
                }
            }
        }

        private void doRunAction(string address)
        {
            int slashLoc = address.IndexOf('/');
            String controllerName = address.Substring(0, slashLoc);
            ++slashLoc;
            String actionName = address.Substring(slashLoc, address.Length - slashLoc);
            MvcController controller = controllers[controllerName];
            controller.runAction(actionName, this);
        }

        void core_TimelineStopped()
        {
            core.TimelineStopped -= core_TimelineStopped;
            checkShutdownConditions();
        }
    }

    public partial class AnomalousMvcContext
    {
        private EditInterface editInterface;

        public enum CustomQueries
        {
            Preview
        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, "MVC", null);// new EditInterface("MVC");
                editInterface.addSubInterface(views.getEditInterface("Views"));
                editInterface.addSubInterface(controllers.getEditInterface("Controllers"));
                editInterface.addCommand(new EditInterfaceCommand("Preview", preview));
            }
            return editInterface;
        }

        private void preview(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.runCustomQuery(CustomQueries.Preview, null, this);
        }
    }
}
