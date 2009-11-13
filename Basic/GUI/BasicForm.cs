using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Controller;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using Medical.Properties;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;
using Engine.Platform;
using ComponentFactory.Krypton.Navigator;

namespace Medical.GUI
{    
    public partial class BasicForm : KryptonForm, OSWindow
    {
        private BasicController controller;
        private OpenPatientDialog openPatient = new OpenPatientDialog();
        private SavePatientDialog savePatient = new SavePatientDialog();
        private AboutBox aboutBox = new AboutBox(Resources.articulometricsclinic);
        private ShortcutController shortcutController;
        private ToolStrip toolStrip1 = new ToolStrip();
        private ToolStripContainer toolStripContainer = new ToolStripContainer();
        private LayerGUIController layerGUIController;
        private WindowGUIController windowGUIController;
        private NavigationGUIController navigationGUIController;
        private RenderGUIController renderGUIController;
        private MandibleGUIController mandibleGUIController;
        private SequencesGUIController sequencesGUIController;

        public BasicForm(ShortcutController shortcuts)
        {
            InitializeComponent();
            this.shortcutController = shortcuts;

            //navigationButton.ImageIndex = 5;

            ShortcutGroup shortcutGroup = shortcuts.createOrRetrieveGroup("MainUI");
            
            ShortcutEventCommand newShortcut = new ShortcutEventCommand("New", Keys.N, true);
            newShortcut.Execute += newShortcut_Execute;
            shortcutGroup.addShortcut(newShortcut);

            ShortcutEventCommand openShortcut = new ShortcutEventCommand("Open", Keys.O, true);
            openShortcut.Execute += openShortcut_Execute;
            shortcutGroup.addShortcut(openShortcut);

            ShortcutEventCommand saveShortcut = new ShortcutEventCommand("Save", Keys.S, true);
            saveShortcut.Execute += saveShortcut_Execute;
            shortcutGroup.addShortcut(saveShortcut);

            StatusController.StatusChanged += new StatusUpdater(StatusController_StatusChanged);

            //App menu
            changeSceneMenuItem.Click += new EventHandler(changeSceneMenuItem_Click);
            openMenuItem.Click += new EventHandler(openMenuItem_Click);
            saveMenuItem.Click += new EventHandler(saveMenuItem_Click);
            exitMenuItem.Click += new EventHandler(exitMenuItem_Click);

            showNavigationQATButton.Click += new EventHandler(showNavigationQATButton_Click);
            showTeethCollisionQATButton.Click += new EventHandler(showTeethCollisionQATButton_Click);

            //Hide navigators
            //primaryNavigator.Visible = false;
        }

        void showTeethCollisionQATButton_Click(object sender, EventArgs e)
        {
            showTeethCollisionCommand.Checked = !showTeethCollisionCommand.Checked;
        }

        void showNavigationQATButton_Click(object sender, EventArgs e)
        {
            showNavigationCommand.Checked = !showNavigationCommand.Checked;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            layerGUIController.Dispose();
            openPatient.Dispose();
            savePatient.Dispose();
            base.Dispose(disposing);
        }

        public void initialize(BasicController controller, ImageList imageList)
        {
            this.controller = controller;
            toolStrip1.ImageList = imageList;
            windowGUIController = new WindowGUIController(this, controller, shortcutController);
            layerGUIController = new LayerGUIController(this, controller, shortcutController);
            navigationGUIController = new NavigationGUIController(this, controller, shortcutController);
            renderGUIController = new RenderGUIController(this, controller, shortcutController);
            mandibleGUIController = new MandibleGUIController(this, controller);
            sequencesGUIController = new SequencesGUIController(this, controller);
        }

        public void setViewMode()
        {
            //windowToolStripMenuItem.Visible = true;
            //toolsToolStripMenuItem.Visible = true;
            //distortionToolStripMenuItem.Visible = true;
            //newToolStripMenuItem.Visible = true;
            //openToolStripMenuItem.Visible = true;
            //saveToolStripMenuItem.Visible = true;
        }

        public void setDistortionMode()
        {
            //windowToolStripMenuItem.Visible = false;
            //toolsToolStripMenuItem.Visible = false;
            //distortionToolStripMenuItem.Visible = false;
            //newToolStripMenuItem.Visible = false;
            //openToolStripMenuItem.Visible = false;
            //saveToolStripMenuItem.Visible = false;
        }

        public void createDistortionMenu(IEnumerable<DistortionWizard> wizards)
        {
            foreach (DistortionWizard wizard in wizards)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(wizard.Name);
                item.Click += distortionWizardItem_Click;
//                distortionToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        void distortionWizardItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                controller.showStatePicker(item.Text);
            }
        }

        public ToolStripContainer ToolStrip
        {
            get
            {
                return toolStripContainer;
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

        #region App Menu

        void saveMenuItem_Click(object sender, EventArgs e)
        {
            //InputResult result = InputBox.GetInput("Save", "Enter a file name", this, validateFileName);
            //if (result.ok)
            //{
            //    controller.saveMedicalState(MedicalConfig.SaveDirectory + "/" + result.text + ".sim.xml");
            //}
            savePatient.ShowDialog(this);
            if (savePatient.SaveFile)
            {
                controller.saveMedicalState(savePatient.Filename);
                StatusController.SetStatus(String.Format("File saved to {0}", savePatient.Filename));
            }
        }

        void openMenuItem_Click(object sender, EventArgs e)
        {
            //fileTracker.openFile(this);
            //if (fileTracker.lastDialogAccepted())
            //{
            //    controller.openStates(fileTracker.getCurrentFile());
            //}
            openPatient.listFiles(MedicalConfig.SaveDirectory);
            openPatient.ShowDialog(this);
            if (openPatient.FileChosen)
            {
                controller.openStates(openPatient.CurrentFile);
            }
        }

        private bool validateFileName(String input, out String errorPrompt)
        {
            if (input == null || input == "")
            {
                errorPrompt = "Please enter a non empty name.";
                return false;
            }
            if (File.Exists(MedicalConfig.SaveDirectory + "/" + input + ".sim.xml"))
            {
                DialogResult msgRes = MessageBox.Show(this, "A file named " + input + " already exists. Would you like to overwrite it?", "Overwright?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msgRes == DialogResult.No)
                {
                    errorPrompt = "Enter a different file name.";
                    return false;
                }
            }
            errorPrompt = "";
            return true;
        }

        void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void changeSceneMenuItem_Click(object sender, EventArgs e)
        {
            controller.newScene();
        }

        void saveShortcut_Execute(ShortcutEventCommand shortcut)
        {
            saveMenuItem_Click(null, null);
        }

        void openShortcut_Execute(ShortcutEventCommand shortcut)
        {
            openMenuItem_Click(null, null);
        }

        void newShortcut_Execute(ShortcutEventCommand shortcut)
        {
            changeSceneMenuItem_Click(null, null);
        }

        #endregion App Menu

        #region Window Management

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.showOptions();
        }

        #endregion Window Management

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutBox.ShowDialog(this);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            shortcutController.resetButtons();
        }

        void StatusController_StatusChanged(string status)
        {
            mainStatusLabel.Text = status;
            Application.DoEvents();
        }

        #region OSWindow Members

        private List<OSWindowListener> listeners = new List<OSWindowListener>();

        public IntPtr WindowHandle
        {
            get
            {
                return this.Handle;
            }
        }

        public int WindowWidth
        {
            get
            {
                return this.Width;
            }
        }

        public int WindowHeight
        {
            get
            {
                return this.Height;
            }
        }

        public void addListener(OSWindowListener listener)
        {
            listeners.Add(listener);
        }

        public void removeListener(OSWindowListener listener)
        {
            listeners.Remove(listener);
        }

        protected override void OnResize(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.resized(this);
            }
            base.OnResize(e);
        }

        protected override void OnMove(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.moved(this);
            }
            base.OnMove(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.closing(this);
            }
            base.OnHandleDestroyed(e);
        }

        #endregion

        private void buttonSpecExpandCollapse_Click(object sender, EventArgs e)
        {
            // Are we currently showing fully expanded?
            if (primaryNavigator.NavigatorMode == NavigatorMode.OutlookFull)
            {
                // Switch to mini mode and reverse direction of arrow
                primaryNavigator.NavigatorMode = NavigatorMode.OutlookMini;
                buttonSpecExpandCollapse.TypeRestricted = PaletteNavButtonSpecStyle.ArrowRight;
            }
            else
            {
                // Switch to full mode and reverse direction of arrow
                primaryNavigator.NavigatorMode = NavigatorMode.OutlookFull;
                buttonSpecExpandCollapse.TypeRestricted = PaletteNavButtonSpecStyle.ArrowLeft;
            }
        }
    }
}
