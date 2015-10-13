using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;
using Engine;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Developer.GUI
{
    class AdvancedCameraGui : MDIDialog
    {
        private SceneViewController sceneViewController;
        private NumericEdit fovyEdit;

        public AdvancedCameraGui(SceneViewController sceneViewController)
            : base("Developer.GUI.AdvancedCamera.AdvancedCameraGui.layout")
        {
            this.sceneViewController = sceneViewController;
            sceneViewController.ActiveWindowChanged += SceneViewController_ActiveWindowChanged;
            fovyEdit = new NumericEdit(window.findWidget("FovYEdit") as EditBox);
            fovyEdit.MinValue = 5;
            fovyEdit.MaxValue = 180;
            fovyEdit.AllowFloat = true;
            fovyEdit.Increment = 10;
            fovyEdit.ValueChanged += FovyEdit_ValueChanged;
            SceneViewController_ActiveWindowChanged(sceneViewController.ActiveWindow);
        }

        public override void Dispose()
        {
            sceneViewController.ActiveWindowChanged -= SceneViewController_ActiveWindowChanged;
            base.Dispose();
        }

        private void SceneViewController_ActiveWindowChanged(SceneViewWindow window)
        {
            if (window != null)
            {
                fovyEdit.FloatValue = window.FovY.Degrees;
            }
        }

        private void FovyEdit_ValueChanged(Widget source, EventArgs e)
        {
            if(sceneViewController.ActiveWindow != null)
            {
                sceneViewController.ActiveWindow.FovY = new Degree(fovyEdit.FloatValue).Radians;
            }
        }
    }
}
