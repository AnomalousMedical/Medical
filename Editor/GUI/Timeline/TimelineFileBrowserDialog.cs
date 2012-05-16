using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    class TimelineFileBrowserDialog : Dialog, ITimelineFileBrowser
    {
        private MultiListBox fileList;
        private TimelineController timelineController;
        private FileChosenCallback callback;
        private String filterString;

        public TimelineFileBrowserDialog(TimelineController timelineController)
            : base("Medical.GUI.Timeline.TimelineFileBrowserDialog.layout")
        {
            this.timelineController = timelineController;

            fileList = window.findWidget("FileList") as MultiListBox;
            fileList.addColumn("File", 50);
            fileList.setColumnResizingPolicyAt(0, ResizingPolicy.Fill);
            fileList.ListSelectAccept += new MyGUIEvent(fileList_ListSelectAccept);

            Button openButton = window.findWidget("OpenButton") as Button;
            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button importButton = window.findWidget("ImportButton") as Button;
            importButton.MouseButtonClick += new MyGUIEvent(importButton_MouseButtonClick);
        }

        /// <summary>
        /// This method is the one that should be called to open a file. It sets up the appropriate callbacks.
        /// </summary>
        /// <param name="callback"></param>
        public void promptForFile(String filterString, FileChosenCallback callback)
        {
            this.filterString = filterString;
            this.callback = callback;
            this.open(true);
        }

        public override void deserialize(Engine.ConfigFile configFile)
        {
            base.deserialize(configFile);
        }

        protected override void onShown(EventArgs args)
        {
            if (callback == null)
            {
                throw new Exception("ChooseTimelineDialog opened without a callback. Use the promptForFile function not the show functions to show the dialog.");
            }
            fileList.removeAllItems();
            String[] files = timelineController.LEGACY_listResourceFiles(filterString);
            foreach (String file in files)
            {
                String fileName = Path.GetFileName(file);
                fileList.addItem(fileName, fileName);
            }
            base.onShown(args);
        }

        protected override void onClosed(EventArgs args)
        {
            callback = null;
            base.onClosed(args);
        }

        public String SelectedFile { get; private set; }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void openButton_MouseButtonClick(Widget source, EventArgs e)
        {
            acceptSelected();
        }

        void fileList_ListSelectAccept(Widget source, EventArgs e)
        {
            acceptSelected();
        }

        private void acceptSelected()
        {
            uint selectedIndex = fileList.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                SelectedFile = fileList.getItemDataAt(selectedIndex).ToString();
                callback.Invoke(SelectedFile);
                if (Modal)
                {
                    this.close();
                }
            }
            else
            {
                MessageBox.show("Please select a file to open.", "Warning", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
            }
        }

        void importButton_MouseButtonClick(Widget source, EventArgs e)
        {
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Choose files to import", "", "", "", true))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    foreach (String path in fileDialog.Paths)
                    {
                        bool copyFile = true;
                        String filename = Path.GetFileName(path);
                        if (timelineController.resourceExists(filename))
                        {
                            copyFile = MessageDialog.showQuestionDialog(MainWindow.Instance, String.Format("The file {0} already exists in the project. Would you like to overwrite?", filename), "Overwrite?") == NativeDialogResult.YES;
                        }
                        if(copyFile)
                        {
                            try
                            {
                                timelineController.importFile(path);
                                uint index;
                                String itemName = Path.GetFileNameWithoutExtension(path);
                                if (!fileList.findSubItemWith(0, itemName, out index))
                                {
                                    fileList.addItem(itemName, Path.GetFileName(path));
                                    index = fileList.getItemCount() - 1;
                                }
                                fileList.setIndexSelected(index);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.show("Error importing file. Reason:\n" + ex.Message, "Import Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                            }
                        }
                    }
                }
            }
        }
    }
}
