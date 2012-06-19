using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    public class VolumeControlTask : Task
    {
        public VolumeControlTask()
            : base("Medical.VolumeControl", "Volume", "StandaloneIcons/VolumeControl", TaskMenuCategories.System)
        {
            this.ShowOnTimelineTaskbar = true;
        }

        public override void clicked(TaskPositioner positioner)
        {
            IntVector2 location = positioner.findGoodWindowPosition(0, 0);
            VolumeControl volumeControl = new VolumeControl();
            volumeControl.Hidden += new EventHandler(volumeControl_Hidden);
            volumeControl.show(location.x, location.y);
        }

        void volumeControl_Hidden(object sender, EventArgs e)
        {
            fireItemClosed();
            ((VolumeControl)sender).Dispose();
        }

        public override bool Active
        {
            get { return false; }
        }
    }
}
