using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Medical
{
    public partial class InputBox : Form
    {
        public InputBox(String title, String message, String text)
        {
            InitializeComponent();
            this.Text = title;
            this.prompt.Text = message;
            this.inputText.Text = text;
            inputText.AcceptsReturn = true;
        }

        public String getText()
        {
            return this.inputText.Text;
        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }

        public static InputResult GetInput(String title, String message, IWin32Window parent)
        {
            return GetInput(title, message, parent, "");
        }

        public static InputResult GetInput(String title, String message, IWin32Window parent, String text)
        {
            InputResult inputResult = new InputResult();
            using (InputBox inputBox = new InputBox(title, message, text))
            {
                DialogResult result = inputBox.ShowDialog(parent);
                if (result == DialogResult.OK)
                {
                    inputResult.ok = true;
                    inputResult.text = inputBox.getText();
                }
            }
            return inputResult;
        }
    }

    public class InputResult
    {
        public bool ok = false;
        public String text;
    }
}