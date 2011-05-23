using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller;

namespace Medical
{
    public enum WizardPanels
    {
        LeftCondylarDegeneration,
        RightCondylarDegeneration,
        LeftCondylarGrowth,
        RightCondylarGrowth,
        LeftFossa,
        RightFossa,
        LeftDopplerPanel,
        RightDopplerPanel,
        ProfileDistortionPanel,
        BottomTeethRemovalPanel,
        TopTeethRemovalPanel,
        TeethHeightAdaptationPanel,
        NotesPanel,
        LeftDiscSpacePanel,
        RightDiscSpacePanel,
        LeftDiscClockFacePanel,
        RightDiscClockFacePanel,
        TeethAdaptationPanel,
        DisclaimerPanel,
    }

    class PiperJBOWizards
    {
        private StateWizardPanelController stateWizardPanelController;
        private StateWizardController stateWizardController;
        private LicenseManager licenseManager;

        public PiperJBOWizards(StateWizardPanelController stateWizardPanelController, StateWizardController stateWizardController, LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
            this.stateWizardPanelController = stateWizardPanelController;
            this.stateWizardController = stateWizardController;

            createWizardPanels();
            createWizards();
        }

        private void createWizardPanels()
        {
            stateWizardPanelController.addCreationFunction(WizardPanels.NotesPanel, createNotesPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.LeftCondylarGrowth, createLeftCondylarGrowth);
            stateWizardPanelController.addCreationFunction(WizardPanels.LeftCondylarDegeneration, createLeftCondylarDegeneration);
            stateWizardPanelController.addCreationFunction(WizardPanels.RightCondylarGrowth, createRightCondylarGrowth);
            stateWizardPanelController.addCreationFunction(WizardPanels.RightCondylarDegeneration, createRightCondylarDegeneration);
            stateWizardPanelController.addCreationFunction(WizardPanels.LeftFossa, createLeftFossaPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.RightFossa, createRightFossaPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.LeftDopplerPanel, createLeftDopplerPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.RightDopplerPanel, createRightDopplerPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.ProfileDistortionPanel, createProfileDistortionPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.BottomTeethRemovalPanel, createBottomTeethRemovalPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.TopTeethRemovalPanel, createTopTeethRemovalPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.TeethHeightAdaptationPanel, createTeethHeightAdaptationPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.LeftDiscSpacePanel, createLeftDiscSpacePanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.RightDiscSpacePanel, createRightDiscSpacePanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.LeftDiscClockFacePanel, createLeftDiscClockFacePanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.RightDiscClockFacePanel, createRightDiscClockFacePanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.TeethAdaptationPanel, createTeethAdaptationPanel);
            stateWizardPanelController.addCreationFunction(WizardPanels.DisclaimerPanel, createDisclaimerPanel);
        }

        #region Creation Functions

        private StateWizardPanel createNotesPanel()
        {
            NotesPanel notesPanel = new NotesPanel(stateWizardPanelController);
            notesPanel.ImageKey = "DistortionPanelIcons/Notes";
            return notesPanel;
        }

        private StateWizardPanel createLeftCondylarGrowth()
        {
            LeftCondylarGrowthPanel leftCondyle = new LeftCondylarGrowthPanel(stateWizardPanelController);
            leftCondyle.LayerState = "MandibleSliderSizeLayers";
            leftCondyle.NavigationState = "WizardGrowthLeftCameraAngle";
            leftCondyle.TextLine1 = "Left Condyle";
            leftCondyle.TextLine2 = "Growth";
            leftCondyle.ImageKey = "DistortionPanelIcons/LeftCondyleGrowth";
            return leftCondyle;
        }

        private StateWizardPanel createLeftCondylarDegeneration()
        {
            LeftCondylarDegenerationPanel leftCondyle = new LeftCondylarDegenerationPanel(stateWizardPanelController);
            leftCondyle.LayerState = "MandibleSliderSizeLayers";
            leftCondyle.NavigationState = "WizardDegenerationLeftCameraAngle";
            leftCondyle.TextLine1 = "Left Condyle";
            leftCondyle.TextLine2 = "Degeneration";
            leftCondyle.ImageKey = "DistortionPanelIcons/LeftCondyleDegeneration";
            return leftCondyle;
        }

        private StateWizardPanel createRightCondylarGrowth()
        {
            RightCondylarGrowthPanel rightCondyle = new RightCondylarGrowthPanel(stateWizardPanelController);
            rightCondyle.LayerState = "MandibleSliderSizeLayers";
            rightCondyle.NavigationState = "WizardGrowthRightCameraAngle";
            rightCondyle.TextLine1 = "Right Condyle";
            rightCondyle.TextLine2 = "Growth";
            rightCondyle.ImageKey = "DistortionPanelIcons/RightCondyleGrowth";
            return rightCondyle;
        }

        private StateWizardPanel createRightCondylarDegeneration()
        {
            RightCondylarDegenerationPanel rightCondyle = new RightCondylarDegenerationPanel(stateWizardPanelController);
            rightCondyle.LayerState = "MandibleSliderSizeLayers";
            rightCondyle.NavigationState = "WizardDegenerationRightCameraAngle";
            rightCondyle.TextLine1 = "Right Condyle";
            rightCondyle.TextLine2 = "Degeneration";
            rightCondyle.ImageKey = "DistortionPanelIcons/RightCondyleDegeneration";
            return rightCondyle;
        }

        private StateWizardPanel createLeftFossaPanel()
        {
            FossaPanel leftFossaPanel = new FossaPanel("LeftFossa", "Medical.GUI.StateWizard.Panels.Fossa.FossaPanelLeft.layout", stateWizardPanelController);
            leftFossaPanel.NavigationState = "WizardLeftTMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            leftFossaPanel.TextLine1 = "Left Fossa";
            leftFossaPanel.ImageKey = "DistortionPanelIcons/LeftFossa";
            return leftFossaPanel;
        }

        private StateWizardPanel createRightFossaPanel()
        {
            FossaPanel rightFossaPanel = new FossaPanel("RightFossa", "Medical.GUI.StateWizard.Panels.Fossa.FossaPanelRight.layout", stateWizardPanelController);
            rightFossaPanel.NavigationState = "WizardRightTMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            rightFossaPanel.TextLine1 = "Right Fossa";
            rightFossaPanel.ImageKey = "DistortionPanelIcons/RightFossa";
            return rightFossaPanel;
        }

        private StateWizardPanel createLeftDopplerPanel()
        {
            DopplerPanel leftDopplerPanel = new DopplerPanel(stateWizardPanelController, "LeftDoppler", "WizardLeftTMJ", "WizardLeftTMJSuperior");
            leftDopplerPanel.NavigationState = "WizardBothTMJSuperior";
            leftDopplerPanel.LayerState = "JointMenuLayers";
            leftDopplerPanel.TextLine1 = "Left TMJ";
            leftDopplerPanel.TextLine2 = "Doppler";
            leftDopplerPanel.ImageKey = "DistortionPanelIcons/LeftDiscSpace";
            return leftDopplerPanel;
        }

        private StateWizardPanel createRightDopplerPanel()
        {
            DopplerPanel rightDopplerPanel = new DopplerPanel(stateWizardPanelController, "RightDoppler", "WizardRightTMJ", "WizardRightTMJSuperior");
            rightDopplerPanel.NavigationState = "WizardBothTMJSuperior";
            rightDopplerPanel.LayerState = "JointMenuLayers";
            rightDopplerPanel.TextLine1 = "Right TMJ";
            rightDopplerPanel.TextLine2 = "Doppler";
            rightDopplerPanel.ImageKey = "DistortionPanelIcons/RightDiscSpace";
            return rightDopplerPanel;
        }

        private StateWizardPanel createProfileDistortionPanel()
        {
            ProfileDistortionPanel profileDistortionPicker = new ProfileDistortionPanel(stateWizardPanelController);
            profileDistortionPicker.NavigationState = "WizardRightLateral";
            profileDistortionPicker.LayerState = "ProfileLayers";
            profileDistortionPicker.TextLine1 = "Profile";
            profileDistortionPicker.ImageKey = "DistortionPanelIcons/Profile";
            return profileDistortionPicker;
        }

        private StateWizardPanel createBottomTeethRemovalPanel()
        {
            ToothRemovalPanel panel = new ToothRemovalPanel("Medical.GUI.StateWizard.Panels.TeethPanels.ToothRemovalPanelBottom.layout", stateWizardPanelController);
            panel.LayerState = "BottomTeethLayers";
            panel.NavigationState = "WizardBottomTeeth";
            panel.TextLine1 = "Remove";
            panel.TextLine2 = "Mandibular Teeth";
            panel.ImageKey = "DistortionPanelIcons/BottomTeeth";
            return panel;
        }

        private StateWizardPanel createTopTeethRemovalPanel()
        {
            ToothRemovalPanel panel = new ToothRemovalPanel("Medical.GUI.StateWizard.Panels.TeethPanels.ToothRemovalPanelTop.layout", stateWizardPanelController);
            panel.LayerState = "TopTeethLayers";
            panel.NavigationState = "WizardTopTeeth";
            panel.TextLine1 = "Remove";
            panel.TextLine2 = "Maxillary Teeth";
            panel.ImageKey = "DistortionPanelIcons/TopTeeth";
            return panel;
        }

        private StateWizardPanel createTeethHeightAdaptationPanel()
        {
            TeethHeightAdaptationPanel teethHeightAdaptation = new TeethHeightAdaptationPanel(stateWizardPanelController);
            teethHeightAdaptation.NavigationState = "WizardTeethMidlineAnterior";
            teethHeightAdaptation.LayerState = "TeethLayers";
            teethHeightAdaptation.TextLine1 = "Teeth";
            teethHeightAdaptation.ImageKey = "DistortionPanelIcons/TeethAdaptation";
            return teethHeightAdaptation;
        }

        private StateWizardPanel createLeftDiscSpacePanel()
        {
            DiscSpacePanel leftDiscPanel = new DiscSpacePanel("FossaLayers", "LeftDiscSpace", stateWizardPanelController);
            leftDiscPanel.NavigationState = "WizardLeftTMJ";
            leftDiscPanel.LayerState = "FossaLayers";
            leftDiscPanel.TextLine1 = "Left Disc";
            leftDiscPanel.TextLine2 = "Space";
            leftDiscPanel.ImageKey = "DistortionPanelIcons/LeftDiscSpace";
            return leftDiscPanel;
        }

        private StateWizardPanel createRightDiscSpacePanel()
        {
            DiscSpacePanel rightDiscPanel = new DiscSpacePanel("FossaLayers", "RightDiscSpace", stateWizardPanelController);
            rightDiscPanel.NavigationState = "WizardRightTMJ";
            rightDiscPanel.LayerState = "FossaLayers";
            rightDiscPanel.TextLine1 = "Right Disc";
            rightDiscPanel.TextLine2 = "Space";
            rightDiscPanel.ImageKey = "DistortionPanelIcons/RightDiscSpace";
            return rightDiscPanel;
        }

        private StateWizardPanel createLeftDiscClockFacePanel()
        {
            PresetStatePanel leftDiscPanel = new PresetStatePanel("LeftDisc", stateWizardPanelController);
            leftDiscPanel.NavigationState = "WizardLeftTMJ";
            leftDiscPanel.LayerState = "DiscLayers";
            leftDiscPanel.TextLine1 = "Left TMJ";
            leftDiscPanel.ImageKey = "DistortionPanelIcons/LeftDiscPosition";
            return leftDiscPanel;
        }

        private StateWizardPanel createRightDiscClockFacePanel()
        {
            PresetStatePanel rightDiscPanel = new PresetStatePanel("RightDisc", stateWizardPanelController);
            rightDiscPanel.NavigationState = "WizardRightTMJ";
            rightDiscPanel.LayerState = "DiscLayers";
            rightDiscPanel.TextLine1 = "Right TMJ";
            rightDiscPanel.ImageKey = "DistortionPanelIcons/RightDiscPosition";
            return rightDiscPanel;
        }

        private StateWizardPanel createDisclaimerPanel()
        {
            StateWizardPanel panel = new StateWizardPanel("Medical.GUI.StateWizard.Panels.Disclaimer.DisclaimerPanel.layout", stateWizardPanelController);
            panel.TextLine1 = "Disclaimer";
            panel.ImageKey = "DistortionPanelIcons/Disclaimer";
            return panel;
        }

        private StateWizardPanel createTeethAdaptationPanel()
        {
            TeethAdaptationPanel teethPanel = new TeethAdaptationPanel(stateWizardPanelController);
            teethPanel.LayerState = "TeethLayers";
            teethPanel.NavigationState = "WizardTeethMidlineAnterior";
            teethPanel.TextLine1 = "Teeth";
            teethPanel.TextLine2 = "Adaptation";
            teethPanel.ImageKey = "DistortionPanelIcons/TeethAdaptation";
            return teethPanel;
        }

        #endregion

        private void createWizards()
        {
            //Create single distortion wizards
            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOClinical))
            {
                //Doppler
                StateWizard dopplerWizard = new StateWizard("Doppler", stateWizardController, WizardType.Exam);
                dopplerWizard.TextLine1 = "Doppler";
                dopplerWizard.ImageKey = "DistortionsToolstrip/Doppler";
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDopplerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(dopplerWizard);
            }

            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOClinical))
            {
                //Teeth
                StateWizard teethWizard = new StateWizard("Dentition", stateWizardController, WizardType.Anatomy);
                teethWizard.TextLine1 = "Dentition";
                teethWizard.ImageKey = "DistortionsToolstrip/Dentition";
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(teethWizard);
            }

            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOClinical))
            {
                //Profile
                StateWizard profileWizard = new StateWizard("Cephalometric", stateWizardController, WizardType.Anatomy);
                profileWizard.TextLine1 = "Cephalometric";
                profileWizard.ImageKey = "DistortionsToolstrip/Cephalometric";
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(profileWizard);
            }



            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOImaging))
            {
                //Bone
                StateWizard boneWizard = new StateWizard("Mandible", stateWizardController, WizardType.Anatomy);
                boneWizard.TextLine1 = "Mandible";
                boneWizard.ImageKey = "DistortionsToolstrip/Mandible";
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(boneWizard);
            }

            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOImaging))
            {
                //Disc
                StateWizard discWizard = new StateWizard("Disc Space", stateWizardController, WizardType.Exam);
                discWizard.TextLine1 = "Disc Space";
                discWizard.ImageKey = "DistortionsToolstrip/DiscSpace";
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(discWizard);
            }

            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOImaging))
            {
                //Disc
                StateWizard discClockWizard = new StateWizard("Disc Clock Face", stateWizardController, WizardType.Anatomy);
                discClockWizard.TextLine1 = "Disc";
                discClockWizard.TextLine2 = "Clock Face";
                discClockWizard.ImageKey = "DistortionsToolstrip/DiscClockFace";
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(discClockWizard);
            }

            //Create combination distortion wizards

            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOClinical))
            {
                //Profile + Teeth
                StateWizard profileTeethWizard = new StateWizard("Cephalometric and Dentition", stateWizardController, WizardType.Exam);
                profileTeethWizard.TextLine1 = "Cephalometric";
                profileTeethWizard.TextLine2 = "and Dentition";
                profileTeethWizard.ImageKey = "DistortionsToolstrip/CephalometricAndDentition";
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(profileTeethWizard);
            }

            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOClinical))
            {
                //Clinical
                StateWizard clinicalWizard = new StateWizard("Clinical and Doppler", stateWizardController, WizardType.Exam);
                clinicalWizard.TextLine1 = "Clinical";
                clinicalWizard.TextLine2 = "and Doppler";
                clinicalWizard.ImageKey = "DistortionsToolstrip/ClinicalAndDoppler";
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDopplerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(clinicalWizard);
            }

            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOImaging))
            {
                //CT/Radiography Wizard
                StateWizard ctWizard = new StateWizard("Clinical and Radiography", stateWizardController, WizardType.Exam);
                ctWizard.TextLine1 = "Radiography";
                ctWizard.ImageKey = "DistortionsToolstrip/ClinicalAndRadiography";
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftFossa));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightFossa));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(ctWizard);
            }

            if (licenseManager.allowFeature((int)FeatureCodes.PiperJBOImaging))
            {
                //MRI Wizard
                StateWizard mriWizard = new StateWizard("Clinical and MRI", stateWizardController, WizardType.Exam);
                mriWizard.TextLine1 = "MRI";
                mriWizard.ImageKey = "DistortionsToolstrip/ClinicalAndMRI";
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftFossa));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightFossa));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(mriWizard);
            }
        }
    }
}
