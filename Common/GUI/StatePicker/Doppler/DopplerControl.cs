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
    /// An enum of doppler stages.
    /// </summary>
    public enum DopplerStage
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

        DopplerStage currentStage = DopplerStage.Unknown;
        Dictionary<DopplerStage, int> stageMap = new Dictionary<DopplerStage, int>();
        private bool allowRdaReductionEventFire = true;
        private bool allowStageChangeEventFire = true;
        RdaReduction currentReduction = RdaReduction.Unknown;

        public DopplerControl()
        {
            InitializeComponent();
            stageMap.Add(DopplerStage.I, 0);
            stageMap.Add(DopplerStage.II, 0);
            stageMap.Add(DopplerStage.IIIa, 0);
            stageMap.Add(DopplerStage.IIIb, 0);
            stageMap.Add(DopplerStage.IVa, 0);
            stageMap.Add(DopplerStage.IVb, 0);
            stageMap.Add(DopplerStage.Va, 0);
            stageMap.Add(DopplerStage.Vb, 0);

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
        public DopplerStage CurrentStage
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

        private DopplerStage computeDopplerStage()
        {
            stageMap[DopplerStage.I] = 0;
            stageMap[DopplerStage.II] = 0;
            stageMap[DopplerStage.IIIa] = 0;
            stageMap[DopplerStage.IIIb] = 0;
            stageMap[DopplerStage.IVa] = 0;
            stageMap[DopplerStage.IVb] = 0;
            stageMap[DopplerStage.Va] = 0;
            stageMap[DopplerStage.Vb] = 0;

            switch (rotatoryCombo.SelectedIndex)
            {
                case 0: //None
                    stageMap[DopplerStage.I] += 2;
                    stageMap[DopplerStage.II] += 2;
                    stageMap[DopplerStage.IIIa] += 2;
                    stageMap[DopplerStage.IIIb] += 2;
                    break;
                case 1: //Mild
                    stageMap[DopplerStage.I] += 1;
                    stageMap[DopplerStage.II] += 1;
                    stageMap[DopplerStage.IIIa] += 1;
                    stageMap[DopplerStage.IIIb] += 1;
                    stageMap[DopplerStage.IVa] += 1;
                    stageMap[DopplerStage.IVb] += 1;
                    break;
                case 2: //Moderate
                    stageMap[DopplerStage.IVa] += 2;
                    stageMap[DopplerStage.IVb] += 2;
                    stageMap[DopplerStage.Va] += 1;
                    stageMap[DopplerStage.Vb] += 1;
                    break;
                case 3: //Coarse Rough
                    stageMap[DopplerStage.Va] += 2;
                    break;
                case 4: //Coarse Eburnated
                    stageMap[DopplerStage.Vb] += 2;
                    break;
            }

            switch (translatoryCombo.SelectedIndex)
            {
                case 0: //None
                    stageMap[DopplerStage.I] += 2;
                    stageMap[DopplerStage.II] += 1;
                    break;
                case 1: //Mild
                    stageMap[DopplerStage.I] += 1;
                    stageMap[DopplerStage.II] += 2;
                    stageMap[DopplerStage.IIIa] += 1;
                    stageMap[DopplerStage.IIIb] += 1;
                    stageMap[DopplerStage.IVa] += 1;
                    stageMap[DopplerStage.IVb] += 1;
                    break;
                case 2: //Moderate
                    stageMap[DopplerStage.IIIa] += 2;
                    stageMap[DopplerStage.IIIb] += 2;
                    stageMap[DopplerStage.IVa] += 2;
                    stageMap[DopplerStage.IVb] += 2;
                    break;
                case 3: //Coarse Rough
                    stageMap[DopplerStage.Va] += 2;
                    break;
                case 4: //Coarse Eburnated
                    stageMap[DopplerStage.Vb] += 2;
                    break;
            }

            switch (clickCombo.SelectedIndex)
            {
                case 0: //None
                    stageMap[DopplerStage.I] += 2;
                    stageMap[DopplerStage.II] += 1;
                    stageMap[DopplerStage.IIIb] += 2;
                    stageMap[DopplerStage.IVb] += 2;
                    stageMap[DopplerStage.Va] += 2;
                    stageMap[DopplerStage.Vb] += 2;
                    break;
                case 1: //Reciprocal
                    stageMap[DopplerStage.II] += 2;
                    stageMap[DopplerStage.IIIa] += 2;
                    stageMap[DopplerStage.IVa] += 2;
                    break;
                case 2: //Surface
                    stageMap[DopplerStage.Va] += 1;
                    stageMap[DopplerStage.Vb] += 1;
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
            DopplerStage stages = DopplerStage.Unknown;

            foreach (DopplerStage stage in stageMap.Keys)
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
            processButton(stageIButton, DopplerStage.I, stages, ref activatedLowest);
            processButton(stageIIButton, DopplerStage.II, stages, ref activatedLowest);
            processButton(stageIIIaButton, DopplerStage.IIIa, stages, ref activatedLowest);
            processButton(stageIIIbButton, DopplerStage.IIIb, stages, ref activatedLowest);
            processButton(stageIVaButton, DopplerStage.IVa, stages, ref activatedLowest);
            processButton(stageIVbButton, DopplerStage.IVb, stages, ref activatedLowest);
            processButton(stageVaButton, DopplerStage.Va, stages, ref activatedLowest);
            processButton(stageVbButton, DopplerStage.Vb, stages, ref activatedLowest);

            //Override the settings with any custom overrides
            //If rotatory crepitus is Moderate
            if (rotatoryCombo.SelectedIndex == 2)
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

        void processButton(KryptonRadioButton button, DopplerStage checkStage, DopplerStage stages, ref bool activatedLowest)
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
            DopplerStage oldStage = currentStage;
            if (stageIButton.Checked)
            {
                currentStage = DopplerStage.I;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageIIButton.Checked)
            {
                currentStage = DopplerStage.II;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageIIIaButton.Checked)
            {
                currentStage = DopplerStage.IIIa;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageIIIbButton.Checked)
            {
                currentStage = DopplerStage.IIIb;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageIVaButton.Checked)
            {
                allowRdaReductionEventFire = false;
                currentStage = DopplerStage.IVa;
                rdaReductionGroupBox.Enabled = true;
                mildRDAReductionButton.Checked = true;
                allowRdaReductionEventFire = true;
            }
            else if (stageIVbButton.Checked)
            {
                allowRdaReductionEventFire = false;
                currentStage = DopplerStage.IVb;
                rdaReductionGroupBox.Enabled = true;
                mildRDAReductionButton.Checked = true;
                allowRdaReductionEventFire = true;
            }
            else if (stageVaButton.Checked)
            {
                currentStage = DopplerStage.Va;
                rdaReductionGroupBox.Enabled = false;
            }
            else if (stageVbButton.Checked)
            {
                currentStage = DopplerStage.Vb;
                rdaReductionGroupBox.Enabled = false;
            }
            else
            {
                currentStage = DopplerStage.Unknown;
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
            setToDefault();
        }
    }
}
