using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using Engine.Resources;
using System.Xml;
using Logging;
using Engine.Saving.XMLSaver;
using Medical.GUI;

namespace Medical.GUI
{
    public class SkullStatePicker : IDisposable
    {
        public event MedicalStateCreated StateCreated;
        public event StatePickerFinished Finished;

        private TemporaryStateBlender temporaryStateBlender;
        private StatePickerController statePicker;

        private PresetStatePanel leftGrowthPanel;
        private PresetStatePanel rightGrowthPanel;
        private PresetStatePanel leftDegenerationPanel;
        private PresetStatePanel rightDegenerationPanel;
        private PresetStatePanel leftDiscPanel;
        private PresetStatePanel rightDiscPanel;
        private PresetStatePanel leftFossaPanel;
        private PresetStatePanel rightFossaPanel;

        private XmlSaver saver = new XmlSaver();

        public SkullStatePicker(DockPanel dockPanel, ToolStripContainer toolStrip, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController)
        {
            temporaryStateBlender = new TemporaryStateBlender(medicalController.MainTimer, stateController);
            statePicker = new StatePickerController(new GUIElementController(dockPanel, toolStrip, medicalController), temporaryStateBlender, navigationController, layerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            leftGrowthPanel = new PresetStatePanel();
            leftGrowthPanel.Text = "Left Condyle Growth";
            leftGrowthPanel.NavigationState = "Left Lateral";
            leftGrowthPanel.LayerState = "MandibleSizeLayers";
            statePicker.addStatePanel(leftGrowthPanel);

            leftDegenerationPanel = new PresetStatePanel();
            leftDegenerationPanel.Text = "Left Condyle Degeneration";
            leftDegenerationPanel.NavigationState = "Left TMJ";
            leftDegenerationPanel.LayerState = "MandibleSizeLayers";
            statePicker.addStatePanel(leftDegenerationPanel);

            leftDiscPanel = new PresetStatePanel();
            leftDiscPanel.Text = "Left Disc";
            leftDiscPanel.NavigationState = "Left TMJ";
            leftDiscPanel.LayerState = "DiscLayers";
            statePicker.addStatePanel(leftDiscPanel);

            leftFossaPanel = new PresetStatePanel();
            leftFossaPanel.Text = "Left Fossa";
            leftFossaPanel.NavigationState = "Left TMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            statePicker.addStatePanel(leftFossaPanel);

            rightGrowthPanel = new PresetStatePanel();
            rightGrowthPanel.Text = "Right Condyle Growth";
            rightGrowthPanel.NavigationState = "Right Lateral";
            rightGrowthPanel.LayerState = "MandibleSizeLayers";
            statePicker.addStatePanel(rightGrowthPanel);

            rightDegenerationPanel = new PresetStatePanel();
            rightDegenerationPanel.Text = "Right Condyle Degeneration";
            rightDegenerationPanel.NavigationState = "Right TMJ";
            rightDegenerationPanel.LayerState = "MandibleSizeLayers";
            statePicker.addStatePanel(rightDegenerationPanel);

            rightDiscPanel = new PresetStatePanel();
            rightDiscPanel.Text = "Right Disc";
            rightDiscPanel.NavigationState = "Right TMJ";
            rightDiscPanel.LayerState = "DiscLayers";
            statePicker.addStatePanel(rightDiscPanel);

            rightFossaPanel = new PresetStatePanel();
            rightFossaPanel.Text = "Right Fossa";
            rightFossaPanel.NavigationState = "Right TMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            statePicker.addStatePanel(rightFossaPanel);

            statePicker.addStatePanel(new BottomTeethRemovalPanel());
            statePicker.addStatePanel(new TopTeethRemovalPanel());
            statePicker.addStatePanel(new TeethAdaptationPanel());
            statePicker.setToDefault();
        }

        public void Dispose()
        {
            if (statePicker != null)
            {
                statePicker.Dispose();
            }
        }

        public void updateStatePicker(String rootDirectory)
        {
            PresetStateSet leftGrowth = new PresetStateSet("Left Condyle Growth", rootDirectory + "/LeftGrowth");
            loadPresetSet(leftGrowth);
            leftGrowthPanel.clear();
            leftGrowthPanel.initialize(leftGrowth);

            PresetStateSet rightGrowth = new PresetStateSet("Right Condyle Growth", rootDirectory + "/RightGrowth");
            loadPresetSet(rightGrowth);
            rightGrowthPanel.clear();
            rightGrowthPanel.initialize(rightGrowth);

            PresetStateSet leftDegeneration = new PresetStateSet("Left Condyle Degeneration", rootDirectory + "/LeftDegeneration");
            loadPresetSet(leftDegeneration);
            leftDegenerationPanel.clear();
            leftDegenerationPanel.initialize(leftDegeneration);

            PresetStateSet rightDegeneration = new PresetStateSet("Right Condyle Degeneration", rootDirectory + "/RightDegeneration");
            loadPresetSet(rightDegeneration);
            rightDegenerationPanel.clear();
            rightDegenerationPanel.initialize(rightDegeneration);

            PresetStateSet leftDisc = new PresetStateSet("Left Disc", rootDirectory + "/LeftDisc");
            loadPresetSet(leftDisc);
            leftDiscPanel.clear();
            leftDiscPanel.initialize(leftDisc);

            PresetStateSet rightDisc = new PresetStateSet("Right Disc", rootDirectory + "/RightDisc");
            loadPresetSet(rightDisc);
            rightDiscPanel.clear();
            rightDiscPanel.initialize(rightDisc);

            PresetStateSet leftFossa = new PresetStateSet("Left Fossa", rootDirectory + "/LeftFossa");
            loadPresetSet(leftFossa);
            leftFossaPanel.clear();
            leftFossaPanel.initialize(leftFossa);

            PresetStateSet rightFossa = new PresetStateSet("Right Fossa", rootDirectory + "/RightFossa");
            loadPresetSet(rightFossa);
            rightFossaPanel.clear();
            rightFossaPanel.initialize(rightFossa);
        }

        public void setToDefault()
        {
            statePicker.setToDefault();
        }

        public void startWizard(DrawingWindow window)
        {
            statePicker.startWizard(window);
            statePicker.show();
        }

        public void close()
        {
            statePicker.close();
        }

        public bool Visible
        {
            get
            {
                return statePicker.Visible;
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
            if (StateCreated != null)
            {
                StateCreated.Invoke(state);
            }
        }

        void statePicker_Finished()
        {
            if (Finished != null)
            {
                Finished.Invoke();
            }
        }
    }
}
