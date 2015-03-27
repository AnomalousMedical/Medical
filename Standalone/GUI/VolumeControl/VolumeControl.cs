using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using SoundPlugin;

namespace Medical.GUI
{
    class VolumeControl : PopupContainer
    {
        private ScrollBar volumeSlider;
        private bool allowVolumeUpdates = true;
        private CheckButton volumeButton;
        private float beforeMuteVolume;

        public VolumeControl()
            : base("Medical.GUI.VolumeControl.VolumeControl.layout")
        {
            SoundConfig.MasterVolumeChanged += SoundConfig_MasterVolumeChanged;
            volumeSlider = widget.findWidget("VolumeSlider") as ScrollBar;
            volumeSlider.ScrollChangePosition += new MyGUIEvent(volumeSlider_ScrollChangePosition);
            SoundConfig_MasterVolumeChanged(null, null);

            volumeButton = new CheckButton((Button)widget.findWidget("VolumeSliderButton"));
            volumeButton.Checked = SoundConfig.MasterVolume < 0.001f;
            volumeButton.CheckedChanged += new MyGUIEvent(volumeButton_CheckedChanged);
            recordLastVolume();
        }

        public override void Dispose()
        {
            SoundConfig.MasterVolumeChanged -= SoundConfig_MasterVolumeChanged;
            base.Dispose();
        }

        void volumeSlider_ScrollChangePosition(Widget source, EventArgs e)
        {
            allowVolumeUpdates = false;
            SoundConfig.MasterVolume = (volumeSlider.ScrollRange - volumeSlider.ScrollPosition) / (float)volumeSlider.ScrollRange;
            volumeButton.Checked = SoundConfig.MasterVolume < 0.001f;
            allowVolumeUpdates = true;
        }

        void SoundConfig_MasterVolumeChanged(object sender, EventArgs e)
        {
            if (allowVolumeUpdates)
            {
                uint scrollPos = (uint)(volumeSlider.ScrollRange - volumeSlider.ScrollRange * SoundConfig.MasterVolume);
                if (scrollPos >= volumeSlider.ScrollRange)
                {
                    --scrollPos;
                }
                volumeSlider.ScrollPosition = scrollPos;
            }
        }

        void volumeButton_CheckedChanged(Widget source, EventArgs e)
        {
            if (volumeButton.Checked)
            {
                recordLastVolume();
                SoundConfig.MasterVolume = 0;
            }
            else
            {
                SoundConfig.MasterVolume = beforeMuteVolume;
            }
        }

        private void recordLastVolume()
        {
            beforeMuteVolume = SoundConfig.MasterVolume;
            if (beforeMuteVolume < 0.001f)
            {
                beforeMuteVolume = 1f;
            }
        }
    }
}
