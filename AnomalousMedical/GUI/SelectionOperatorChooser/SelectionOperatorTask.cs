using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    class SelectionOperatorTask : Task
    {
        private SelectionOperatorChooser selectionOperatorChooser;

        public SelectionOperatorTask(AnatomyController anatomyController)
            :base("Medical.SelectionOperator", "Selection Operator", "", TaskMenuCategories.Navigation)
        {
            this.ShowOnTaskbar = false;
            selectionOperatorChooser = new SelectionOperatorChooser(anatomyController);
            anatomyController.SelectionOperatorChanged += anatomyController_SelectionOperatorChanged;
            anatomyController_SelectionOperatorChanged(anatomyController, anatomyController.SelectionOperator);
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

        void anatomyController_SelectionOperatorChanged(AnatomyController source, SelectionOperator arg)
        {
            switch (arg)
            {
                case SelectionOperator.Select:
                    IconName = "AnatomyFinder.Select";
                    break;
                case SelectionOperator.Add:
                    IconName = "AnatomyFinder.Add";
                    break;
                case SelectionOperator.Remove:
                    IconName = "AnatomyFinder.Remove";
                    break;
            }
            fireIconChanged();
        }
    }
}
