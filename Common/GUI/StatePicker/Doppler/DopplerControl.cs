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

    /// <summary>
    /// A reuseable control that can compute doppler stages.
    /// </summary>
    public partial class DopplerControl : UserControl
    {
        DopplerStage currentStage = DopplerStage.Unknown;
        Dictionary<DopplerStage, int> stageMap = new Dictionary<DopplerStage, int>();

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

            //Log.Debug("Stage {0}", stages);

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

            return stages;
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
    }
}
