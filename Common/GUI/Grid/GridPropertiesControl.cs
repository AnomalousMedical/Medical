using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Medical.GUI.Grid
{
    public partial class GridPropertiesControl : UserControl
    {
        private MeasurementGrid grid;
        private Vector3 origin = Vector3.Zero;

        public GridPropertiesControl()
        {
            InitializeComponent();
            showGridCheckBox.CheckedChanged += new EventHandler(showGridCheckBox_CheckedChanged);
            gridSpaceControl.ValueChanged += new EventHandler(gridSpaceControl_ValueChanged);
        }

        public void setGrid(MeasurementGrid grid)
        {
            this.grid = grid;
        }

        public void updateGrid()
        {
            grid.drawGrid((float)gridSpaceControl.Value, 100);
            grid.Visible = showGridCheckBox.Checked;
            grid.Origin = origin;
        }

        public bool GridVisible
        {
            get
            {
                return gridSpaceControl.Visible;
            }
            set
            {
                gridSpaceControl.Visible = value;
            }
        }

        public float GridSpacing
        {
            get
            {
                return (float)gridSpaceControl.Value;
            }
            set
            {
                gridSpaceControl.Value = (decimal)value;
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

        void gridSpaceControl_ValueChanged(object sender, EventArgs e)
        {
            if (grid != null)
            {
                grid.drawGrid((float)gridSpaceControl.Value, 100);
            }
        }

        void showGridCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (grid != null)
            {
                grid.Visible = showGridCheckBox.Checked;
            }
        }
    }
}
