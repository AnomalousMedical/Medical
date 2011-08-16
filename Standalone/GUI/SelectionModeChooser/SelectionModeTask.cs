using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    public class SelectionModeTask : Task
    {
        private SelectionModeChooser selectionModeChooser;

        public SelectionModeTask(AnatomyController anatomyController)
            :base("Medical.SelectionMode", "Selection Mode", "", TaskMenuCategories.Navigation)
        {
            this.ShowOnTaskbar = false;
            selectionModeChooser = new SelectionModeChooser(anatomyController);
            anatomyController.PickingModeChanged += new EventDelegate<AnatomyController, AnatomyPickingMode>(anatomyController_PickingModeChanged);
            anatomyController_PickingModeChanged(anatomyController, anatomyController.PickingMode);
        }

        public void Dispose()
        {
            selectionModeChooser.Dispose();
        }

        public override void clicked()
        {
            IntVector2 pos = findGoodWindowPosition(selectionModeChooser.Width, selectionModeChooser.Height);
            selectionModeChooser.show(pos.x, pos.y);
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        void anatomyController_PickingModeChanged(AnatomyController source, AnatomyPickingMode arg)
        {
            switch (arg)
            {
                case AnatomyPickingMode.Group:
                    IconName = "SelectionIcons\\GroupSelection";
                    break;
                case AnatomyPickingMode.Individual:
                    IconName = "SelectionIcons\\IndividualSelection";
                    break;
                case AnatomyPickingMode.None:
                    IconName = "SelectionIcons\\NoSelection";
                    break;
            }
            fireIconChanged();
        }
    }
}
