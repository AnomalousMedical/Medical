﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Controller;
using WeifenLuo.WinFormsUI.Docking;
using ComponentFactory.Krypton.Ribbon;

namespace Medical.GUI
{
    public partial class AdvancedForm : MedicalForm
    {
        private AdvancedController controller;
        private FileTracker patientFileTracker = new FileTracker("*.pat|*.pat");
        private FileTracker sequenceFileTracker = new FileTracker("*.seq|*.seq");

        public AdvancedForm()
        {
            InitializeComponent();
            this.initialize(Text);

            //Commands
            changeSceneCommand.Execute += new EventHandler(changeSceneCommand_Execute);
            exitCommand.Execute += new EventHandler(exitCommand_Execute);

            addCurrentStateCommand.Execute += new EventHandler(addCurrentStateCommand_Execute);
            newDistortionCommand.Execute += new EventHandler(newDistortionCommand_Execute);
            openDistortionCommand.Execute += new EventHandler(openDistortionCommand_Execute);
            saveDistortionCommand.Execute += new EventHandler(saveDistortionCommand_Execute);
            saveDistortionAsCommand.Execute += new EventHandler(saveDistortionAsCommand_Execute);

            newSequenceCommand.Execute += new EventHandler(newSequenceCommand_Execute);
            openSequenceCommand.Execute += new EventHandler(openSequenceCommand_Execute);
            saveSequenceCommand.Execute += new EventHandler(saveSequenceCommand_Execute);
            saveSequenceAsCommand.Execute += new EventHandler(saveSequenceAsCommand_Execute);

            optionsCommand.Execute += new EventHandler(optionsCommand_Execute);
            oneWindowLayoutCommand.Execute += new EventHandler(oneWindowLayoutCommand_Execute);
            twoWindowLayoutCommand.Execute += new EventHandler(twoWindowLayoutCommand_Execute);
            threeWindowLayoutCommand.Execute += new EventHandler(threeWindowLayoutCommand_Execute);
            fourWindowLayoutCommand.Execute += new EventHandler(fourWindowLayoutCommand_Execute);
            cloneWindowCommand.Execute += new EventHandler(cloneWindowCommand_Execute);
        }

        public void initialize(AdvancedController controller)
        {
            this.controller = controller;
        }

        public KryptonRibbonTab HomeTab
        {
            get
            {
                return homeTab;
            }
        }

        public DockPanel DockPanel
        {
            get
            {
                return dockPanel;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            controller.stop();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.newScene();
            clearWindowTitle();
            patientFileTracker.clearCurrentFile();
        }

        private void oneWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void twoWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void threeWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void fourWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void newDistortion_Click(object sender, EventArgs e)
        {
            
        }

        private void openDistortion_Click(object sender, EventArgs e)
        {
            
        }

        private void saveDistortion_Click(object sender, EventArgs e)
        {
            
        }

        private void saveDistortionAs_Click(object sender, EventArgs e)
        {
            
        }

        private void newSequence_Click(object sender, EventArgs e)
        {
            
        }

        private void openSequence_Click(object sender, EventArgs e)
        {
            
        }

        private void saveSequence_Click(object sender, EventArgs e)
        {
            
        }

        private void saveSequenceAs_Click(object sender, EventArgs e)
        {
            
        }

        private void cloneSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        //File menu
        void exitCommand_Execute(object sender, EventArgs e)
        {
            this.Close();
        }

        void changeSceneCommand_Execute(object sender, EventArgs e)
        {
            controller.newScene();
            clearWindowTitle();
            patientFileTracker.clearCurrentFile();
        }

        //Distortion tab
        void saveDistortionAsCommand_Execute(object sender, EventArgs e)
        {
            patientFileTracker.saveFileAs(this);
            if (patientFileTracker.lastDialogAccepted())
            {
                controller.saveMedicalState(patientFileTracker.getCurrentFile());
                updateWindowTitle(patientFileTracker.getCurrentFile());
            }
        }

        void saveDistortionCommand_Execute(object sender, EventArgs e)
        {
            patientFileTracker.saveFile(this);
            if (patientFileTracker.lastDialogAccepted())
            {
                controller.saveMedicalState(patientFileTracker.getCurrentFile());
                updateWindowTitle(patientFileTracker.getCurrentFile());
            }
        }

        void openDistortionCommand_Execute(object sender, EventArgs e)
        {
            patientFileTracker.openFile(this);
            if (patientFileTracker.lastDialogAccepted())
            {
                controller.openStates(patientFileTracker.getCurrentFile());
                updateWindowTitle(patientFileTracker.getCurrentFile());
            }
            //fileTracker.openFile(this);
            //if (fileTracker.lastDialogAccepted())
            //{
            //    controller.open(fileTracker.getCurrentFile());
            //}
        }

        void newDistortionCommand_Execute(object sender, EventArgs e)
        {
            controller.createNewMedicalStates();
            clearWindowTitle();
            patientFileTracker.clearCurrentFile();
        }

        void addCurrentStateCommand_Execute(object sender, EventArgs e)
        {
            controller.createMedicalState("Test");
        }

        //Sequence tab
        void saveSequenceAsCommand_Execute(object sender, EventArgs e)
        {
            sequenceFileTracker.saveFileAs(this);
            if (sequenceFileTracker.lastDialogAccepted())
            {
                controller.saveSequence(sequenceFileTracker.getCurrentFile());
            }
        }

        void saveSequenceCommand_Execute(object sender, EventArgs e)
        {
            sequenceFileTracker.saveFile(this);
            if (sequenceFileTracker.lastDialogAccepted())
            {
                controller.saveSequence(sequenceFileTracker.getCurrentFile());
            }
        }

        void openSequenceCommand_Execute(object sender, EventArgs e)
        {
            sequenceFileTracker.openFile(this);
            if (sequenceFileTracker.lastDialogAccepted())
            {
                controller.loadSequence(sequenceFileTracker.getCurrentFile());
            }
        }

        void newSequenceCommand_Execute(object sender, EventArgs e)
        {
            controller.createNewSequence();
        }

        //Window tab
        void cloneWindowCommand_Execute(object sender, EventArgs e)
        {
            controller.cloneActiveWindow();
        }

        void fourWindowLayoutCommand_Execute(object sender, EventArgs e)
        {
            controller.setFourWindowLayout();
        }

        void threeWindowLayoutCommand_Execute(object sender, EventArgs e)
        {
            controller.setThreeWindowLayout();
        }

        void twoWindowLayoutCommand_Execute(object sender, EventArgs e)
        {
            controller.setTwoWindowLayout();
        }

        void oneWindowLayoutCommand_Execute(object sender, EventArgs e)
        {
            controller.setOneWindowLayout();
        }

        void optionsCommand_Execute(object sender, EventArgs e)
        {
            controller.showOptions();
        }
    }
}
