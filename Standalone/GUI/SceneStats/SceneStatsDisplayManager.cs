using Medical.Controller;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class SceneStatsDisplayManager
    {
        private List<SceneStatsDisplay> activeSceneStats = new List<SceneStatsDisplay>();
        private RenderTarget displayStatsTarget;
        private bool statsVisible = MedicalConfig.EngineConfig.ShowStatistics;

        public SceneStatsDisplayManager(SceneViewController sceneViewController, RenderTarget displayStatsTarget)
        {
            this.displayStatsTarget = displayStatsTarget;
            sceneViewController.WindowCreated += sceneViewController_WindowCreated;
            MedicalConfig.EngineConfig.ShowStatsToggled += EngineConfig_ShowStatsToggled;
        }

        void sceneViewController_WindowCreated(SceneViewWindow window)
        {
            MDISceneViewWindow mdiWindow = window as MDISceneViewWindow;
            if (mdiWindow != null)
            {
                SceneStatsDisplay licenseDisplay = new SceneStatsDisplay(displayStatsTarget);
                licenseDisplay.Visible = statsVisible;
                activeSceneStats.Add(licenseDisplay);
                mdiWindow.addChildContainer(licenseDisplay.LayoutContainer);
                mdiWindow.Disposed += (win) =>
                {
                    activeSceneStats.Remove(licenseDisplay);
                    licenseDisplay.Dispose();
                };
            }
        }

        void EngineConfig_ShowStatsToggled(EngineConfig config)
        {
            statsVisible = MedicalConfig.EngineConfig.ShowStatistics;
            foreach (var display in activeSceneStats)
            {
                display.Visible = statsVisible;
            }
        }
    }
}
