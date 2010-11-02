using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    class OpenTimelineDialog : Dialog
    {
        public event EventHandler OpenFile;

        private MultiList fileList;
        private TimelineController timelineController;

        public OpenTimelineDialog(TimelineController timelineController)
            :base("Medical.GUI.Timeline.OpenTimelineDialog.layout")
        {
            this.timelineController = timelineController;

            fileList = window.findWidget("FileList") as MultiList;
            fileList.addColumn("File", fileList.Width);

            Button openButton = window.findWidget("OpenButton") as Button;
            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        protected override void onShown(EventArgs args)
        {
            fileList.removeAllItems();
            String[] files = timelineController.listResourceFiles("*.tl");
            foreach (String file in files)
            {
                fileList.addItem(Path.GetFileNameWithoutExtension(file), Path.GetFileName(file));
            }
            base.onShown(args);
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
                if (OpenFile != null)
                {
                    OpenFile.Invoke(this, EventArgs.Empty);
                }
                this.close();
            }
            else
            {
                MessageBox.show("Please select a file to open.", "Warning", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
            }
        }
    }
}
