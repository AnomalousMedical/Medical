using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    public delegate void FileChosenCallback(String filename);

    class TimelineFileBrowserDialog : Dialog
    {
        private MultiList fileList;
        private TimelineController timelineController;
        private FileChosenCallback callback;
        private String filterString;

        public TimelineFileBrowserDialog(TimelineController timelineController)
            : base("Medical.GUI.Timeline.TimelineFileBrowserDialog.layout")
        {
            this.timelineController = timelineController;

            fileList = window.findWidget("FileList") as MultiList;
            fileList.addColumn("File", fileList.Width);

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

        protected override void onShown(EventArgs args)
        {
            if (callback == null)
            {
                throw new Exception("ChooseTimelineDialog opened without a callback. Use the promptForFile function not the show functions to show the dialog.");
            }
            fileList.removeAllItems();
            String[] files = timelineController.listResourceFiles(filterString);
            foreach (String file in files)
            {
                fileList.addItem(Path.GetFileNameWithoutExtension(file), Path.GetFileName(file));
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
            uint selectedIndex = fileList.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                SelectedFile = fileList.getItemDataAt(selectedIndex).ToString();
                callback.Invoke(SelectedFile);
                this.close();
            }
            else
            {
                MessageBox.show("Please select a file to open.", "Warning", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
            }
        }

        void importButton_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }
    }
}
