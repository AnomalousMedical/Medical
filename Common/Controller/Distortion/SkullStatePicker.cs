#define USE_SLIDER_GUIS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Resources;
using System.Xml;
using Logging;
using Engine.Saving.XMLSaver;
using Medical.GUI;
using Engine.ObjectManagement;
using Medical.Properties;
using System.Drawing;

namespace Medical
{
    public class SkullStatePicker : DistortionWizard
    {
        private TemporaryStateBlender temporaryStateBlender;
        private StatePickerWizard statePicker;

        private FossaPanel leftFossaPanel;
        private FossaPanel rightFossaPanel;
        private PresetStatePanel leftDiscPanel;
        private PresetStatePanel rightDiscPanel;

        private XmlSaver saver = new XmlSaver();

        private String lastRootDirectory;

        public SkullStatePicker(StatePickerPanelController panelController)
        {
            temporaryStateBlender = new TemporaryStateBlender(panelController.MedicalController.MainTimer, panelController.StateController);
            statePicker = new StatePickerWizard(Name, panelController.UiHost, temporaryStateBlender, panelController.NavigationController, panelController.LayerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftCondylarGrowth));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftCondylarDegeneration));

            leftDiscPanel = new PresetStatePanel();
            leftDiscPanel.Text = "Left Disc";
            leftDiscPanel.NavigationState = "Left TMJ";
            leftDiscPanel.LayerState = "DiscLayers";
            leftDiscPanel.TextLine1 = "Left TMJ";
            leftDiscPanel.LargeIcon = Resources.LeftDiscPosition;
            statePicker.addStatePanel(leftDiscPanel);

            leftFossaPanel = new FossaPanel("LeftFossa");
            leftFossaPanel.NormalImage = Resources.LeftFossaNormal;
            leftFossaPanel.DistortedImage = Resources.LeftFossaFlat;
            leftFossaPanel.NavigationState = "Left TMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            leftFossaPanel.Text = "Left Fossa";
            leftFossaPanel.TextLine1 = "Left Fossa";
            leftFossaPanel.LargeIcon = Resources.LeftFossaFlatness;
            statePicker.addStatePanel(leftFossaPanel);

            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightCondylarGrowth));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightCondylarDegeneration));

            rightDiscPanel = new PresetStatePanel();
            rightDiscPanel.Text = "Right Disc";
            rightDiscPanel.NavigationState = "Right TMJ";
            rightDiscPanel.LayerState = "DiscLayers";
            rightDiscPanel.TextLine1 = "Right TMJ";
            rightDiscPanel.LargeIcon = Resources.RightDiscPosition;
            statePicker.addStatePanel(rightDiscPanel);

            rightFossaPanel = new FossaPanel("RightFossa");
            rightFossaPanel.NormalImage = Resources.RightFossaNormal;
            rightFossaPanel.DistortedImage = Resources.RightFossaFlat;
            rightFossaPanel.NavigationState = "Right TMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            rightFossaPanel.Text = "Right Fossa";
            rightFossaPanel.TextLine1 = "Right Fossa";
            rightFossaPanel.LargeIcon = Resources.RightFossaFlatness;
            statePicker.addStatePanel(rightFossaPanel);

            statePicker.addStatePanel(new BottomTeethRemovalPanel());
            statePicker.addStatePanel(new TopTeethRemovalPanel());
            statePicker.addStatePanel(new TeethAdaptationPanel());
            statePicker.addStatePanel(new NotesPanel("MRI", panelController.ImageRenderer));

            statePicker.initializeImageHandle();
            statePicker.setToDefault();
        }

        public override void Dispose()
        {
            if (statePicker != null)
            {
                statePicker.Dispose();
            }
        }

        public override void sceneChanged(SimScene scene, String rootDirectory)
        {
            if (statePicker.Visible)
            {
                statePicker.closeForSceneChange();
            }

            leftFossaPanel.sceneLoaded(scene);
            rightFossaPanel.sceneLoaded(scene);
            if (rootDirectory != lastRootDirectory)
            {
                lastRootDirectory = rootDirectory;

                PresetStateSet leftDisc = new PresetStateSet("Left Disc", rootDirectory + "/LeftDisc");
                loadPresetSet(leftDisc);
                leftDiscPanel.clear();
                leftDiscPanel.initialize(leftDisc);

                PresetStateSet rightDisc = new PresetStateSet("Right Disc", rootDirectory + "/RightDisc");
                loadPresetSet(rightDisc);
                rightDiscPanel.clear();
                rightDiscPanel.initialize(rightDisc);
            }
        }

        public override void setToDefault()
        {
            statePicker.setToDefault();
        }

        public override void startWizard(DrawingWindow window)
        {
            statePicker.startWizard(window);
            statePicker.show();
        }

        public override String Name
        {
            get
            {
                return "MRI Wizard";
            }
        }

        private void loadPresetSet(PresetStateSet presetStateSet)
        {
            using (Archive archive = FileSystem.OpenArchive(presetStateSet.SourceDirectory))
            {
                String[] files = archive.listFiles(presetStateSet.SourceDirectory, "*.pre", false);
                foreach (String file in files)
                {
                    XmlTextReader reader = new XmlTextReader(archive.openStream(file, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read));
                    PresetState preset = saver.restoreObject(reader) as PresetState;
                    if (preset != null)
                    {
                        presetStateSet.addPresetState(preset);
                    }
                    else
                    {
                        Log.Error("Could not load preset from file {0}. Object was not a BoneManipulatorPresetState.", file);
                    }
                    reader.Close();
                }
            }
        }

        void statePicker_StateCreated(MedicalState state)
        {
            alertStateCreated(state);
        }

        void statePicker_Finished()
        {
            alertWizardFinished();
        }

        public override String TextLine1
        {
            get
            {
                return "MRI Wizard";
            }
        }

        public override String TextLine2
        {
            get
            {
                return "";
            }
        }

        public override Image ImageLarge
        {
            get
            {
                return Resources.MRIWizardLarge;
            }
        }
    }
}
