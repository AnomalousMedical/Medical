using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class EditorTaskbarFileButton : Component
    {
        private Button fileButton;

        public event Action<EditorTaskbarFileButton> Closed;
        public event Action<EditorTaskbarFileButton> ChangeFile;

        public EditorTaskbarFileButton(Widget parentWidget, String file, int left)
            :base("Medical.GUI.EditorTaskbar.EditorTaskbarFileButton.layout")
        {
            widget.attachToWidget(parentWidget);

            fileButton = (Button)widget.findWidget("FileButton");
            fileButton.MouseButtonClick += new MyGUIEvent(fileButton_MouseButtonClick);
            fileButton.Caption = File = file;

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            int textWidth = fileButton.getTextSize().Width;
            widget.setSize(textWidth + widget.Width, widget.Height);
            widget.setPosition(left, 0);
        }

        public String File { get; private set; }

        public bool CurrentFile
        {
            get
            {
                return fileButton.Selected;
            }
            set
            {
                fileButton.Selected = value;
            }
        }

        public int Width
        {
            get
            {
                return widget.Width;
            }
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (Closed != null)
            {
                Closed.Invoke(this);
            }
        }

        void fileButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (ChangeFile != null)
            {
                ChangeFile.Invoke(this);
            }
        }
    }
}
