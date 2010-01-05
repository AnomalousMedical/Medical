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

namespace Medical.GUI
{
    public partial class DiscSpacePanel : StatePickerPanel
    {
        private String discName;
        private Disc disc;
        private Vector3 openingStateOffset = Vector3.Zero;

        public DiscSpacePanel(String discName)
        {
            InitializeComponent();
            this.discName = discName;
            discOffsetSlider.ValueChanged += new EventHandler(discOffsetSlider_ValueChanged);
        }

        void discOffsetSlider_ValueChanged(object sender, EventArgs e)
        {
            disc.PopLocation = disc.NormalPopLocation;
            disc.PopLocation = disc.NormalPopLocation;
            disc.Locked = false;
            Vector3 newOffset = disc.NormalDiscOffset;
            newOffset.y = discOffsetSlider.Value / -(float)discOffsetSlider.Maximum;
            disc.DiscOffset = newOffset;
        }

        public override void applyToState(MedicalState state)
        {
            state.Disc.addPosition(new DiscStateProperties(disc));
        }

        internal void sceneLoaded(SimScene scene)
        {
            disc = DiscController.getDisc(discName);
        }

        protected override void onPanelOpening()
        {
            discOffsetSlider.Value = (int)(disc.DiscOffset.y * -discOffsetSlider.Maximum);
        }

        public override void recordOpeningState()
        {
            openingStateOffset = disc.DiscOffset;
        }

        public override void resetToOpeningState()
        {
            disc.DiscOffset = openingStateOffset;
        }

        public Image BoneOnBoneImage
        {
            get
            {
                return normalPanel.BackgroundImage;
            }
            set
            {
                normalPanel.BackgroundImage = value;
            }
        }

        public Image OpenImage
        {
            get
            {
                return distortedPanel.BackgroundImage;
            }
            set
            {
                distortedPanel.BackgroundImage = value;
            }
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            resetToOpeningState();
            onPanelOpening();
        }

        private void makeNormalButton_Click(object sender, EventArgs e)
        {
            disc.DiscOffset = disc.NormalDiscOffset;
            onPanelOpening();
        }
    }
}
