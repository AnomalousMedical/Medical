using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class MedicalUICallback : EditUICallback
    {
        public void getInputString(string prompt, SendResult<string> resultCallback)
        {
            InputBox.GetInput("Enter value", prompt, true, resultCallback);
        }

        public EditInterface getSelectedEditInterface()
        {
            return SelectedEditInterface;
        }

        public void showBrowser(Browser browser, SendResult<object> resultCallback)
        {
            throw new NotImplementedException();
        }

        public void showFolderBrowserDialog(SendResult<string> resultCallback)
        {
            throw new NotImplementedException();
        }

        public void showOpenFileDialog(string filterString, SendResult<string> resultCallback)
        {
            throw new NotImplementedException();
        }

        public void showSaveFileDialog(string filterString, SendResult<string> resultCallback)
        {
            throw new NotImplementedException();
        }

        public EditInterface SelectedEditInterface { get; set; }
    }
}
