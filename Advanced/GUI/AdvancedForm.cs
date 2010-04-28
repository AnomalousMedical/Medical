﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Controller;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Toolkit;
using Engine;

namespace Medical.GUI
{
    public partial class AdvancedForm : MedicalForm
    {
        private MedicalFormController controller;
        private FileTracker patientFileTracker = new FileTracker("*.pat|*.pat");
        private FileTracker sequenceFileTracker = new FileTracker("*.seq|*.seq");

        public AdvancedForm()
        {
            InitializeComponent();

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

            dockingManager.ManageFloating("Floating", this);
            dockingManager.ManageControl(autoHideDock);
            drawingWindowDockingManager.ManageWorkspace("Workspace", drawingWindowWorkspace);
            drawingWindowDockingManager.ManageFloating("Floating2", this);

            backgroundColorPicker.SelectedColorChanged += new EventHandler<ComponentFactory.Krypton.Toolkit.ColorEventArgs>(backgroundColorPicker_SelectedColorChanged);

            showStatsCommand.Execute += new EventHandler(showStatsCommand_Execute);

            normalRenderingMode.Execute += new EventHandler(normalRenderingMode_Execute);
            wireframeRenderingMode.Execute += new EventHandler(wireframeRenderingMode_Execute);
            pointRenderingMode.Execute += new EventHandler(pointRenderingMode_Execute);
        }

        public void initialize(MedicalFormController controller)
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

        public KryptonDockingManager DrawingWindowDockingManager
        {
            get
            {
                return drawingWindowDockingManager;
            }
        }

        public KryptonDockableWorkspace DrawingWindowWorkspace
        {
            get
            {
                return drawingWindowWorkspace;
            }
        }

        public KryptonDockingManager DockingManager
        {
            get
            {
                return dockingManager;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            controller.stop();
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

        void showStatsCommand_Execute(object sender, EventArgs e)
        {
            controller.DrawingWindowController.showStats(showStatsCommand.Checked);
        }

        void optionsCommand_Execute(object sender, EventArgs e)
        {
            controller.showOptions();
        }

        private void backgroundColorPicker_SelectedColorChanged(object sender, ColorEventArgs e)
        {
            DrawingWindowHost window = controller.DrawingWindowController.getActiveWindow();
            if (window != null)
            {
                window.DrawingWindow.BackColor = backgroundColorPicker.SelectedColor;
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] files = drgevent.Data.GetData(DataFormats.FileDrop) as String[];
                if (files.Length > 0)
                {
                    if (files[0].EndsWith(".dst"))
                    {
                        drgevent.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] files = drgevent.Data.GetData(DataFormats.FileDrop) as String[];
                if (files.Length > 0)
                {
                    if (files[0].EndsWith(".dst"))
                    {
                        controller.applyDistortion(files[0]);
                    }
                }
            }
        }

        void pointRenderingMode_Execute(object sender, EventArgs e)
        {
            controller.changeRenderingMode(RenderingMode.Points);
        }

        void wireframeRenderingMode_Execute(object sender, EventArgs e)
        {
            controller.changeRenderingMode(RenderingMode.Wireframe);
        }

        void normalRenderingMode_Execute(object sender, EventArgs e)
        {
            controller.changeRenderingMode(RenderingMode.Solid);
        }
    }
}
