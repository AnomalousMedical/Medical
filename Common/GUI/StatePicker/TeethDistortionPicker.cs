using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Properties;

namespace Medical.GUI
{
    class TeethDistortionPicker : FileBrowserPickerPanel
    {
        public TeethDistortionPicker()
            :base("*.dst")
        {
            this.Text = "Teeth";
            this.NavigationState = "Teeth Midline Anterior";
            this.LayerState = "TeethLayers";
            this.TextLine1 = "Teeth";
            this.LargeIcon = Resources.AdaptationIcon;
        }

        public override void applyToState(MedicalState state)
        {
            
        }

        protected override void onFileChosen(string filename)
        {
            
        }
    }
}
