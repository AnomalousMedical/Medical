using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using System.IO;
using Medical.Controller;

namespace Medical.GUI
{
    public class BrowserWindow<BrowseType> : Dialog
    {
        private SendResult<BrowseType> SendResult;

        private Tree browserTree;
        private Button importButton;

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
            importButton = (Button)window.findWidget("Import");

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
            browserTree.layout();
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
            if (SelectedValue != null)
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

        private static BrowserWindow<BrowseType> createInputBox(Browser browser, bool modal, SendResult<BrowseType> sendResult)
        {
            BrowserWindow<BrowseType> inputBox = new BrowserWindow<BrowseType>(browser.Prompt);
            inputBox.setBrowser(browser);
            inputBox.SendResult = sendResult;
            inputBox.Closing += new EventHandler<DialogCancelEventArgs>(inputBox_Closing);
            inputBox.Closed += new EventHandler(inputBox_Closed);
            inputBox.center();
            inputBox.open(modal);
            return inputBox;
        }

        public static void GetInput(Browser browser, bool modal, SendResult<BrowseType> sendResult)
        {
            BrowserWindow<BrowseType> inputBox = createInputBox(browser, modal, sendResult);
            inputBox.importButton.Visible = false;
        }

        public static void GetInput(Browser browser, bool modal, SendResult<BrowseType> sendResult, Action<String, Action<String>> importCallback, String prompt, String wildcard, String extension)
        {
            BrowserWindow<BrowseType> inputBox = createInputBox(browser, modal, sendResult);
            inputBox.importButton.MouseButtonClick += (source, e) =>
            {
                FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, prompt, wildcard: wildcard);
                openDialog.showModal((result, paths) =>
                {
                    if (result == NativeDialogResult.OK)
                    {
                        String path = paths.First();
                        String ext = Path.GetExtension(path);
                        if (ext.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                        {
                            importCallback(path, previewPath =>
                            {
                                ThreadManager.invoke(new Action(() =>
                                {
                                    BrowserNode node = new BrowserNode(Path.GetFileName(previewPath), previewPath);
                                    inputBox.browserTree.Nodes.First().Children.add(inputBox.addNodes(node, node));
                                    inputBox.browserTree.layout();
                                }));
                            });
                        }
                        else
                        {
                            MessageBox.show(String.Format("Cannot open a file with extension '{0}'.", extension), "Can't Load File", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                        }
                    }
                });
            };
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
