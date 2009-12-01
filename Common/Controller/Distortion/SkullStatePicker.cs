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

namespace Medical
{
    public class SkullStatePicker : DistortionWizard
    {
        private TemporaryStateBlender temporaryStateBlender;
        private StatePickerWizard statePicker;

#if USE_SLIDER_GUIS
        private LeftCondylarGrowthPanel leftCondylarGrowthPanel;
        private RightCondylarGrowthPanel rightCondylarGrowthPanel;
        private LeftCondylarDegenrationPanel leftCondylarDegenerationPanel;
        private RightCondylarDegenerationPanel rightCondylarDegenerationPanel;
        private FossaPanel leftFossaPanel;
        private FossaPanel rightFossaPanel;
#else
        private PresetStatePanel leftGrowthPanel;
        private PresetStatePanel rightGrowthPanel;
        private PresetStatePanel leftDegenerationPanel;
        private PresetStatePanel rightDegenerationPanel;
        private PresetStatePanel leftFossaPanel;
        private PresetStatePanel rightFossaPanel;
#endif
        private PresetStatePanel leftDiscPanel;
        private PresetStatePanel rightDiscPanel;

        private XmlSaver saver = new XmlSaver();

        private String lastRootDirectory;

        public SkullStatePicker(StatePickerUIHost uiHost, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, ImageRenderer imageRenderer)
        {
            temporaryStateBlender = new TemporaryStateBlender(medicalController.MainTimer, stateController);
            statePicker = new StatePickerWizard(uiHost, temporaryStateBlender, navigationController, layerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

#if USE_SLIDER_GUIS
            leftCondylarGrowthPanel = new LeftCondylarGrowthPanel();
            statePicker.addStatePanel(leftCondylarGrowthPanel);

            leftCondylarDegenerationPanel = new LeftCondylarDegenrationPanel();
            statePicker.addStatePanel(leftCondylarDegenerationPanel);
#else
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
#endif

            leftDiscPanel = new PresetStatePanel();
            leftDiscPanel.Text = "Left Disc";
            leftDiscPanel.NavigationState = "Left TMJ";
            leftDiscPanel.LayerState = "DiscLayers";
            leftDiscPanel.TextLine1 = "Left TMJ";
            leftDiscPanel.LargeIcon = Resources.LeftDiscPosition;
            statePicker.addStatePanel(leftDiscPanel);

#if USE_SLIDER_GUIS
            leftFossaPanel = new FossaPanel("LeftFossa");
            leftFossaPanel.NormalImage = Resources.LeftFossaNormal;
            leftFossaPanel.DistortedImage = Resources.LeftFossaFlat;
            leftFossaPanel.NavigationState = "Left TMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            leftFossaPanel.Text = "Left Fossa";
            leftFossaPanel.TextLine1 = "Left Fossa";
            leftFossaPanel.LargeIcon = Resources.LeftFossaFlatness;
            statePicker.addStatePanel(leftFossaPanel);
#else
            leftFossaPanel = new PresetStatePanel();
            leftFossaPanel.Text = "Left Fossa";
            leftFossaPanel.NavigationState = "Left TMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            statePicker.addStatePanel(leftFossaPanel);
#endif

#if USE_SLIDER_GUIS
            rightCondylarGrowthPanel = new RightCondylarGrowthPanel();
            statePicker.addStatePanel(rightCondylarGrowthPanel);

            rightCondylarDegenerationPanel = new RightCondylarDegenerationPanel();
            statePicker.addStatePanel(rightCondylarDegenerationPanel);
#else
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
#endif

            rightDiscPanel = new PresetStatePanel();
            rightDiscPanel.Text = "Right Disc";
            rightDiscPanel.NavigationState = "Right TMJ";
            rightDiscPanel.LayerState = "DiscLayers";
            rightDiscPanel.TextLine1 = "Right TMJ";
            rightDiscPanel.LargeIcon = Resources.RightDiscPosition;
            statePicker.addStatePanel(rightDiscPanel);

#if USE_SLIDER_GUIS
            rightFossaPanel = new FossaPanel("RightFossa");
            rightFossaPanel.NormalImage = Resources.RightFossaNormal;
            rightFossaPanel.DistortedImage = Resources.RightFossaFlat;
            rightFossaPanel.NavigationState = "Right TMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            rightFossaPanel.Text = "Right Fossa";
            rightFossaPanel.TextLine1 = "Right Fossa";
            rightFossaPanel.LargeIcon = Resources.RightFossaFlatness;
            statePicker.addStatePanel(rightFossaPanel);
#else
            rightFossaPanel = new PresetStatePanel();
            rightFossaPanel.Text = "Right Fossa";
            rightFossaPanel.NavigationState = "Right TMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            statePicker.addStatePanel(rightFossaPanel);
#endif

            statePicker.addStatePanel(new BottomTeethRemovalPanel());
            statePicker.addStatePanel(new TopTeethRemovalPanel());
            statePicker.addStatePanel(new TeethAdaptationPanel());
            statePicker.addStatePanel(new NotesPanel("MRI", imageRenderer));

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
#if USE_SLIDER_GUIS
            leftCondylarGrowthPanel.sceneLoaded(scene);
            rightCondylarGrowthPanel.sceneLoaded(scene);
            leftCondylarDegenerationPanel.sceneLoaded(scene);
            rightCondylarDegenerationPanel.sceneLoaded(scene);
            leftFossaPanel.sceneLoaded(scene);
            rightFossaPanel.sceneLoaded(scene);
#endif
            if (rootDirectory != lastRootDirectory)
            {
                lastRootDirectory = rootDirectory;

#if !USE_SLIDER_GUIS
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

                PresetStateSet leftFossa = new PresetStateSet("Left Fossa", rootDirectory + "/LeftFossa");
                loadPresetSet(leftFossa);
                leftFossaPanel.clear();
                leftFossaPanel.initialize(leftFossa);

                PresetStateSet rightFossa = new PresetStateSet("Right Fossa", rootDirectory + "/RightFossa");
                loadPresetSet(rightFossa);
                rightFossaPanel.clear();
                rightFossaPanel.initialize(rightFossa);
#endif

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
    }
}
