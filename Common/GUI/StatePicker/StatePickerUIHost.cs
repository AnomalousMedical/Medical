using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    /// <summary>
    /// This interface allows the StatePickerWizard to be displayed on a user
    /// interface of some sort. The specifics are up to the interface itself,
    /// but the functions give a general idea of how the controls should be laid
    /// out.
    /// </summary>
    public interface StatePickerUIHost
    {
        void setDataControl(Control control);

        void setTopInformation(Control control);
    }
}
