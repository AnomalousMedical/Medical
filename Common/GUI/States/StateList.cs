using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Medical.Properties;
using Engine;

namespace Medical.GUI
{
    public partial class StateList : UserControl
    {
        private MedicalStateController stateController;
        private Dictionary<MedicalState, KryptonListItem> entries = new Dictionary<MedicalState, KryptonListItem>();
        private ImageRenderer imageRenderer;

        private static ImageRendererProperties imageProperties = new ImageRendererProperties();

        static StateList()
        {
            imageProperties.Width = 100;
            imageProperties.Height = 100;
            imageProperties.UseActiveViewportLocation = false;
            imageProperties.AntiAliasingMode = 2;
            imageProperties.UseWindowBackgroundColor = false;
            imageProperties.CustomBackgroundColor = Engine.Color.Black;
            imageProperties.ShowWatermark = false;
            
            imageProperties.UseNavigationStatePosition = true;
            imageProperties.NavigationStateName = "Midline Anterior";

            imageProperties.OverrideLayers = true;
            imageProperties.LayerState = "MandibleSizeLayers";
        }

        public StateList(MedicalStateController stateController, ImageRenderer imageRenderer)
        {
            InitializeComponent();
            
            this.stateController = stateController;
            stateController.StateAdded += new MedicalStateAdded(stateController_StateAdded);
            stateController.StateRemoved += new MedicalStateRemoved(stateController_StateRemoved);
            stateController.StatesCleared += new MedicalStatesCleared(stateController_StatesCleared);
            stateController.StateChanged += new MedicalStateChanged(stateController_StateChanged);

            this.imageRenderer = imageRenderer;
        }

        void stateController_StateAdded(MedicalStateController controller, MedicalState state, int index)
        {
            

            KryptonListItem entry = new KryptonListItem();
            entry.ShortText = state.Name;
            entry.Image = imageRenderer.renderImage(imageProperties);
            entries.Add(state, entry);
            stateListBox.Items.Add(entry);
        }

        void stateController_StateRemoved(MedicalStateController controller, MedicalState state, int index)
        {
            KryptonListItem entry = entries[state];
            stateListBox.Items.Remove(entry);
            entry.Dispose();
            entries.Remove(state);
        }

        void stateController_StatesCleared(MedicalStateController controller)
        {
            stateListBox.Items.Clear();
            entries.Clear();
        }

        void stateController_StateChanged(MedicalState state)
        {
            
        }
    }
}
