using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public partial class NavigationMenuEditor : GUIElement
    {
        NavigationController navController;
        private NavigationMenus menus;
        ImageRenderer imageRenderer;
        private Point mouseDownLoc;
        private NavigationEditor navigationEditor;
        DrawingWindowController drawingWindowController;

        public NavigationMenuEditor(NavigationController navController, ImageRenderer imageRenderer, NavigationEditor navigationEditor, DrawingWindowController drawingWindowController)
        {
            InitializeComponent();
            this.drawingWindowController = drawingWindowController;
            this.navController = navController;
            this.navigationEditor = navigationEditor;
            navController.NavigationStateSetChanged += new NavigationControllerEvent(navController_NavigationStateSetChanged);
            this.imageRenderer = imageRenderer;
            emptySpaceMenu.Opening += new CancelEventHandler(emptySpaceMenu_Opening);
            menuTree.MouseDown += new MouseEventHandler(menuTree_MouseDown);
            menuTree.AfterSelect += new TreeViewEventHandler(menuTree_AfterSelect);
            menuTree.MouseDoubleClick += new MouseEventHandler(menuTree_MouseDoubleClick);
        }

        void menuTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode node = menuTree.GetNodeAt(e.Location);
            if (node != null)
            {
                NavigationState state = node.Tag as NavigationState;
                if (state != null)
                {
                    navController.setNavigationState(state.Name, drawingWindowController.getActiveWindow().DrawingWindow);
                }
            }
        }

        void menuTree_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownLoc = e.Location;
        }

        void emptySpaceMenu_Opening(object sender, CancelEventArgs e)
        {
            if (menuTree.GetNodeAt(mouseDownLoc) != null)
            {
                e.Cancel = true;
                nodeMenu.Show(menuTree.PointToScreen(mouseDownLoc));
            }
        }

        void navController_NavigationStateSetChanged(NavigationController controller)
        {
            menuTree.Nodes.Clear();
            menus = controller.NavigationSet.Menus;
            foreach (NavigationMenuEntry entry in menus.ParentEntries)
            {
                TreeNode node = new TreeNode(entry.Text);
                node.Tag = entry;
                menuTree.Nodes.Add(node);
                if (entry.SubEntries != null)
                {
                    foreach (NavigationMenuEntry subEntry in entry.SubEntries)
                    {
                        addRecursiveMenu(node, subEntry);
                    }
                }
                if (entry.States != null)
                {
                    foreach (NavigationState state in entry.States)
                    {
                        TreeNode stateNode = new TreeNode(state.Name);
                        stateNode.Tag = state;
                        node.Nodes.Add(stateNode);
                    }
                }
            }
        }

        void addRecursiveMenu(TreeNode parentNode, NavigationMenuEntry entry)
        {
            TreeNode node = new TreeNode(entry.Text);
            node.Tag = entry;
            parentNode.Nodes.Add(node);
            if (entry.SubEntries != null)
            {
                foreach (NavigationMenuEntry subEntry in entry.SubEntries)
                {
                    addRecursiveMenu(node, subEntry);
                }
            }
            if (entry.States != null)
            {
                foreach (NavigationState state in entry.States)
                {
                    TreeNode stateNode = new TreeNode(state.Name);
                    stateNode.Tag = state;
                    node.Nodes.Add(stateNode);
                }
            }
        }

        private void renderButton_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = imageRenderer.renderImage(51, 38, false, Color.Black, 8);
            ThumbnailImage = bitmap;
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this.FindForm()) == DialogResult.OK)
            {
                ThumbnailImage = new Bitmap(openFileDialog.FileName);
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            TreeNode selected = menuTree.SelectedNode;
            if (selected != null)
            {
                NavigationMenuEntry entry = selected.Tag as NavigationMenuEntry;
                if (entry != null)
                {
                    entry.Text = textText.Text;
                    if (thumbnailPanel.BackgroundImage != null)
                    {
                        entry.Thumbnail = new Bitmap(thumbnailPanel.BackgroundImage);
                    }
                    else
                    {
                        entry.Thumbnail = null;
                    }
                    selected.Text = entry.Text;
                }
                else
                {
                    NavigationState state = selected.Tag as NavigationState;
                    if (state != null)
                    {
                        if (thumbnailPanel.BackgroundImage != null)
                        {
                            state.Thumbnail = new Bitmap(thumbnailPanel.BackgroundImage);
                        }
                        else
                        {
                            state.Thumbnail = null;
                        }
                    }
                }
            }
        }

        void menuTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selected = menuTree.SelectedNode;
            if (selected != null)
            {
                NavigationMenuEntry entry = selected.Tag as NavigationMenuEntry;
                if (entry != null)
                {
                    textText.Text = entry.Text;
                    if (entry.Thumbnail != null)
                    {
                        ThumbnailImage = new Bitmap(entry.Thumbnail);
                    }
                    else
                    {
                        ThumbnailImage = null;
                    }
                }
                else
                {
                    NavigationState state = selected.Tag as NavigationState;
                    if (state != null)
                    {
                        if (state.Thumbnail != null)
                        {
                            ThumbnailImage = new Bitmap(state.Thumbnail);
                        }
                        else
                        {
                            ThumbnailImage = null;
                        }
                    }
                }
            }
        }

        private Bitmap ThumbnailImage
        {
            get
            {
                return thumbnailPanel.BackgroundImage as Bitmap;
            }
            set
            {
                if (thumbnailPanel.BackgroundImage != null)
                {
                    thumbnailPanel.BackgroundImage.Dispose();
                }
                thumbnailPanel.BackgroundImage = value;
            }
        }

        private void addParentMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavigationMenuEntry newEntry = menus.addParentEntry("New Menu");
            TreeNode node = new TreeNode(newEntry.Text);
            node.Tag = newEntry;
            menuTree.Nodes.Add(node);
            menuTree.SelectedNode = node;
        }

        private void addStatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = menuTree.SelectedNode;
            NavigationMenuEntry entry = parentNode.Tag as NavigationMenuEntry;
            if (entry != null)
            {
                foreach (NavigationState state in navigationEditor.SelectedStates)
                {
                    menus.addState(entry, state);
                    TreeNode node = new TreeNode(state.Name);
                    node.Tag = state;
                    parentNode.Nodes.Add(node);
                }
            }
        }

        private void addSubEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = menuTree.SelectedNode;
            NavigationMenuEntry entry = parentNode.Tag as NavigationMenuEntry;
            if (entry != null)
            {
                NavigationMenuEntry newEntry = menus.addSubEntry(entry, "New Menu");
                TreeNode node = new TreeNode(newEntry.Text);
                node.Tag = newEntry;
                parentNode.Nodes.Add(node);
                menuTree.SelectedNode = node;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = menuTree.SelectedNode;
            NavigationMenuEntry entry = node.Tag as NavigationMenuEntry;
            if (entry != null)
            {
                TreeNode parentNode = menuTree.SelectedNode.Parent;
                if (parentNode == null)
                {
                    menuTree.Nodes.Remove(node);
                    menus.removeParentEntry(entry);
                }
                else
                {
                    NavigationMenuEntry parentEntry = parentNode.Tag as NavigationMenuEntry;
                    parentNode.Nodes.Remove(node);
                    menus.removeSubEntry(parentEntry, entry);
                }
            }
            else
            {
                NavigationState state = node.Tag as NavigationState;
                TreeNode parentNode = menuTree.SelectedNode.Parent;
                NavigationMenuEntry parentEntry = parentNode.Tag as NavigationMenuEntry;
                parentNode.Nodes.Remove(node);
                menus.removeState(parentEntry, state);
            }
        }
    }
}
