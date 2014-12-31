using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Anomalous.GuiFramework;

namespace Medical.GUI
{
    class GridPropertiesDialog : MDIDialog
    {
        GridPropertiesControl gridPropertiesControl;

        public GridPropertiesDialog(MeasurementGrid measurementGrid)
            :base("Developer.GUI.GridPropertiesDialog.GridPropertiesDialog.layout")
        {
            gridPropertiesControl = new GridPropertiesControl(measurementGrid, window);
            gridPropertiesControl.GridSpacing = 2;
            this.Shown += GridPropertiesDialog_Shown;
        }

        void GridPropertiesDialog_Shown(object sender, EventArgs e)
        {
            gridPropertiesControl.updateGrid();
        }
    }
}
