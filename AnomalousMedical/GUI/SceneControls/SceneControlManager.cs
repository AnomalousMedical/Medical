using Engine;
using Engine.ObjectManagement;
using Engine.Platform;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class SceneControlManager : IDisposable
    {
        StandaloneController standaloneController;
        SceneViewController sceneViewController;

        private List<SceneControlWidget> widgets = new List<SceneControlWidget>();

        public SceneControlManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            this.sceneViewController = standaloneController.SceneViewController;

            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;
            standaloneController.MedicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;

            createWidgets();
        }

        public void Dispose()
        {
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            destroyWidgets();
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
            //Replace this polling with an event, will need to modify sceneviews some to do this
            var activeWindow = sceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                foreach (var widget in widgets)
                {
                    IntVector2 point = activeWindow.getAbsoluteScreenPosition(widget.SceneAnatomyControl.WorldPosition);
                    widget.Position = point;
                    widget.Visible = activeWindow.containsPoint(point);
                }
            }
            else if(widgets.Count > 0 && widgets[0].Visible)
            {
                foreach (var widget in widgets)
                {
                    widget.Visible = false;
                }
            }
        }

        private void createWidgets()
        {
            foreach (var sceneControl in SceneAnatomyControlManager.Controls)
            {
                SceneControlWidget widget = new PinControlWidget(sceneControl);
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
