using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Logging;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    /// <summary>
    /// An enum of piper stages.
    /// </summary>
    public enum PiperStage
    {
        Unknown = 0,
        I = 1,
        II = 2,
        IIIa = 4,
        IIIb = 8,
        IVa = 16,
        IVb = 32,
        Va = 64,
        Vb = 128,
    }

    public enum RdaReduction
    {
        Mild,
        Moderate,
        Severe,
        Unknown
    }

    /// <summary>
    /// A reuseable control that can compute doppler stages.
    /// </summary>
    public partial class DopplerControl : UserControl
    {
        public event EventHandler CurrentStageChanged;

        PiperStage currentStage = PiperStage.Unknown;
        Dictionary<PiperStage, int> stageMap = new Dictionary<PiperStage, int>();
        private bool allowRdaReductionEventFire = true;
        private bool allowStageChangeEventFire = true;
        RdaReduction currentReduction = RdaReduction.Unknown;

        public DopplerControl()
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

            rotatoryCombo.SelectedIndexChanged += new EventHandler(rotatoryCombo_SelectedIndexChanged);
            translatoryCombo.SelectedIndexChanged += new EventHandler(translatoryCombo_SelectedIndexChanged);
            clickCombo.SelectedIndexChanged += new EventHandler(clickCombo_SelectedIndexChanged);

            stageIButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIIButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIIIaButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIIIbButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIVaButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageIVbButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageVaButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);
            stageVbButton.CheckedChanged += new EventHandler(stageButton_CheckedChanged);

            mildRDAReductionButton.CheckedChanged += new EventHandler(RDAReductionButton_CheckedChanged);
            moderateRDAReductionButton.CheckedChanged += new EventHandler(RDAReductionButton_CheckedChanged);
            severeRDAReductionButton.CheckedChanged += new EventHandler(RDAReductionButton_CheckedChanged);

            rdaReductionGroupBox.Enabled = false;
        }

        public void setToDefault()
        {
            rotatoryCombo.SelectedItem = "Unknown";
            translatoryCombo.SelectedItem = "Unknown";
            clickCombo.SelectedItem = "Unknown";
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

        private PiperStage computeDopplerStage()
        {
            stageMap[PiperStage.I] = 0;
            stageMap[PiperStage.II] = 0;
            stageMap[PiperStage.IIIa] = 0;
            stageMap[PiperStage.IIIb] = 0;
            stageMap[PiperStage.IVa] = 0;
            stageMap[PiperStage.IVb] = 0;
            stageMap[PiperStage.Va] = 0;
            stageMap[PiperStage.Vb] = 0;

            switch (rotatoryCombo.SelectedIndex)
            {
                case 0: //None
                    stageMap[PiperStage.I] += 2;
                    stageMap[PiperStage.II] += 2;
                    stageMap[PiperStage.IIIa] += 2;
                    stageMap[PiperStage.IIIb] += 2;
                    break;
                case 1: //Mild
                    stageMap[PiperStage.I] += 1;
                    stageMap[PiperStage.II] += 1;
                    stageMap[PiperStage.IIIa] += 1;
                    stageMap[PiperStage.IIIb] += 1;
                    stageMap[PiperStage.IVa] += 1;
                    stageMap[PiperStage.IVb] += 1;
                    break;
                case 2: //Moderate
                    stageMap[PiperStage.IVa] += 2;
                    stageMap[PiperStage.IVb] += 2;
                    stageMap[PiperStage.Va] += 1;
                    stageMap[PiperStage.Vb] += 1;
                    break;
                case 3: //Coarse Rough
                    stageMap[PiperStage.Va] += 10;
                    break;
                case 4: //Coarse Eburnated
                    stageMap[PiperStage.Vb] += 10;
                    break;
            }

            switch (translatoryCombo.SelectedIndex)
            {
                case 0: //None
                    stageMap[PiperStage.I] += 2;
                    stageMap[PiperStage.II] += 1;
                    break;
                case 1: //Mild
                    stageMap[PiperStage.I] += 1;
                    stageMap[PiperStage.II] += 2;
                    stageMap[PiperStage.IIIa] += 1;
                    stageMap[PiperStage.IIIb] += 1;
                    stageMap[PiperStage.IVa] += 1;
                    stageMap[PiperStage.IVb] += 1;
                    break;
                case 2: //Moderate
                    stageMap[PiperStage.IIIa] += 2;
                    stageMap[PiperStage.IIIb] += 2;
                    stageMap[PiperStage.IVa] += 2;
                    stageMap[PiperStage.IVb] += 2;
                    break;
                case 3: //Coarse Rough
                    stageMap[PiperStage.Va] += 10;
                    break;
                case 4: //Coarse Eburnated
                    stageMap[PiperStage.Vb] += 10;
                    break;
            }

            switch (clickCombo.SelectedIndex)
            {
                case 0: //None
                    stageMap[PiperStage.I] += 2;
                    stageMap[PiperStage.II] += 1;
                    stageMap[PiperStage.IIIb] += 2;
                    stageMap[PiperStage.IVb] += 2;
                    stageMap[PiperStage.Va] += 2;
                    stageMap[PiperStage.Vb] += 2;
                    break;
                case 1: //Reciprocal
                    stageMap[PiperStage.II] += 2;
                    stageMap[PiperStage.IIIa] += 2;
                    stageMap[PiperStage.IVa] += 2;
                    break;
                case 2: //Surface
                    stageMap[PiperStage.Va] += 1;
                    stageMap[PiperStage.Vb] += 1;
                    break;
            }

            //Find the largest number
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

            allowStageChangeEventFire = false;
            allowRdaReductionEventFire = false;

            //Update UI
            bool activatedLowest = false;
            processButton(stageIButton, PiperStage.I, stages, ref activatedLowest);
            processButton(stageIIButton, PiperStage.II, stages, ref activatedLowest);
            processButton(stageIIIaButton, PiperStage.IIIa, stages, ref activatedLowest);
            processButton(stageIIIbButton, PiperStage.IIIb, stages, ref activatedLowest);
            processButton(stageIVaButton, PiperStage.IVa, stages, ref activatedLowest);
            processButton(stageIVbButton, PiperStage.IVb, stages, ref activatedLowest);
            processButton(stageVaButton, PiperStage.Va, stages, ref activatedLowest);
            processButton(stageVbButton, PiperStage.Vb, stages, ref activatedLowest);

            //Override the settings with any custom overrides
            //If rotatory crepitus is Moderate and we do not have coarse rough or coarse ebrunated
            if (rotatoryCombo.SelectedIndex == 2 && translatoryCombo.SelectedIndex != 3 && translatoryCombo.SelectedIndex != 4)
            {
                //Force stage IVa if reciprocal click
                if (clickCombo.SelectedIndex == 1)
                {
                    stageIVaButton.Checked = true;
                }
                //Force stage IVb for others ignoring value under translatory
                else
                {
                    stageIVbButton.Checked = true;
                    stageIVbButton.Enabled = true;
                }
            }

            allowStageChangeEventFire = true;
            allowRdaReductionEventFire = true;

            if (CurrentStageChanged != null)
            {
                CurrentStageChanged.Invoke(this, EventArgs.Empty);
            }

            return stages;
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

        void processButton(KryptonRadioButton button, PiperStage checkStage, PiperStage stages, ref bool activatedLowest)
        {
            button.Enabled = (stages & checkStage) == checkStage;
            if (!activatedLowest && button.Enabled)
            {
                button.Checked = true;
                activatedLowest = true;
            }
        }

        void stageButton_CheckedChanged(object sender, EventArgs e)
        {
            PiperStage oldStage = currentStage;
            if (stageIButton.Checked)
            {
                currentStage = PiperStage.I;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageIIButton.Checked)
            {
                currentStage = PiperStage.II;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageIIIaButton.Checked)
            {
                currentStage = PiperStage.IIIa;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageIIIbButton.Checked)
            {
                currentStage = PiperStage.IIIb;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageIVaButton.Checked)
            {
                allowRdaReductionEventFire = false;
                currentStage = PiperStage.IVa;
                rdaReductionGroupBox.Enabled = true;
                mildRDAReductionButton.Checked = true;
                allowRdaReductionEventFire = true;
            }
            else if (stageIVbButton.Checked)
            {
                allowRdaReductionEventFire = false;
                currentStage = PiperStage.IVb;
                rdaReductionGroupBox.Enabled = true;
                mildRDAReductionButton.Checked = true;
                allowRdaReductionEventFire = true;
            }
            else if (stageVaButton.Checked)
            {
                currentStage = PiperStage.Va;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageVbButton.Checked)
            {
                currentStage = PiperStage.Vb;
                rdaReductionGroupBox.Enabled = false;
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

        void rotatoryCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            computeDopplerStage();
        }

        void clickCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            computeDopplerStage();
        }

        void translatoryCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            computeDopplerStage();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to reset the doppler settings?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                setToDefault();
            }
        }
    }
}
