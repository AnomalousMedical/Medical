using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    public enum ClockFace
    {
        Unknown = 0,
        Clock12 = 1,
        Clock11 = 2,
        Clock10 = 4
    }

    public partial class DiscSpaceControl : UserControl
    {
        public event EventHandler CurrentStageChanged;

        PiperStage currentStage = PiperStage.Unknown;
        Dictionary<PiperStage, int> stageMap = new Dictionary<PiperStage, int>();
        Dictionary<ClockFace, int> clockFaceMap = new Dictionary<ClockFace, int>();
        private bool allowRdaReductionEventFire = true;
        private bool allowStageChangeEventFire = true;
        private bool allowClockChangeEventFire = true;
        RdaReduction currentReduction = RdaReduction.Unknown;
        ClockFace currentClockFace = ClockFace.Unknown;

        public DiscSpaceControl()
        {
            InitializeComponent();
            
            stageMap.Add(PiperStage.I, 0);
            stageMap.Add(PiperStage.II, 0);
            stageMap.Add(PiperStage.IIIa, 0);
            stageMap.Add(PiperStage.IIIb, 0);
            stageMap.Add(PiperStage.IVa, 0);
            stageMap.Add(PiperStage.IVb, 0);
            stageMap.Add(PiperStage.Va, 0);
            stageMap.Add(PiperStage.Vb, 0);

            clockFaceMap.Add(ClockFace.Clock10, 0);
            clockFaceMap.Add(ClockFace.Clock11, 0);
            clockFaceMap.Add(ClockFace.Clock12, 0);

            verticalSpaceCombo.SelectedIndexChanged += new EventHandler(comboSelectionChanged);
            horizontalSpaceCombo.SelectedIndexChanged += new EventHandler(comboSelectionChanged);
            condyleShapeCombo.SelectedIndexChanged += new EventHandler(comboSelectionChanged);

            stageIButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIIButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIIIaButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIIIbButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIVaButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIVbButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageVaButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageVbButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);

            clock10Radio.CheckedChanged += new EventHandler(clockRadio_CheckedChanged);
            clock11Radio.CheckedChanged += new EventHandler(clockRadio_CheckedChanged);
            clock12Radio.CheckedChanged += new EventHandler(clockRadio_CheckedChanged);

            mildRDAReductionButton.CheckedChanged += new EventHandler(RDAReductionButton_CheckedChanged);
            moderateRDAReductionButton.CheckedChanged += new EventHandler(RDAReductionButton_CheckedChanged);
            severeRDAReductionButton.CheckedChanged += new EventHandler(RDAReductionButton_CheckedChanged);

            rdaReductionGroupBox.Enabled = false;
            clockFaceGroupBox.Enabled = false;

            computeDiscSpaceStage();
        }

        public void setToDefault()
        {
            verticalSpaceCombo.SelectedItem = "Normal";
            horizontalSpaceCombo.SelectedItem = "Normal";
            condyleShapeCombo.SelectedItem = "Normal";
            stageIButton.Checked = true;
        }

        /// <summary>
        /// The current stage as calculated from the form's input.
        /// </summary>
        public PiperStage CurrentStage
        {
            get
            {
                return currentStage;
            }
        }

        public RdaReduction CurrentReduction
        {
            get
            {
                return currentReduction;
            }
        }

        public ClockFace CurrentClockFace
        {
            get
            {
                return currentClockFace;
            }
        }

        private void computeDiscSpaceStage()
        {
            stageMap[PiperStage.I] = 0;
            stageMap[PiperStage.II] = 0;
            stageMap[PiperStage.IIIa] = 0;
            stageMap[PiperStage.IIIb] = 0;
            stageMap[PiperStage.IVa] = 0;
            stageMap[PiperStage.IVb] = 0;
            stageMap[PiperStage.Va] = 0;
            stageMap[PiperStage.Vb] = 0;

            clockFaceMap[ClockFace.Clock10] = 0;
            clockFaceMap[ClockFace.Clock11] = 0;
            clockFaceMap[ClockFace.Clock12] = 0;

            switch (verticalSpaceCombo.SelectedIndex)
            {
                case 0: //Normal
                    stageMap[PiperStage.I] += 1;
                    stageMap[PiperStage.II] += 1;
                    stageMap[PiperStage.IIIa] += 1;
                    stageMap[PiperStage.IIIb] += 1;
                    //stageMap[PiperStage.IVa] += 1;
                    clockFaceMap[ClockFace.Clock11] += 1;
                    break;
                case 1: //Increased
                    stageMap[PiperStage.IVa] += 1;
                    clockFaceMap[ClockFace.Clock11] += 1;
                    clockFaceMap[ClockFace.Clock12] += 2;
                    break;
                case 2: //Decreased
                    stageMap[PiperStage.IVa] += 1;
                    stageMap[PiperStage.IVb] += 1;
                    clockFaceMap[ClockFace.Clock11] += 1;
                    clockFaceMap[ClockFace.Clock10] += 2;
                    break;
                case 3: //Bone on Bone
                    stageMap[PiperStage.Va] += 10;
                    stageMap[PiperStage.Vb] += 10;
                    break;
            }

            switch (horizontalSpaceCombo.SelectedIndex)
            {
                case 0: //Normal
                    stageMap[PiperStage.I] += 1;
                    stageMap[PiperStage.II] += 1;
                    stageMap[PiperStage.IIIa] += 1;
                    stageMap[PiperStage.IIIb] += 1;
                    stageMap[PiperStage.IVa] += 1;
                    stageMap[PiperStage.IVb] += 1;
                    stageMap[PiperStage.Va] += 1;
                    stageMap[PiperStage.Vb] += 1;
                    break;
                case 1: //Posterior Shift
                    stageMap[PiperStage.IVa] += 2;
                    clockFaceMap[ClockFace.Clock11] += 2;
                    break;
            }

            switch (condyleShapeCombo.SelectedIndex)
            {
                case 0: //Normal
                    stageMap[PiperStage.I] += 1;
                    stageMap[PiperStage.II] += 1;
                    stageMap[PiperStage.IIIa] += 1;
                    stageMap[PiperStage.IIIb] += 1;
                    stageMap[PiperStage.IVa] += 1;
                    stageMap[PiperStage.IVb] += 1;
                    stageMap[PiperStage.Va] += 1;
                    break;
                case 1: //Osteophyte
                    stageMap[PiperStage.Vb] += 10;
                    break;
            }

            //Find the largest piper stage number
            int largest = 0;
            foreach (int value in stageMap.Values)
            {
                if (value > largest)
                {
                    largest = value;
                }
            }

            //Find all matching stages and return them
            PiperStage stages = PiperStage.Unknown;

            foreach (PiperStage stage in stageMap.Keys)
            {
                if (stageMap[stage] == largest)
                {
                    stages |= stage;
                }
            }

            //Find the largest clock face number
            largest = 0;
            foreach (int value in clockFaceMap.Values)
            {
                if (value > largest)
                {
                    largest = value;
                }
            }

            ClockFace clockFaces = ClockFace.Unknown;

            foreach (ClockFace clockFace in clockFaceMap.Keys)
            {
                if (clockFaceMap[clockFace] == largest)
                {
                    clockFaces |= clockFace;
                }
            }

            allowStageChangeEventFire = false;
            allowRdaReductionEventFire = false;
            allowClockChangeEventFire = false;

            //Update UI
            bool activatedLowest = false;
            processButton(stageIButton, (int)PiperStage.I, (int)stages, ref activatedLowest);
            processButton(stageIIButton, (int)PiperStage.II, (int)stages, ref activatedLowest);
            processButton(stageIIIaButton, (int)PiperStage.IIIa, (int)stages, ref activatedLowest);
            processButton(stageIIIbButton, (int)PiperStage.IIIb, (int)stages, ref activatedLowest);
            processButton(stageIVaButton, (int)PiperStage.IVa, (int)stages, ref activatedLowest);
            processButton(stageIVbButton, (int)PiperStage.IVb, (int)stages, ref activatedLowest);
            processButton(stageVaButton, (int)PiperStage.Va, (int)stages, ref activatedLowest);
            processButton(stageVbButton, (int)PiperStage.Vb, (int)stages, ref activatedLowest);

            activatedLowest = false;
            processButton(clock12Radio, (int)ClockFace.Clock12, (int)clockFaces, ref activatedLowest);
            processButton(clock11Radio, (int)ClockFace.Clock11, (int)clockFaces, ref activatedLowest);
            processButton(clock10Radio, (int)ClockFace.Clock10, (int)clockFaces, ref activatedLowest);

            allowStageChangeEventFire = true;
            allowRdaReductionEventFire = true;
            allowClockChangeEventFire = true;

            if (CurrentStageChanged != null)
            {
                CurrentStageChanged.Invoke(this, EventArgs.Empty);
            }
        }

        void processButton(KryptonRadioButton button, int checkStage, int stages, ref bool activatedLowest)
        {
            button.Enabled = (stages & checkStage) == checkStage;
            if (!activatedLowest && button.Enabled)
            {
                button.Checked = true;
                activatedLowest = true;
            }
        }

        void RDAReductionButton_CheckedChanged(object sender, EventArgs e)
        {
            RdaReduction oldReduction = currentReduction;
            if (mildRDAReductionButton.Checked)
            {
                currentReduction = RdaReduction.Mild;
            }
            else if (moderateRDAReductionButton.Checked)
            {
                currentReduction = RdaReduction.Moderate;
            }
            else if (severeRDAReductionButton.Checked)
            {
                currentReduction = RdaReduction.Severe;
            }
            if (allowRdaReductionEventFire && oldReduction != currentReduction)
            {
                if (CurrentStageChanged != null)
                {
                    CurrentStageChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        void clockRadio_CheckedChanged(object sender, EventArgs e)
        {
            ClockFace oldClockFace = currentClockFace;
            if (clock10Radio.Checked)
            {
                currentClockFace = ClockFace.Clock10;
                rdaReductionGroupBox.Enabled = true;
                mildRDAReductionButton.Checked = true;
            }
            else if (clock11Radio.Checked)
            {
                currentClockFace = ClockFace.Clock11;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (clock12Radio.Checked)
            {
                currentClockFace = ClockFace.Clock12;
                rdaReductionGroupBox.Enabled = false;
            }
            if (allowClockChangeEventFire && oldClockFace != currentClockFace)
            {
                if (CurrentStageChanged != null)
                {
                    CurrentStageChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        void stageButton_CheckedChanged(object sender, EventArgs e)
        {
            PiperStage oldStage = currentStage;
            if (stageIButton.Checked)
            {
                currentStage = PiperStage.I;
                rdaReductionGroupBox.Enabled = false;
                clockFaceGroupBox.Enabled = false;
            }
            else if (stageIIButton.Checked)
            {
                currentStage = PiperStage.II;
                rdaReductionGroupBox.Enabled = false;
                clockFaceGroupBox.Enabled = false;
            }
            else if (stageIIIaButton.Checked)
            {
                currentStage = PiperStage.IIIa;
                rdaReductionGroupBox.Enabled = false;
                clockFaceGroupBox.Enabled = false;
            }
            else if (stageIIIbButton.Checked)
            {
                currentStage = PiperStage.IIIb;
                rdaReductionGroupBox.Enabled = false;
                clockFaceGroupBox.Enabled = false;
            }
            else if (stageIVaButton.Checked)
            {
                allowRdaReductionEventFire = false;
                currentStage = PiperStage.IVa;
                rdaReductionGroupBox.Enabled = clock10Radio.Checked;
                mildRDAReductionButton.Checked = true;
                clockFaceGroupBox.Enabled = true;
                allowRdaReductionEventFire = true;
            }
            else if (stageIVbButton.Checked)
            {
                allowRdaReductionEventFire = false;
                currentStage = PiperStage.IVb;
                rdaReductionGroupBox.Enabled = false;
                clockFaceGroupBox.Enabled = false;
                allowRdaReductionEventFire = true;
            }
            else if (stageVaButton.Checked)
            {
                currentStage = PiperStage.Va;
                rdaReductionGroupBox.Enabled = false;
                clockFaceGroupBox.Enabled = false;
            }
            else if (stageVbButton.Checked)
            {
                currentStage = PiperStage.Vb;
                rdaReductionGroupBox.Enabled = false;
                clockFaceGroupBox.Enabled = false;
            }
            else
            {
                currentStage = PiperStage.Unknown;
            }
            if (currentStage != oldStage && allowStageChangeEventFire)
            {
                if (CurrentStageChanged != null)
                {
                    CurrentStageChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        void comboSelectionChanged(object sender, EventArgs e)
        {
            computeDiscSpaceStage();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to reset the disc space settings?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                this.setToDefault();
            }
        }
    }
}
