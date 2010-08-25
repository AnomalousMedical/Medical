using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller;

namespace Medical.GUI
{
    class GridPropertiesControl
    {
        private MeasurementGrid grid;
        private Vector3 origin = Vector3.Zero;
        private NumericEdit gridSpaceControl;
        private CheckButton showGridCheckBox;

        public GridPropertiesControl(MeasurementGrid grid, Widget mainWidget)
        {
            this.grid = grid;

            gridSpaceControl = new NumericEdit(mainWidget.findWidget("GridControl/GridSpace") as Edit, mainWidget.findWidget("GridControl/GridSpaceUp") as Button, mainWidget.findWidget("GridControl/GridSpaceDown") as Button);
            showGridCheckBox = new CheckButton(mainWidget.findWidget("GridControl/ShowGrid") as Button);

            gridSpaceControl.MinValue = 1;

            showGridCheckBox.CheckedChanged += showGridCheckBox_CheckedChanged; 
            gridSpaceControl.ValueChanged += gridSpaceControl_ValueChanged;
        }

        public void updateGrid()
        {
            grid.drawGrid(gridSpaceControl.FloatValue, 100);
            grid.Visible = showGridCheckBox.Checked;
            grid.Origin = origin;
        }

        public bool GridVisible
        {
            get
            {
                return showGridCheckBox.Checked;
            }
            set
            {
                showGridCheckBox.Checked = value;
            }
        }

        public float GridSpacing
        {
            get
            {
                return gridSpaceControl.FloatValue;
            }
            set
            {
                gridSpaceControl.FloatValue = value;
            }
        }

        public Vector3 Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
            }
        }

        void gridSpaceControl_ValueChanged(Widget sender, EventArgs e)
        {
            if (grid != null)
            {
                grid.drawGrid(gridSpaceControl.FloatValue, 100);
            }
        }

        void showGridCheckBox_CheckedChanged(Widget sender, EventArgs e)
        {
            if (grid != null)
            {
                grid.Visible = showGridCheckBox.Checked;
            }
        }
    }
}
