using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;
using Logging;

namespace Medical.GUI
{
    public class MedicalUICallback : EditUICallback
    {
        public delegate void CustomQueryDelegate(SendResult<Object> resultCallback, params Object[] args);

        BrowserWindow browserWindow;
        private SendResult<Object> showBrowserCallback;
        private Dictionary<Object, CustomQueryDelegate> customQueries = new Dictionary<object,CustomQueryDelegate>();

        public MedicalUICallback(BrowserWindow browserWindow)
        {
            this.browserWindow = browserWindow;
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
            browserWindow.ItemSelected += browserWindow_ItemSelected;
            browserWindow.Canceled += browserWindow_Canceled;
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

        public void runCustomQuery(Object queryKey, SendResult<Object> resultCallback, params Object[] args)
        {
            CustomQueryDelegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                queryDelegate.Invoke(resultCallback, args);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
            }
        }

        public void addCustomQuery(Object queryKey, CustomQueryDelegate queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void removeCustomQuery(Object queryKey)
        {
            customQueries.Remove(queryKey);
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
            browserWindow.ItemSelected -= browserWindow_ItemSelected;
            browserWindow.Canceled -= browserWindow_Canceled;
        }

        void browserWindow_Canceled(object sender, EventArgs e)
        {
            showBrowserCallback = null;
            browserWindow.ItemSelected -= browserWindow_ItemSelected;
            browserWindow.Canceled -= browserWindow_Canceled;
        }
    }
}
