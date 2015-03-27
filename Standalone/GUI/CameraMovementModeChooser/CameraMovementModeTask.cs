using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;

namespace Medical.GUI
{
    class CameraMovementModeTask : Task
    {
        private CameraMovementModeChooser selectionOperatorChooser;

        public CameraMovementModeTask(SceneViewController sceneViewController)
            :base("Medical.CameraMovementMode", "Camera Mode", "", TaskMenuCategories.Explore)
        {
            this.ShowOnTaskbar = false;
            selectionOperatorChooser = new CameraMovementModeChooser(sceneViewController);
            sceneViewController.CameraMovementModeChanged += sceneViewController_CameraMovementModeChanged;
            sceneViewController_CameraMovementModeChanged(sceneViewController.MovementMode);
        }

        public void Dispose()
        {
            selectionOperatorChooser.Dispose();
        }

        public override void clicked(TaskPositioner positioner)
        {
            IntVector2 pos = positioner.findGoodWindowPosition(selectionOperatorChooser.Width, selectionOperatorChooser.Height);
            selectionOperatorChooser.show(pos.x, pos.y);
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        void sceneViewController_CameraMovementModeChanged(CameraMovementMode arg)
        {
            switch (arg)
            {
                case CameraMovementMode.Rotate:
                    IconName = "CameraMovementIcons.Rotate";
                    break;
                case CameraMovementMode.Pan:
                    IconName = "CameraMovementIcons.Pan";
                    break;
                case CameraMovementMode.Zoom:
                    IconName = "CameraMovementIcons.Zoom";
                    break;
            }
            fireIconChanged();
        }
    }
}
