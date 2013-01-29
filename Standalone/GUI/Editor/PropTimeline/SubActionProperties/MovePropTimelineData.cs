using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class MovePropTimelineData : PropTimelineData
    {
        private MovePropAction moveProp;
        private PropEditController propEditController;

        public MovePropTimelineData(MovePropAction movePropAction, PropEditController propEditController)
            : base(movePropAction)
        {
            this.moveProp = movePropAction;
            this.propEditController = propEditController;
        }

        public override void editingStarted()
        {
            base.editingStarted();
            propEditController.CurrentMovePropAction = moveProp;
            propEditController.ShowTools = true;
        }

        public override void editingCompleted()
        {
            propEditController.CurrentMovePropAction = null;
            propEditController.ShowTools = false;
            base.editingCompleted();
        }
    }
}
