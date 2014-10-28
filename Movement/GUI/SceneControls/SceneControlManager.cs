using Engine;
using Engine.ObjectManagement;
using Engine.Platform;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Movement.GUI
{
    class SceneControlManager : IDisposable
    {
        StandaloneController standaloneController;
        SceneViewController sceneViewController;

        private bool subscribedToUpdates = false;
        private SceneAnatomyControlType visibleTypes;

        private List<SceneControlWidget> widgets = new List<SceneControlWidget>();

        public SceneControlManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            this.sceneViewController = standaloneController.SceneViewController;

            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            createWidgets();
        }

        public void Dispose()
        {
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
            destroyWidgets();
        }

        public void setTypeVisible(SceneAnatomyControlType type, bool visible)
        {
            if(visible)
            {
                visibleTypes |= type;
            }
            else
            {
                visibleTypes &= ~type;
            }

            foreach(var widget in widgets)
            {
                widget.setVisibleTypes(visibleTypes);
            }

            if(visibleTypes == 0 && subscribedToUpdates)
            {
                standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
                subscribedToUpdates = false;
            }
            else if(visibleTypes != 0 && !subscribedToUpdates)
            {
                standaloneController.MedicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;
                subscribedToUpdates = true;
            }
        }

        public bool isTypeVisible(SceneAnatomyControlType type)
        {
            return (type & visibleTypes) != 0;
        }

        void standaloneController_SceneUnloading(SimScene scene)
        {
            destroyWidgets();
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            createWidgets();
        }

        void MedicalController_OnLoopUpdate(Clock time)
        {
            var activeWindow = sceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                foreach (var widget in widgets)
                {
                    IntVector2 point = activeWindow.getAbsoluteScreenPosition(widget.SceneAnatomyControl.WorldPosition);
                    widget.Position = point;
                    widget.setCameraVisible(activeWindow.containsPoint(point));
                }
            }
        }

        private void createWidgets()
        {
            foreach (var sceneControl in SceneAnatomyControlManager.Controls)
            {
                SceneControlWidget widget = new ToggleControlWidget(sceneControl);
                widget.setVisibleTypes(visibleTypes);
                widgets.Add(widget);
            }
        }

        private void destroyWidgets()
        {
            foreach (var widget in widgets)
            {
                widget.Dispose();
            }
            widgets.Clear();
        }
    }
}
