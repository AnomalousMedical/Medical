using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    class SaveTimelineDialog : Dialog
    {
        public event EventHandler SaveFile;

        private Edit filename;

        public SaveTimelineDialog()
            :base("Medical.GUI.Timeline.SaveTimelineDialog.layout")
        {
            filename = window.findWidget("Filename") as Edit;
            filename.EventEditSelectAccept += new MyGUIEvent(filename_EventEditSelectAccept);

            Button saveButton = window.findWidget("SaveButton") as Button;
            saveButton.MouseButtonClick += new MyGUIEvent(saveButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        public String Filename
        {
            get
            {
                if (!filename.Caption.EndsWith(".tl"))
                {
                    return filename.Caption + ".tl";
                }
                return filename.Caption;
            }
            set
            {
                filename.Caption = value;
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void filename_EventEditSelectAccept(Widget source, EventArgs e)
        {
            save();
        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
        {
            save();
        }

        private void save()
        {
            if (filename.Caption.Length > 0)
            {
                if (SaveFile != null)
                {
                    SaveFile.Invoke(this, EventArgs.Empty);
                }
                this.close();
            }
            else
            {
                MessageBox.show("Please enter a file name.", "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }
    }
}
