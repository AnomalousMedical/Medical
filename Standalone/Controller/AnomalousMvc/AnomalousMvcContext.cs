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
        private ModelCollection models;

        //State recording stuff
        private ModelMemory modelMemory = new ModelMemory();

        //Action queue stuff
        private String queuedTimeline = null;
        private bool allowShutdown = true;
        private Queue<String> queuedActions = new Queue<string>();
        private ViewHost runningActionViewHost;

        public AnomalousMvcContext()
        {
            controllers = new ControllerCollection();
            views = new ViewCollection();
            models = new ModelCollection();
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

        public void runAction(string address, ViewHost viewHost = null)
        {
            runningActionViewHost = viewHost;
            queuedTimeline = null;

            doRunAction(address);

            if (queuedTimeline != null)
            {
                playTimeline(queuedTimeline);
            }

            core.processViewChanges();

            if (queuedActions.Count > 0)
            {
                runAction(queuedActions.Dequeue(), viewHost);
            }
            else
            {
                checkShutdownConditions();
            }
        }

        public void queueRunAction(String address)
        {
            queuedActions.Enqueue(address);
        }

        public string getFullPath(string file)
        {
            return resourceProvider.getFullFilePath(file);
        }

        public void queueTimeline(string timeline)
        {
            queuedTimeline = timeline;
        }

        public void queueShowView(String view, ViewLocations viewLocation)
        {
            core.queueShowView(views[view], this, viewLocation);
        }

        public void queueCloseView()
        {
            core.queueCloseView(runningActionViewHost);
        }

        public void queueCloseAllViews()
        {
            core.queueCloseAllViews();
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

        public void createMedicalState(MedicalStateInfoModel stateInfo)
        {
            core.createMedicalState(stateInfo);
        }

        public void addModel(String name, Object model)
        {
            modelMemory.add(name, model);
        }

        public TypeName getModel<TypeName>(String name)
            where TypeName : class
        {
            return modelMemory.get<TypeName>(name);
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

        public ModelCollection Models
        {
            get
            {
                return models;
            }
        }

        internal void starting(AnomalousMvcCore core)
        {
            this.core = core;
            foreach (MvcModel model in models)
            {
                model.reset();
                modelMemory.add(model.Name, model);
            }
        }

        protected AnomalousMvcContext(LoadInfo info)
        {
            StartupAction = info.GetString("StartupAction");
            ShutdownAction = info.GetString("ShutdownAction");
            controllers = info.GetValue<ControllerCollection>("Controllers");
            views = info.GetValue<ViewCollection>("Views");
            models = info.GetValue<ModelCollection>("Models", null);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("StartupAction", StartupAction);
            info.AddValue("ShutdownAction", ShutdownAction);
            info.AddValue("Controllers", controllers);
            info.AddValue("Views", views);
            info.AddValue("Models", models);
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

        public ResourceProvider ResourceProvider
        {
            get
            {
                return resourceProvider;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return core.MeasurementGrid;
            }
        }

        public AnatomyController AnatomyController
        {
            get
            {
                return core.AnatomyController;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return core.ImageRenderer;
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
            if (address != null)
            {
                int slashLoc = address.IndexOf('/');
                if (slashLoc != -1)
                {
                    String controllerName = address.Substring(0, slashLoc);
                    ++slashLoc;
                    String actionName = address.Substring(slashLoc, address.Length - slashLoc);
                    MvcController controller = controllers[controllerName];
                    controller.runAction(actionName, this);
                }
                else
                {
                    Log.Error("Malformed action address '{0}' the format must be 'Controller/Action' cannot run action", address);
                }
            }
            else
            {
                Log.Error("Address was null, cannot run action.", address);
            }
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
                editInterface.addSubInterface(models.getEditInterface("Models"));
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
