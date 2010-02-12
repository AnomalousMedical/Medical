using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;
using Engine;
using Engine.Saving.XMLSaver;
using Engine.Resources;
using System.IO;
using System.Xml;
using Logging;

namespace Medical.GUI
{
    public partial class DiscSpacePanel : StatePickerPanel
    {
        private String discName;
        private Disc disc;
        private Vector3 openingStateOffset = Vector3.Zero;

        private String currentPresetDirectory;
        private static XmlSaver xmlSaver = new XmlSaver();
        private PresetState presetState;
        private String defaultLayerState;
        private String subDirectory;

        public DiscSpacePanel(String discName, String defaultLayerState, String subDirectory, StatePickerPanelController panelController)
            :base(panelController)
        {
            InitializeComponent();
            this.discName = discName;
            this.defaultLayerState = defaultLayerState;
            this.subDirectory = subDirectory;
        }

        public override void recordOpeningState()
        {
            
        }

        public override void resetToOpeningState()
        {
            
        }

        public override void applyToState(MedicalState state)
        {
            
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            
        }

        protected override void onPanelOpening()
        {
            
        }
    }
}
