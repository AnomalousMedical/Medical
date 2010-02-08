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
    public partial class TeethHeightAdaptationPanel : TeethAdaptationPanel
    {
        public TeethHeightAdaptationPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            heightControl1.sceneChanged();
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            heightControl1.getPositionFromScene();
        }
    }
}
