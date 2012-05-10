using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving.XMLSaver;
using System.IO;
using System.Xml;
using Logging;
using MyGUIPlugin;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class DiscSpaceGUI : WizardComponent<DiscSpaceView>
    {
        private Vector3 openingStateOffset = Vector3.Zero;

        private static XmlSaver xmlSaver = new XmlSaver();
        private PresetState presetState;
        private String subDirectory;

        private DiscSpaceControl discSpaceControl;
        private CheckButton showDiscCheckBox;

        public DiscSpaceGUI(String subDirectory, DiscSpaceView wizardView, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.AnomalousMvc.DistortionWizard.DiscSpace.DiscSpaceGUI.layout", wizardView, context, viewHost)
        {
            this.subDirectory = subDirectory;

            discSpaceControl = new DiscSpaceControl(widget);
            discSpaceControl.CurrentStageChanged += new EventHandler(discSpaceControl1_CurrentStageChanged);

            showDiscCheckBox = new CheckButton(widget.findWidget("DiscSpace/ShowDisc") as Button);
            showDiscCheckBox.CheckedChanged += new MyGUIEvent(showDiscCheckBox_CheckedChanged);

            Button resetButton = widget.findWidget("DiscSpace/Reset") as Button;
            resetButton.MouseButtonClick += new MyGUIEvent(resetButton_MouseButtonClick);
        }

        void discSpaceControl1_CurrentStageChanged(object sender, EventArgs e)
        {
            String filename = "I.dst";
            switch (discSpaceControl.CurrentStage)
            {
                case PiperStage.I:
                    filename = "I.dst";
                    break;
                case PiperStage.II:
                    filename = "II.dst";
                    break;
                case PiperStage.IIIa:
                    filename = "IIIa.dst";
                    break;
                case PiperStage.IIIb:
                    filename = "IIIb.dst";
                    break;
                case PiperStage.IVa:
                    switch (discSpaceControl.CurrentClockFace)
                    {
                        case ClockFace.Clock12:
                            filename = "IVa12.dst";
                            break;
                        case ClockFace.Clock11:
                            filename = "IVa11.dst";
                            break;
                        case ClockFace.Clock10:
                            switch (discSpaceControl.CurrentReduction)
                            {
                                case RdaReduction.Mild:
                                    filename = "IVa10MildRDAReduction.dst";
                                    break;
                                case RdaReduction.Moderate:
                                    filename = "IVa10ModerateRDAReduction.dst";
                                    break;
                                case RdaReduction.Severe:
                                    filename = "IVa10SevereRDAReduction.dst";
                                    break;
                            }
                            break;
                    }
                    break;
                case PiperStage.IVb:
                    filename = "IVb.dst";
                    break;
                case PiperStage.Va:
                    filename = "Va.dst";
                    break;
                case PiperStage.Vb:
                    filename = "Vb.dst";
                    break;
            }
            filename = String.Format("{0}/{1}", subDirectory, filename);
            if (context.ResourceProvider.exists(filename))
            {
                using (Stream stream = context.ResourceProvider.openFile(filename))
                {
                    using (XmlTextReader textReader = new XmlTextReader(stream))
                    {
                        presetState = xmlSaver.restoreObject(textReader) as PresetState;
                        context.applyPresetState(presetState, 1.0f);
                    }
                }
            }
            else
            {
                presetState = null;
                Log.Error("Cannot load doppler distortion file {0}.", filename);
            }
        }

        public override void opening()
        {
            showDiscCheckBox.Checked = false;
            discSpaceControl.setToDefault();
            String filename = String.Format("{0}/{1}", subDirectory, "I.dst");
            if (context.ResourceProvider.exists(filename))
            {
                using (Stream stream = context.ResourceProvider.openFile(filename))
                {
                    using (XmlTextReader textReader = new XmlTextReader(stream))
                    {
                        presetState = xmlSaver.restoreObject(textReader) as PresetState;
                    }
                }
            }
            else
            {
                presetState = null;
                Log.Error("Cannot load disc space distortion file {0}.", filename);
            }
            base.opening();
        }

        void resetButton_MouseButtonClick(Widget source, EventArgs e)
        {
            discSpaceControl.setToDefault();
        }

        void showDiscCheckBox_CheckedChanged(Widget source, EventArgs e)
        {
            if (showDiscCheckBox.Checked)
            {
                context.runAction(wizardView.ShowDiscAction, ViewHost);
            }
            else
            {
                context.runAction(wizardView.NormalViewAction, ViewHost);
            }
        }
    }
}
