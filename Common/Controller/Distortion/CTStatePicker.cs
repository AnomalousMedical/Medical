using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using System.Drawing;
using Medical.Properties;
using Medical.GUI;
using Engine.Saving.XMLSaver;

namespace Medical
{
    public class CTStatePicker : DistortionWizard
    {
        private TemporaryStateBlender temporaryStateBlender;
        private StatePickerWizard statePicker;

        private FossaPanel leftFossaPanel;
        private FossaPanel rightFossaPanel;
        private DiscSpacePanel leftDiscPanel;
        private DiscSpacePanel rightDiscPanel;

        private XmlSaver saver = new XmlSaver();

        private String lastRootDirectory;

        public CTStatePicker(StatePickerPanelController panelController)
        {
            temporaryStateBlender = new TemporaryStateBlender(panelController.MedicalController.MainTimer, panelController.StateController);
            statePicker = new StatePickerWizard(Name, panelController.UiHost, temporaryStateBlender, panelController.NavigationController, panelController.LayerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftCondylarGrowth));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftCondylarDegeneration));

            leftFossaPanel = new FossaPanel("LeftFossa");
            leftFossaPanel.NormalImage = Resources.LeftFossaNormal;
            leftFossaPanel.DistortedImage = Resources.LeftFossaFlat;
            leftFossaPanel.NavigationState = "Left TMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            leftFossaPanel.Text = "Left Fossa";
            leftFossaPanel.TextLine1 = "Left Fossa";
            leftFossaPanel.LargeIcon = Resources.LeftFossaFlatness;
            statePicker.addStatePanel(leftFossaPanel);

            leftDiscPanel = new DiscSpacePanel("LeftTMJDisc", "FossaLayers");
            leftDiscPanel.BoneOnBoneImage = Resources.LeftDiscBoneOnBone;
            leftDiscPanel.OpenImage = Resources.LeftDiscOpen;
            leftDiscPanel.NavigationState = "Left TMJ";
            leftDiscPanel.LayerState = "FossaLayers";
            leftDiscPanel.Text = "Left Disc Space";
            leftDiscPanel.TextLine1 = "Left Disc";
            leftDiscPanel.TextLine2 = "Space";
            leftDiscPanel.LargeIcon = Resources.LeftDiscSpace;
            statePicker.addStatePanel(leftDiscPanel);

            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightCondylarGrowth));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightCondylarDegeneration));

            rightFossaPanel = new FossaPanel("RightFossa");
            rightFossaPanel.NormalImage = Resources.RightFossaNormal;
            rightFossaPanel.DistortedImage = Resources.RightFossaFlat;
            rightFossaPanel.NavigationState = "Right TMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            rightFossaPanel.Text = "Right Fossa";
            rightFossaPanel.TextLine1 = "Right Fossa";
            rightFossaPanel.LargeIcon = Resources.RightFossaFlatness;
            statePicker.addStatePanel(rightFossaPanel);

            rightDiscPanel = new DiscSpacePanel("RightTMJDisc", "FossaLayers");
            rightDiscPanel.BoneOnBoneImage = Resources.RightDiscBoneOnBone;
            rightDiscPanel.OpenImage = Resources.RightDiscOpen;
            rightDiscPanel.NavigationState = "Right TMJ";
            rightDiscPanel.LayerState = "FossaLayers";
            rightDiscPanel.Text = "Right Disc Space";
            rightDiscPanel.TextLine1 = "Right Disc";
            rightDiscPanel.TextLine2 = "Space";
            rightDiscPanel.LargeIcon = Resources.RightDiscSpace;
            statePicker.addStatePanel(rightDiscPanel);

            statePicker.addStatePanel(new BottomTeethRemovalPanel());
            statePicker.addStatePanel(new TopTeethRemovalPanel());
            statePicker.addStatePanel(new TeethAdaptationPanel());
            statePicker.addStatePanel(new NotesPanel("CT", panelController.ImageRenderer));

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
            leftDiscPanel.sceneLoaded(scene, rootDirectory + "/LeftDoppler");
            rightDiscPanel.sceneLoaded(scene, rootDirectory + "/RightDoppler");

            if (rootDirectory != lastRootDirectory)
            {
                lastRootDirectory = rootDirectory;
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

        public override string Name
        {
            get 
            {
                return "CT Wizard";
            }
        }

        public override string TextLine1
        {
            get
            {
                return "CT Wizard";
            }
        }

        public override string TextLine2
        {
            get 
            {
                return null;
            }
        }

        public override Image ImageLarge
        {
            get
            {
                return Resources.CTWizardLarge;
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
