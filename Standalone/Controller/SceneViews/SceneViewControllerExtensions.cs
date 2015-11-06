using Anomalous.GuiFramework.Cameras;
using Engine;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public static class SceneViewControllerExtensions
    {
        public static BookmarksController BookmarksController { get; set; }

        public static void createFromPresets(this SceneViewController controller, SceneViewWindowPresetSet presets, bool keepOldSettings = true)
        {
            //Capture current window configuration info
            List<Bookmark> currentWindowConfig = new List<Bookmark>();
            if (keepOldSettings)
            {
                SceneViewWindow activeWindow = controller.ActiveWindow;
                if (activeWindow != null)
                {
                    TransparencyController.ActiveTransparencyState = activeWindow.CurrentTransparencyState;
                    LayerState layerState = new LayerState();
                    layerState.captureState();
                    currentWindowConfig.Add(new Bookmark("", activeWindow.Translation, activeWindow.LookAt, layerState));
                }
                foreach (MDISceneViewWindow window in controller.MdiWindows)
                {
                    if (window != activeWindow)
                    {
                        TransparencyController.ActiveTransparencyState = window.CurrentTransparencyState;
                        LayerState layerState = new LayerState();
                        layerState.captureState();
                        currentWindowConfig.Add(new Bookmark("", window.Translation, window.LookAt, layerState));
                    }
                }
            }

            //Create windows
            int windowIndex = 0;
            int zOrder = 100;
            int zOrderInc = 10;
            controller.closeAllWindows();
            MDISceneViewWindow camera;
            MDISceneViewWindow toSelect = null;
            foreach (SceneViewWindowPreset preset in presets.getPresetEnum())
            {
                if (windowIndex < currentWindowConfig.Count)
                {
                    Bookmark bmk = currentWindowConfig[windowIndex++];
                    camera = controller.createWindow(preset.Name, bmk.CameraPosition.Translation, bmk.CameraPosition.LookAt, preset.BoundMin, preset.BoundMax, preset.OrbitMinDistance, preset.OrbitMaxDistance, zOrder, controller.findWindow(preset.ParentWindow), preset.WindowPosition);
                    TransparencyController.ActiveTransparencyState = camera.CurrentTransparencyState;
                    bmk.Layers.instantlyApply();
                }
                else
                {
                    camera = controller.createWindow(preset.Name, preset.Position, preset.LookAt, preset.BoundMin, preset.BoundMax, preset.OrbitMinDistance, preset.OrbitMaxDistance, zOrder, controller.findWindow(preset.ParentWindow), preset.WindowPosition);
                    Bookmark bmk = null;
                    if (BookmarksController != null)
                    {
                        bmk = BookmarksController.loadBookmark(String.Format("Cameras/{0}.bmk", camera.CurrentTransparencyState));

                        if (bmk != null)
                        {
                            camera.setPosition(bmk.CameraPosition, 0.0f);
                            TransparencyController.ActiveTransparencyState = camera.CurrentTransparencyState;
                            bmk.Layers.instantlyApply();
                        }
                    }
                }
                if (toSelect == null)
                {
                    toSelect = camera;
                }
                zOrder += zOrderInc;
            }
            if (toSelect != null)
            {
                controller.setActiveMdiWindow(toSelect);
            }
        }
    }
}
