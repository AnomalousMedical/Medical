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
        BrowserWindow browserWindow;
        private SendResult<Object> showBrowserCallback;

        public MedicalUICallback(BrowserWindow browserWindow)
        {
            this.browserWindow = browserWindow;
            if (browserWindow != null)
            {
                browserWindow.ItemSelected += new EventHandler(browserWindow_ItemSelected);
                browserWindow.Canceled += new EventHandler(browserWindow_Canceled);
            }
        }

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
            browserWindow.setBrowser(browser);
            showBrowserCallback = resultCallback;
            browserWindow.open(true);
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

        void browserWindow_ItemSelected(object sender, EventArgs e)
        {
            if (showBrowserCallback != null)
            {
                String error = null;
                showBrowserCallback.Invoke(browserWindow.SelectedValue, ref error);
                showBrowserCallback = null;
            }
        }

        void browserWindow_Canceled(object sender, EventArgs e)
        {
            showBrowserCallback = null;
        }
    }
}
