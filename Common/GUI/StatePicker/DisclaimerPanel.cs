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
    public partial class DisclaimerPanel : StatePickerPanel
    {
        public DisclaimerPanel(StatePickerPanelController controller)
            :base(controller)
        {
            InitializeComponent();
        }
    }
}
