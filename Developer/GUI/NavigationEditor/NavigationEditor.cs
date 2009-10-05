using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class NavigationStateSelector : GUIElement
    {
        private NavigationController navController;
        private DrawingWindowController drawingWindowController;
        private NavigationState currentState;
        private ListViewItem currentStateItem;

        public NavigationStateSelector(NavigationController navController, DrawingWindowController drawingWindowController)
        {
            InitializeComponent();
            this.navController = navController;
            this.drawingWindowController = drawingWindowController;
            navController.NavigationStateSetChanged += new NavigationControllerEvent(navController_NavigationStateSetChanged);
            navigationStateView.SelectedIndexChanged += new EventHandler(navigationStateView_SelectedIndexChanged);
        }

        void navigationStateView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (navigationStateView.SelectedItems.Count > 0)
            {
                currentStateItem = navigationStateView.SelectedItems[0];
                currentState = currentStateItem.Tag as NavigationState;
                nameText.Text = currentState.Name;
                translationText.Text = currentState.Translation.ToString();
                lookAtText.Text = currentState.LookAt.ToString();
                hiddenCheck.Checked = currentState.Hidden;
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
            else
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

        private void stateUpdate_Click(object sender, EventArgs e)
        {

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

        private void showNavigationCheck_CheckedChanged(object sender, EventArgs e)
        {
            navController.ShowOverlays = showNavigationCheck.Checked;
        }
    }
}
