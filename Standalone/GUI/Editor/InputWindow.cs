using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class InputWindow : Dialog
    {
        private EditBox inputBox;

        protected InputWindow(String message, String text, String layout)
            : base(layout)
        {
            Button selectButton = (Button)window.findWidget("Select");
            selectButton.MouseButtonClick += new MyGUIEvent(selectButton_MouseButtonClick);
            Button cancelButton = (Button)window.findWidget("Cancel");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            inputBox = (EditBox)window.findWidget("NameEdit");
            inputBox.EventEditSelectAccept += new MyGUIEvent(inputBox_EventEditSelectAccept);

            Accepted = false;
            window.Caption = message;
            Input = text;
        }

        public void selectAllText()
        {
            inputBox.setTextSelection(0, uint.MaxValue);
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
            if (String.IsNullOrEmpty(Input))
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
    }
}
