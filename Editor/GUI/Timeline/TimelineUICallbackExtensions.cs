using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.GUI
{
    class TimelineUICallbackExtensions
    {
        private MedicalUICallback medicalUICallback;
        private TimelineController editorTimelineController;
        private BrowserWindow browserWindow;

        private SendResult<Object> changeGUITypeCallback;

        public TimelineUICallbackExtensions(MedicalUICallback medicalUICallback, TimelineController editorTimelineController, BrowserWindow browserWindow)
        {
            this.medicalUICallback = medicalUICallback;
            this.editorTimelineController = editorTimelineController;
            this.browserWindow = browserWindow;

            medicalUICallback.addCustomQuery(TimelineCustomQueries.ChangeGUIType, changeGUIType);
            medicalUICallback.addCustomQuery(TimelineCustomQueries.GetGUIData, getGUIData);
        }

        private void getGUIData(SendResult<Object> resultCallback, params Object[] args)
        {
            String prototypeName = (String)args[0];
            TimelineGUIData guiData = editorTimelineController.GUIFactory.getGUIData(prototypeName);
            String error = null;
            resultCallback(guiData, ref error);
        }

        private void changeGUIType(SendResult<Object> resultCallback, params Object[] args)
        {
            changeGUITypeCallback = resultCallback;
            browserWindow.setBrowser(editorTimelineController.GUIFactory.GUIBrowser);
            browserWindow.ItemSelected += browserWindow_ChangeGUIType_ItemSelected;
            browserWindow.Canceled += browserWindow_ChangeGUIType_Canceled;
            browserWindow.open(true);
        }

        void browserWindow_ChangeGUIType_ItemSelected(object sender, EventArgs e)
        {
            if (changeGUITypeCallback != null)
            {
                String error = null;
                changeGUITypeCallback.Invoke(browserWindow.SelectedValue, ref error);
                changeGUITypeCallback = null;
            }
            browserWindow.ItemSelected -= browserWindow_ChangeGUIType_ItemSelected;
            browserWindow.Canceled -= browserWindow_ChangeGUIType_Canceled;
        }

        void browserWindow_ChangeGUIType_Canceled(object sender, EventArgs e)
        {
            changeGUITypeCallback = null;
            browserWindow.ItemSelected -= browserWindow_ChangeGUIType_ItemSelected;
            browserWindow.Canceled -= browserWindow_ChangeGUIType_Canceled;
        }
    }
}
