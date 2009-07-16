using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Medical
{
    /// <summary>
    /// This delegate can be passed to the GetInput function to validate the
    /// user's input. Return false if the input is invalid and fill out the
    /// newPrompt with something meaningful.
    /// </summary>
    /// <param name="input">The input string entered.</param>
    /// <param name="newPrompt">An output error message and new prompt for the dialog.</param>
    /// <returns>True if the input is valid, false if it is invalid.</returns>
    public delegate bool ValidateInput(String input, out String newPrompt);

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

        public static InputResult GetInput(String title, String message, IWin32Window parent, ValidateInput validate)
        {
            InputResult result = GetInput(title, message, parent, "");
            if (validate != null)
            {
                String error;
                while (result.ok && !validate.Invoke(result.text, out error))
                {
                    result = GetInput(title, error, parent, result.text);
                }
            }
            return result;
        }

        private static InputResult GetInput(String title, String message, IWin32Window parent, String text)
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