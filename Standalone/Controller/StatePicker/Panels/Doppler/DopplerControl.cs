using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

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
    public class DopplerControl
    {
        public event EventHandler CurrentStageChanged;

        PiperStage currentStage = PiperStage.Unknown;
        Dictionary<PiperStage, int> stageMap = new Dictionary<PiperStage, int>();
        private bool allowRdaReductionEventFire = true;
        private bool allowStageChangeEventFire = true;
        RdaReduction currentReduction = RdaReduction.Unknown;

        //UI
        ComboBox rotatoryCombo;
        ComboBox translatoryCombo;
        ComboBox clickCombo;

        Button stageIButton;
        Button stageIIButton;
        Button stageIIIaButton;
        Button stageIIIbButton;
        Button stageIVaButton;
        Button stageIVbButton;
        Button stageVaButton;
        Button stageVbButton;
        ButtonGroup stageGroup = new ButtonGroup();

        Button mildRDAReductionButton;
        Button moderateRDAReductionButton;
        Button severeRDAReductionButton;
        ButtonGroup rdaGroup = new ButtonGroup();

        Button resetButton;

        public DopplerControl(Widget widget)
        {
            stageMap.Add(PiperStage.I, 0);
            stageMap.Add(PiperStage.II, 0);
            stageMap.Add(PiperStage.IIIa, 0);
            stageMap.Add(PiperStage.IIIb, 0);
            stageMap.Add(PiperStage.IVa, 0);
            stageMap.Add(PiperStage.IVb, 0);
            stageMap.Add(PiperStage.Va, 0);
            stageMap.Add(PiperStage.Vb, 0);

            rotatoryCombo = widget.findWidget("DopplerPanel/RotatoryCombo") as ComboBox;
            translatoryCombo = widget.findWidget("DopplerPanel/TranslatoryCombo") as ComboBox;
            clickCombo = widget.findWidget("DopplerPanel/ClickCombo") as ComboBox;

            rotatoryCombo.EventComboChangePosition += rotatoryCombo_SelectedIndexChanged;
            translatoryCombo.EventComboChangePosition += translatoryCombo_SelectedIndexChanged;
            clickCombo.EventComboChangePosition += clickCombo_SelectedIndexChanged;

            stageIButton = widget.findWidget("DopplerPanel/StageI") as Button;
            stageIIButton = widget.findWidget("DopplerPanel/StageII") as Button;
            stageIIIaButton = widget.findWidget("DopplerPanel/StageIIIa") as Button;
            stageIIIbButton = widget.findWidget("DopplerPanel/StageIIIb") as Button;
            stageIVaButton = widget.findWidget("DopplerPanel/StageIVa") as Button;
            stageIVbButton = widget.findWidget("DopplerPanel/StageIVb") as Button;
            stageVaButton = widget.findWidget("DopplerPanel/StageVa") as Button;
            stageVbButton = widget.findWidget("DopplerPanel/StageVb") as Button;

            stageGroup.addButton(stageIButton);
            stageGroup.addButton(stageIIButton);
            stageGroup.addButton(stageIIIaButton);
            stageGroup.addButton(stageIIIbButton);
            stageGroup.addButton(stageIVaButton);
            stageGroup.addButton(stageIVbButton);
            stageGroup.addButton(stageVaButton);
            stageGroup.addButton(stageVbButton);
            stageGroup.SelectedButtonChanged += new EventHandler(stageGroup_SelectedButtonChanged);

            mildRDAReductionButton = widget.findWidget("DopplerPanel/MildRDA") as Button;
            moderateRDAReductionButton = widget.findWidget("DopplerPanel/ModerateRDA") as Button;
            severeRDAReductionButton = widget.findWidget("DopplerPanel/SevereRDA") as Button;

            rdaGroup.addButton(mildRDAReductionButton);
            rdaGroup.addButton(moderateRDAReductionButton);
            rdaGroup.addButton(severeRDAReductionButton);
            rdaGroup.Enabled = false;
            rdaGroup.SelectedButtonChanged += new EventHandler(rdaGroup_SelectedButtonChanged);

            resetButton = widget.findWidget("DopplerPanel/Reset") as Button;
            resetButton.MouseButtonClick += new MyGUIEvent(resetButton_MouseButtonClick);
        }

        public void setToDefault()
        {
            rotatoryCombo.SelectedIndex = rotatoryCombo.findItemIndexWith("Unknown");
            translatoryCombo.SelectedIndex = translatoryCombo.findItemIndexWith("Unknown");
            clickCombo.SelectedIndex = clickCombo.findItemIndexWith("Unknown");
            stageGroup.SelectedButton = stageIButton;
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
                    stageGroup.SelectedButton = stageIVaButton;
                }
                //Force stage IVb for others ignoring value under translatory
                else
                {
                    stageGroup.SelectedButton = stageIVbButton;
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

        void rdaGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            RdaReduction oldReduction = currentReduction;
            if (mildRDAReductionButton.StateCheck)
            {
                currentReduction = RdaReduction.Mild;
            }
            else if (moderateRDAReductionButton.StateCheck)
            {
                currentReduction = RdaReduction.Moderate;
            }
            else if (severeRDAReductionButton.StateCheck)
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

        void processButton(Button button, PiperStage checkStage, PiperStage stages, ref bool activatedLowest)
        {
            button.Enabled = (stages & checkStage) == checkStage;
            if (!activatedLowest && button.Enabled)
            {
                stageGroup.SelectedButton = button;
                activatedLowest = true;
            }
        }

        void stageGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            PiperStage oldStage = currentStage;
            if (stageIButton.StateCheck)
            {
                currentStage = PiperStage.I;
                rdaGroup.Enabled = false;
            }
            else if (stageIIButton.StateCheck)
            {
                currentStage = PiperStage.II;
                rdaGroup.Enabled = false;
            }
            else if (stageIIIaButton.StateCheck)
            {
                currentStage = PiperStage.IIIa;
                rdaGroup.Enabled = false;
            }
            else if (stageIIIbButton.StateCheck)
            {
                currentStage = PiperStage.IIIb;
                rdaGroup.Enabled = false;
            }
            else if (stageIVaButton.StateCheck)
            {
                allowRdaReductionEventFire = false;
                currentStage = PiperStage.IVa;
                rdaGroup.Enabled = true;
                rdaGroup.SelectedButton = mildRDAReductionButton;
                allowRdaReductionEventFire = true;
            }
            else if (stageIVbButton.StateCheck)
            {
                allowRdaReductionEventFire = false;
                currentStage = PiperStage.IVb;
                rdaGroup.Enabled = true;
                rdaGroup.SelectedButton = mildRDAReductionButton;
                allowRdaReductionEventFire = true;
            }
            else if (stageVaButton.StateCheck)
            {
                currentStage = PiperStage.Va;
                rdaGroup.Enabled = false;
            }
            else if (stageVbButton.StateCheck)
            {
                currentStage = PiperStage.Vb;
                rdaGroup.Enabled = false;
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

        void rotatoryCombo_SelectedIndexChanged(Widget sender, EventArgs e)
        {
            computeDopplerStage();
        }

        void clickCombo_SelectedIndexChanged(Widget sender, EventArgs e)
        {
            computeDopplerStage();
        }

        void translatoryCombo_SelectedIndexChanged(Widget sender, EventArgs e)
        {
            computeDopplerStage();
        }

        void resetButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure you want to reset the doppler settings?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                setToDefault();
            }
        }
    }
}
