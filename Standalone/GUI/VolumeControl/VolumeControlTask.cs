using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using SoundPlugin;

namespace Medical.GUI
{
    public class VolumeControlTask : Task
    {
        public VolumeControlTask()
            : base("Medical.VolumeControl", "Volume", SoundConfig.MasterVolume > 0.0f ? "StandaloneIcons/VolumeControl" : "StandaloneIcons/VolumeControlOff", TaskMenuCategories.System)
        {
            this.ShowOnTimelineTaskbar = true;
            SoundConfig.MasterVolumeChanged += new EventHandler(SoundConfig_MasterVolumeChanged);
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

        void SoundConfig_MasterVolumeChanged(object sender, EventArgs e)
        {
            if (SoundConfig.MasterVolume > 0.0f)
            {
                if (IconName != "StandaloneIcons/VolumeControl")
                {
                    IconName = "StandaloneIcons/VolumeControl";
                    fireIconChanged();
                }
            }
            else
            {
                if (IconName != "StandaloneIcons/VolumeControlOff")
                {
                    IconName = "StandaloneIcons/VolumeControlOff";
                    fireIconChanged();
                }
            }
        }
    }
}
