using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public enum ClockFace
    {
        Unknown = 0,
        Clock12 = 1,
        Clock11 = 2,
        Clock10 = 4
    }

    public partial class DiscSpaceControl
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

        private ComboBox verticalSpaceCombo;
        private ComboBox horizontalSpaceCombo;
        private ComboBox condyleShapeCombo;

        private Button stageIButton;
        private Button stageIIButton;
        private Button stageIIIaButton;
        private Button stageIIIbButton;
        private Button stageIVaButton;
        private Button stageIVbButton;
        private Button stageVaButton;
        private Button stageVbButton;
        private ButtonGroup stageGroup = new ButtonGroup();

        private Button clock10Radio;
        private Button clock11Radio;
        private Button clock12Radio;
        private ButtonGroup clockGroup = new ButtonGroup();

        private Button mildRDAReductionButton;
        private Button moderateRDAReductionButton;
        private Button severeRDAReductionButton;
        private ButtonGroup rdaReductionGroup = new ButtonGroup();

        private Widget clockFacePanel;
        private Widget rdaPanel;

        public DiscSpaceControl(Widget widget)
        {
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

            verticalSpaceCombo = widget.findWidget("DiscSpace/VerticalSpaceCombo") as ComboBox;
            horizontalSpaceCombo = widget.findWidget("DiscSpace/HorizontalSpaceCombo") as ComboBox;
            condyleShapeCombo = widget.findWidget("DiscSpace/CondyleShapeCombo") as ComboBox;

            verticalSpaceCombo.EventComboChangePosition += new MyGUIEvent(comboSelectionChanged);
            horizontalSpaceCombo.EventComboChangePosition += new MyGUIEvent(comboSelectionChanged);
            condyleShapeCombo.EventComboChangePosition += new MyGUIEvent(comboSelectionChanged);

            stageIButton = widget.findWidget("DiscSpace/StageI") as Button;
            stageIIButton = widget.findWidget("DiscSpace/StageII") as Button;
            stageIIIaButton = widget.findWidget("DiscSpace/StageIIIa") as Button;
            stageIIIbButton = widget.findWidget("DiscSpace/StageIIIb") as Button;
            stageIVaButton = widget.findWidget("DiscSpace/StageIVa") as Button;
            stageIVbButton = widget.findWidget("DiscSpace/StageIVb") as Button;
            stageVaButton = widget.findWidget("DiscSpace/StageVa") as Button;
            stageVbButton = widget.findWidget("DiscSpace/StageVb") as Button;

            stageGroup.addButton(stageIButton);
            stageGroup.addButton(stageIIButton);
            stageGroup.addButton(stageIIIaButton);
            stageGroup.addButton(stageIIIbButton);
            stageGroup.addButton(stageIVaButton);
            stageGroup.addButton(stageIVbButton);
            stageGroup.addButton(stageVaButton);
            stageGroup.addButton(stageVbButton);
            stageGroup.SelectedButtonChanged += new EventHandler(stageButton_CheckedChanged);

            clock10Radio = widget.findWidget("DiscSpace/Clock10") as Button;
            clock11Radio = widget.findWidget("DiscSpace/Clock11") as Button;
            clock12Radio = widget.findWidget("DiscSpace/Clock12") as Button;

            clockGroup.addButton(clock10Radio);
            clockGroup.addButton(clock11Radio);
            clockGroup.addButton(clock12Radio);
            clockGroup.SelectedButtonChanged += new EventHandler(clockRadio_CheckedChanged);

            mildRDAReductionButton = widget.findWidget("DiscSpace/RDAMild") as Button;
            moderateRDAReductionButton = widget.findWidget("DiscSpace/RDAModerate") as Button;
            severeRDAReductionButton = widget.findWidget("DiscSpace/RDASevere") as Button;

            rdaReductionGroup.addButton(mildRDAReductionButton);
            rdaReductionGroup.addButton(moderateRDAReductionButton);
            rdaReductionGroup.addButton(severeRDAReductionButton);
            rdaReductionGroup.SelectedButtonChanged += new EventHandler(RDAReductionButton_CheckedChanged);

            clockFacePanel = widget.findWidget("DiscSpace/ClockFace");
            rdaPanel = widget.findWidget("DiscSpace/RDAReduction");

            rdaPanel.Visible = false;
            clockFacePanel.Visible = false;

            computeDiscSpaceStage();
        }

        public void setToDefault()
        {
            verticalSpaceCombo.SelectedIndex = 0;
            horizontalSpaceCombo.SelectedIndex = 0;
            condyleShapeCombo.SelectedIndex = 0;
            stageGroup.SelectedButton = stageIButton;
            computeDiscSpaceStage();
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
            processStageButton(stageIButton, (int)PiperStage.I, (int)stages, ref activatedLowest);
            processStageButton(stageIIButton, (int)PiperStage.II, (int)stages, ref activatedLowest);
            processStageButton(stageIIIaButton, (int)PiperStage.IIIa, (int)stages, ref activatedLowest);
            processStageButton(stageIIIbButton, (int)PiperStage.IIIb, (int)stages, ref activatedLowest);
            processStageButton(stageIVaButton, (int)PiperStage.IVa, (int)stages, ref activatedLowest);
            processStageButton(stageIVbButton, (int)PiperStage.IVb, (int)stages, ref activatedLowest);
            processStageButton(stageVaButton, (int)PiperStage.Va, (int)stages, ref activatedLowest);
            processStageButton(stageVbButton, (int)PiperStage.Vb, (int)stages, ref activatedLowest);

            activatedLowest = false;
            processClockButton(clock12Radio, (int)ClockFace.Clock12, (int)clockFaces, ref activatedLowest);
            processClockButton(clock11Radio, (int)ClockFace.Clock11, (int)clockFaces, ref activatedLowest);
            processClockButton(clock10Radio, (int)ClockFace.Clock10, (int)clockFaces, ref activatedLowest);

            allowStageChangeEventFire = true;
            allowRdaReductionEventFire = true;
            allowClockChangeEventFire = true;

            if (CurrentStageChanged != null)
            {
                CurrentStageChanged.Invoke(this, EventArgs.Empty);
            }
        }

        void processStageButton(Button button, int checkStage, int stages, ref bool activatedLowest)
        {
            button.Enabled = (stages & checkStage) == checkStage;
            if (!activatedLowest && button.Enabled)
            {
                stageGroup.SelectedButton = button;
                activatedLowest = true;
            }
        }

        void processClockButton(Button button, int checkStage, int stages, ref bool activatedLowest)
        {
            button.Enabled = (stages & checkStage) == checkStage;
            if (!activatedLowest && button.Enabled)
            {
                clockGroup.SelectedButton = button;
                activatedLowest = true;
            }
        }

        void RDAReductionButton_CheckedChanged(object sender, EventArgs e)
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

        void clockRadio_CheckedChanged(object sender, EventArgs e)
        {
            ClockFace oldClockFace = currentClockFace;
            if (clock10Radio.StateCheck)
            {
                currentClockFace = ClockFace.Clock10;
                rdaPanel.Visible = true;
                rdaReductionGroup.SelectedButton = mildRDAReductionButton;
            }
            else if (clock11Radio.StateCheck)
            {
                currentClockFace = ClockFace.Clock11;
                rdaPanel.Visible = false;
            }
            else if (clock12Radio.StateCheck)
            {
                currentClockFace = ClockFace.Clock12;
                rdaPanel.Visible = false;
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
            if (stageIButton.StateCheck)
            {
                currentStage = PiperStage.I;
                rdaPanel.Visible = false;
                clockFacePanel.Visible = false;
            }
            else if (stageIIButton.StateCheck)
            {
                currentStage = PiperStage.II;
                rdaPanel.Visible = false;
                clockFacePanel.Visible = false;
            }
            else if (stageIIIaButton.StateCheck)
            {
                currentStage = PiperStage.IIIa;
                rdaPanel.Visible = false;
                clockFacePanel.Visible = false;
            }
            else if (stageIIIbButton.StateCheck)
            {
                currentStage = PiperStage.IIIb;
                rdaPanel.Visible = false;
                clockFacePanel.Visible = false;
            }
            else if (stageIVaButton.StateCheck)
            {
                allowRdaReductionEventFire = false;
                currentStage = PiperStage.IVa;
                rdaPanel.Visible = clock10Radio.StateCheck;
                rdaReductionGroup.SelectedButton = mildRDAReductionButton;
                clockFacePanel.Visible = true;
                allowRdaReductionEventFire = true;
            }
            else if (stageIVbButton.StateCheck)
            {
                allowRdaReductionEventFire = false;
                currentStage = PiperStage.IVb;
                rdaPanel.Visible = false;
                clockFacePanel.Visible = false;
                allowRdaReductionEventFire = true;
            }
            else if (stageVaButton.StateCheck)
            {
                currentStage = PiperStage.Va;
                rdaPanel.Visible = false;
                clockFacePanel.Visible = false;
            }
            else if (stageVbButton.StateCheck)
            {
                currentStage = PiperStage.Vb;
                rdaPanel.Visible = false;
                clockFacePanel.Visible = false;
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

        void comboSelectionChanged(Widget sender, EventArgs e)
        {
            computeDiscSpaceStage();
        }

        void resetButton_Click(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to reset the disc space settings?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doMakeNormalButtonClick);
        }

        private void doMakeNormalButtonClick(MessageBoxStyle style)
        {
            if (style == MessageBoxStyle.Yes)
            {
                this.setToDefault();
            }
        }
    }
}
