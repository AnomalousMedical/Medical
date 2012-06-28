using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    public class InputBrowserWindow<BrowseType> : Dialog
    {
        private Tree browserTree;
        private EditBox inputBox;
        private bool allowNameChanges = true;

        private SendResult<BrowseType, String> SendResult;

        protected InputBrowserWindow(String message, String text, String layout = "Medical.GUI.Editor.InputBrowserWindow.layout")
            : base(layout)
        {
            browserTree = new Tree((ScrollView)window.findWidget("ScrollView"));
            browserTree.NodeMouseDoubleClick += new EventHandler<TreeEventArgs>(browserTree_NodeMouseDoubleClick);
            browserTree.AfterSelect += new EventHandler<TreeEventArgs>(browserTree_AfterSelect);
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            Button selectButton = (Button)window.findWidget("Select");
            selectButton.MouseButtonClick += new MyGUIEvent(selectButton_MouseButtonClick);
            Button cancelButton = (Button)window.findWidget("Cancel");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            inputBox = (EditBox)window.findWidget("NameEdit");
            inputBox.EventEditSelectAccept += new MyGUIEvent(inputBox_EventEditSelectAccept);
            inputBox.KeyButtonPressed += inputBox_KeyButtonPressed;

            Accepted = false;
            window.Caption = message;
            Input = text;
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
            treeNode.UserData = node;
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

        public void selectAllText()
        {
            inputBox.setTextSelection(0, uint.MaxValue);
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
                    return (BrowseType)((BrowserNode)browserTree.SelectedNode.UserData).Value;
                }
                return default(BrowseType);
            }
        }

        public String Input
        {
            get
            {
                return inputBox.OnlyText;
            }
            set
            {
                inputBox.OnlyText = value;
            }
        }

        public bool Accepted { get; set; }

        void browserTree_NodeMouseDoubleClick(object sender, TreeEventArgs e)
        {
            if (SelectedValue != null)
            {
                fireSelected();
            }
        }

        void selectButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fireSelected();
        }

        void inputBox_EventEditSelectAccept(Widget source, EventArgs e)
        {
            fireSelected();
        }

        private void fireSelected()
        {
            if (SelectedValue == null)
            {
                MessageBox.show("Please select an item.", "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
            else if (String.IsNullOrEmpty(Input))
            {
                MessageBox.show("Please enter a name.", "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
            else
            {
                Accepted = true;
                close();
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Accepted = false;
            close();
        }

        void browserTree_AfterSelect(object sender, TreeEventArgs e)
        {
            if (allowNameChanges)
            {
                BrowserNode selectedNode = (BrowserNode)e.Node.UserData;
                String defaultName = selectedNode.DefaultName;
                if (!String.IsNullOrEmpty(defaultName))
                {
                    inputBox.Caption = defaultName;
                }
            }
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            browserTree.layout();
        }

        public static void GetInput(Browser browser, bool modal, SendResult<BrowseType, String> sendResult)
        {
            InputBrowserWindow<BrowseType> inputBox = new InputBrowserWindow<BrowseType>(browser.Prompt, "");
            inputBox.setBrowser(browser);
            inputBox.SendResult = sendResult;
            inputBox.Closing += new EventHandler<DialogCancelEventArgs>(inputBox_Closing);
            inputBox.Closed += new EventHandler(inputBox_Closed);
            inputBox.center();
            inputBox.open(modal);
        }

        static void inputBox_Closing(object sender, DialogCancelEventArgs e)
        {
            InputBrowserWindow<BrowseType> inputBox = (InputBrowserWindow<BrowseType>)sender;
            String errorPrompt = null;
            if (inputBox.Accepted && !inputBox.SendResult(inputBox.SelectedValue, inputBox.Input, ref errorPrompt))
            {
                MessageBox.show(errorPrompt, "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                inputBox.selectAllText();
                e.Cancel = true;
            }
        }

        private static void inputBox_Closed(object sender, EventArgs e)
        {
            InputBrowserWindow<BrowseType> inputBox = (InputBrowserWindow<BrowseType>)sender;
            inputBox.Dispose();
        }

        void inputBox_KeyButtonPressed(Widget source, EventArgs e)
        {
            allowNameChanges = false;
            inputBox.KeyButtonPressed -= inputBox_KeyButtonPressed;
        }
    }
}
