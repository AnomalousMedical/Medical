using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    public class BrowserWindow : Dialog
    {
        public event EventHandler ItemSelected;
        public event EventHandler Canceled;

        private Tree browserTree;

        public BrowserWindow(String name)
            :base("Medical.GUI.Editor.BrowserWindow.layout", "Medical.GUI.BrowserWindow." + name)
        {
            browserTree = new Tree((ScrollView)window.findWidget("ScrollView"));
            browserTree.NodeMouseDoubleClick += new EventHandler<TreeEventArgs>(browserTree_NodeMouseDoubleClick);
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            Button selectButton = (Button)window.findWidget("Select");
            selectButton.MouseButtonClick += new MyGUIEvent(selectButton_MouseButtonClick);
            Button cancelButton = (Button)window.findWidget("Cancel");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            browserTree.layout();
        }

        public void setBrowser(Browser browser)
        {
            browserTree.Nodes.clear();
            TreeNode parentNode = addNodes(browser.getTopNode());
            browserTree.Nodes.add(parentNode);
            parentNode.Expanded = true;
        }

        private TreeNode addNodes(BrowserNode node)
        {
            TreeNode treeNode = new TreeNode(node.Text);
            treeNode.UserData = node.Value;
            foreach (BrowserNode child in node.getChildIterator())
            {
                treeNode.Children.add(addNodes(child));
            }
            return treeNode;
        }

        /// <summary>
        /// The value that is selected on the tree. Can be null.
        /// </summary>
        public Object SelectedValue
        {
            get
            {
                if (browserTree.SelectedNode != null)
                {
                    return browserTree.SelectedNode.UserData;
                }
                return null;
            }
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
        }

        void browserTree_NodeMouseDoubleClick(object sender, TreeEventArgs e)
        {
            if (SelectedValue != null)
            {
                if (ItemSelected != null)
                {
                    ItemSelected.Invoke(this, EventArgs.Empty);
                }
                close();
            }
        }

        void selectButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (ItemSelected != null)
            {
                ItemSelected.Invoke(this, EventArgs.Empty);
            }
            close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (Canceled != null)
            {
                Canceled.Invoke(this, EventArgs.Empty);
            }
            close();
        }
    }
}
