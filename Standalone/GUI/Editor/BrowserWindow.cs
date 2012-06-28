using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    public class BrowserWindow<BrowseType> : Dialog
    {
        private SendResult<BrowseType> SendResult;

        private Tree browserTree;

        public BrowserWindow(String message)
            :base("Medical.GUI.Editor.BrowserWindow.layout")
        {
            browserTree = new Tree((ScrollView)window.findWidget("ScrollView"));
            browserTree.NodeMouseDoubleClick += new EventHandler<TreeEventArgs>(browserTree_NodeMouseDoubleClick);
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
            window.Caption = message;

            Button selectButton = (Button)window.findWidget("Select");
            selectButton.MouseButtonClick += new MyGUIEvent(selectButton_MouseButtonClick);
            Button cancelButton = (Button)window.findWidget("Cancel");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Accepted = false;
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            browserTree.layout();
        }

        public void setBrowser(Browser browser)
        {
            browserTree.Nodes.clear();
            TreeNode parentNode = addNodes(browser.getTopNode(), browser.DefaultSelection);
            browserTree.Nodes.add(parentNode);
            parentNode.Expanded = true;
        }

        private TreeNode addNodes(BrowserNode node, BrowserNode defaultNode)
        {
            TreeNode treeNode = new TreeNode(node.Text);
            treeNode.ImageResource = node.IconName;
            treeNode.UserData = node.Value;
            if (node == defaultNode)
            {
                browserTree.SelectedNode = treeNode;
            }
            foreach (BrowserNode child in node.getChildIterator())
            {
                treeNode.Children.add(addNodes(child, defaultNode));
            }
            return treeNode;
        }

        /// <summary>
        /// The value that is selected on the tree. Can be null.
        /// </summary>
        public BrowseType SelectedValue
        {
            get
            {
                if (browserTree.SelectedNode != null)
                {
                    return (BrowseType)browserTree.SelectedNode.UserData;
                }
                return default(BrowseType);
            }
        }

        public bool Accepted { get; set; }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
        }

        void browserTree_NodeMouseDoubleClick(object sender, TreeEventArgs e)
        {
            if (SelectedValue != null)
            {
                Accepted = true;
                close();
            }
        }

        void selectButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Accepted = true;
            close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Accepted = false;
            close();
        }

        public static void GetInput(Browser browser, bool modal, SendResult<BrowseType> sendResult)
        {
            BrowserWindow<BrowseType> inputBox = new BrowserWindow<BrowseType>(browser.Prompt);
            inputBox.setBrowser(browser);
            inputBox.SendResult = sendResult;
            inputBox.Closing += new EventHandler<DialogCancelEventArgs>(inputBox_Closing);
            inputBox.Closed += new EventHandler(inputBox_Closed);
            inputBox.center();
            inputBox.open(modal);
        }

        static void inputBox_Closing(object sender, DialogCancelEventArgs e)
        {
            BrowserWindow<BrowseType> inputBox = (BrowserWindow<BrowseType>)sender;
            String errorPrompt = null;
            if (inputBox.Accepted && !inputBox.SendResult(inputBox.SelectedValue, ref errorPrompt))
            {
                MessageBox.show(errorPrompt, "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                e.Cancel = true;
            }
        }

        private static void inputBox_Closed(object sender, EventArgs e)
        {
            BrowserWindow<BrowseType> inputBox = (BrowserWindow<BrowseType>)sender;
            inputBox.Dispose();
        }
    }
}
