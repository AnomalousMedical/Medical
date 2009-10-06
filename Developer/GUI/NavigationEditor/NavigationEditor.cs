using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using Engine;

namespace Medical.GUI
{
    public partial class NavigationStateSelector : GUIElement
    {
        private NavigationController navController;
        private DrawingWindowController drawingWindowController;
        private NavigationState currentState;
        private ListViewItem currentStateItem;
        private FileTracker cameraFileTracker = new FileTracker("*.cam|*.cam");
        private String titleFormat = "Nav Editor - {0}";

        private NavigationLink currentLink;
        private ListViewItem currentLinkItem;

        public NavigationStateSelector(NavigationController navController, DrawingWindowController drawingWindowController)
        {
            InitializeComponent();
            this.navController = navController;
            this.drawingWindowController = drawingWindowController;
            navController.NavigationStateSetChanged += new NavigationControllerEvent(navController_NavigationStateSetChanged);
            navigationStateView.SelectedIndexChanged += new EventHandler(navigationStateView_SelectedIndexChanged);
            navigationStateView.MouseClick += new MouseEventHandler(navigationStateView_MouseClick);
            createStateMenu.Opening += new CancelEventHandler(createStateMenu_Opening);
            linkMenu.Opening += new CancelEventHandler(linkMenu_Opening);
            linkView.SelectedIndexChanged += new EventHandler(linkView_SelectedIndexChanged);

            foreach (FieldInfo fieldInfo in typeof(NavigationButtons).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                buttonCombo.Items.Add(fieldInfo.Name);
            }
        }

        void navigationStateView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (navigationStateView.SelectedItems.Count == 0)
                {
                    createStateMenu.Show(navigationStateView.PointToScreen(e.Location));
                }
                else if (navigationStateView.SelectedItems.Count == 1)
                {
                    singleStateMenu.Show(navigationStateView.PointToScreen(e.Location));
                }
                else
                {
                    multipleStateMenu.Show(navigationStateView.PointToScreen(e.Location));
                }
            }
        }

        void navigationStateView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (navigationStateView.SelectedItems.Count == 1)
            {
                currentStateItem = navigationStateView.SelectedItems[0];
                currentState = currentStateItem.Tag as NavigationState;
                nameText.Text = currentState.Name;
                translationText.Text = currentState.Translation.ToString();
                lookAtText.Text = currentState.LookAt.ToString();
                hiddenCheck.Checked = currentState.Hidden;
                linkView.SelectedItems.Clear();
                foreach (NavigationLink link in currentState.AdjacentStates)
                {
                    ListViewItem item = linkView.Items.Add(link.Destination.Name, link.Destination.Name, 0);
                    item.Tag = link;
                }
                stateUpdate.Enabled = true;
                gotoButton.Enabled = true;
                useCurrentButton.Enabled = true;
                linkView.Enabled = true;
            }
            else if(navigationStateView.SelectedItems.Count == 0)
            {
                currentState = null;
                nameText.Text = "";
                translationText.Text = "";
                lookAtText.Text = "";
                hiddenCheck.Checked = false;
                linkView.Items.Clear();
                stateUpdate.Enabled = false;
                gotoButton.Enabled = false;
                useCurrentButton.Enabled = false;
                linkView.Enabled = false;
            }
        }

        void navController_NavigationStateSetChanged(NavigationController controller)
        {
            cameraFileTracker.setCurrentFile(controller.CurrentCameraFile);
            setTitleText(controller.CurrentCameraFile);
            navigationStateView.Items.Clear();
            foreach (NavigationState state in controller.NavigationSet.States)
            {
                ListViewItem item = navigationStateView.Items.Add(state.Name, state.Name, 0);
                item.Tag = state;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (navigationStateView != null)
            {
                navigationStateView.Columns[0].Width = -2;
            }
            if (linkView != null)
            {
                linkView.Columns[0].Width = -2;
            }
        }

        private void setTitleText(String filename)
        {
            if (filename != null)
            {
                this.Text = String.Format(titleFormat, filename);
            }
            else
            {
                this.Text = "Navigation State Editor";
            }
        }

        private void stateUpdate_Click(object sender, EventArgs e)
        {
            if (currentState.Name == nameText.Text || navController.getState(nameText.Text) == null)
            {
                navController.NavigationSet.renameState(currentState, nameText.Text);
                currentState.Translation = new Vector3(translationText.Text);
                translationText.Text = currentState.Translation.ToString();
                currentState.LookAt = new Vector3(lookAtText.Text);
                lookAtText.Text = currentState.LookAt.ToString();
                currentState.Hidden = hiddenCheck.Checked;
                currentStateItem.Text = currentState.Name;
            }
            else
            {
                MessageBox.Show(this, String.Format("A state named {0} already exists. Please enter another.", nameText.Text), "Name Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void gotoButton_Click(object sender, EventArgs e)
        {
            DrawingWindowHost window = drawingWindowController.getActiveWindow();
            if (window != null)
            {
                navController.setNavigationState(currentState.Name, window.DrawingWindow);
            }
        }

        private void useCurrentButton_Click(object sender, EventArgs e)
        {
            DrawingWindowHost window = drawingWindowController.getActiveWindow();
            if (window != null)
            {
                translationText.Text = window.DrawingWindow.Translation.ToString();
                lookAtText.Text = window.DrawingWindow.LookAt.ToString();
            }
        }

        private void createStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawingWindowHost window = drawingWindowController.getActiveWindow();
            if (window != null)
            {
                InputResult result = InputBox.GetInput("Create State", "Enter a name for the new state.", this, validateStateName);
                if (result.ok)
                {
                    NavigationState state = new NavigationState(result.text, window.DrawingWindow.LookAt, window.DrawingWindow.Translation, hiddenCheck.Checked);
                    ListViewItem item = navigationStateView.Items.Add(state.Name, state.Name, 0);
                    item.Tag = state;
                    navController.NavigationSet.addState(state);
                }
            }
        }

        private bool validateStateName(String input, out String newPrompt)
        {
            if (navController.getState(input) != null)
            {
                newPrompt = String.Format("A state named {0} already exists. Please enter another name", input);
                return false;
            }
            newPrompt = "";
            return true;
        }

        private void destroyStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            destroySelectedToolStripMenuItem_Click(sender, e);
        }

        private void destroySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkedList<ListViewItem> toRemove = new LinkedList<ListViewItem>();
            foreach (ListViewItem item in navigationStateView.SelectedItems)
            {
                toRemove.AddLast(item);
                NavigationState state = item.Tag as NavigationState;
                navController.NavigationSet.removeState(state);
            }
            foreach (ListViewItem item in toRemove)
            {
                navigationStateView.Items.Remove(item);
            }
        }

        private void createLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in navigationStateView.SelectedItems)
            {
                NavigationState state = item.Tag as NavigationState;
                if (state != currentState)
                {
                    currentState.addAdjacentState(state, NavigationButtons.Down);
                }
            }
        }

        private void createTwoWayLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in navigationStateView.SelectedItems)
            {
                NavigationState state = item.Tag as NavigationState;
                if (state != currentState)
                {
                    currentState.addTwoWayAdjacentState(state, NavigationButtons.Down);
                }
            }
        }

        private void navigationArrowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            navController.ShowOverlays = navigationArrowsToolStripMenuItem.Checked;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cameraFileTracker.saveFile(this);
            if(cameraFileTracker.lastDialogAccepted())
            {
                navController.saveNavigationSet(cameraFileTracker.getCurrentFile());
                setTitleText(cameraFileTracker.getCurrentFile());
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cameraFileTracker.saveFileAs(this);
            if (cameraFileTracker.lastDialogAccepted())
            {
                navController.saveNavigationSet(cameraFileTracker.getCurrentFile());
                setTitleText(cameraFileTracker.getCurrentFile());
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cameraFileTracker.openFile(this);
            if (cameraFileTracker.lastDialogAccepted())
            {
                navController.loadNavigationSet(cameraFileTracker.getCurrentFile());
            }
        }

        void createStateMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = navigationStateView.SelectedItems.Count != 0;
        }

        //link

        void linkView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (linkView.SelectedItems.Count > 0)
            {
                currentLinkItem = linkView.SelectedItems[0];
                currentLink = currentLinkItem.Tag as NavigationLink;
                buttonCombo.SelectedItem = currentLink.Button.ToString();
                radiusUpDown.Value = (decimal)currentLink.VisualRadius;
            }
            else
            {
                currentLinkItem = null;
                currentLink = null;
            }
        }

        private void linkUpdate_Click(object sender, EventArgs e)
        {
            if (currentLink != null && buttonCombo.SelectedItem != null)
            {
                currentLink.Button = (NavigationButtons)Enum.Parse(typeof(NavigationButtons), buttonCombo.SelectedItem.ToString());
                currentLink.VisualRadius = (float)radiusUpDown.Value;
            }
        }

        private void deleteLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkedList<ListViewItem> removeItems = new LinkedList<ListViewItem>();
            foreach (ListViewItem item in linkView.SelectedItems)
            {
                removeItems.AddLast(item);
                NavigationLink link = item.Tag as NavigationLink;
                currentState.removeAdjacentState(link.Destination);
            }
            foreach (ListViewItem item in removeItems)
            {
                linkView.Items.Remove(item);
            }
        }

        private void deleteTwoWayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkedList<ListViewItem> removeItems = new LinkedList<ListViewItem>();
            foreach (ListViewItem item in linkView.SelectedItems)
            {
                removeItems.AddLast(item);
                NavigationLink link = item.Tag as NavigationLink;
                currentState.removeAdjacentState(link.Destination);
                link.Destination.removeAdjacentState(currentState);
            }
            foreach (ListViewItem item in removeItems)
            {
                linkView.Items.Remove(item);
            }
        }

        void linkMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = linkView.SelectedItems.Count == 0;
        }
    }
}
