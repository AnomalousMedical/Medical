using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class StatePickerPanel : UserControl
    {
        protected StatePicker parentPicker;

        public StatePickerPanel()
        {
            InitializeComponent();
        }

        public void setStatePicker(StatePicker parentPicker)
        {
            this.parentPicker = parentPicker;
        }

        public virtual void applyToState(MedicalState state)
        { 
        
        }

        public virtual void setToDefault()
        {

        }

        public virtual void recordOpeningState()
        {

        }

        public virtual void resetToOpeningState()
        {

        }

        public String NavigationState { get; set; }

        public String LayerState { get; set; }

        protected void showChanges(bool immediate)
        {
            parentPicker.showChanges(immediate);
        }
    }
}
