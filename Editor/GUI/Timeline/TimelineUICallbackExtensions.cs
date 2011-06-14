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
        private QuestionEditor questionEditor;

        private SendResult<Object> changeGUITypeCallback;
        private SendResult<Object> showQuestionEditorCallback;

        public TimelineUICallbackExtensions(MedicalUICallback medicalUICallback, TimelineController editorTimelineController, BrowserWindow browserWindow, QuestionEditor questionEditor)
        {
            this.medicalUICallback = medicalUICallback;
            this.editorTimelineController = editorTimelineController;
            this.browserWindow = browserWindow;
            this.questionEditor = questionEditor;

            medicalUICallback.addCustomQuery(TimelineCustomQueries.ChangeGUIType, changeGUIType);
            medicalUICallback.addCustomQuery(TimelineCustomQueries.GetGUIData, getGUIData);
            medicalUICallback.addCustomQuery(TimelineCustomQueries.OpenQuestionEditor, openQuestionEditor);
        }

        private void openQuestionEditor(SendResult<Object> resultCallback, params Object[] args)
        {
            showQuestionEditorCallback = resultCallback;
            questionEditor.clear();
            ShowPromptAction showPromptAction = (ShowPromptAction)args[0];
            foreach (PromptQuestion question in showPromptAction.Questions)
            {
                questionEditor.Question = question;
                break;
            }
            questionEditor.SoundFile = showPromptAction.SoundFile;
            questionEditor.Closed += questionEditor_Closed;
            questionEditor.open(true);
        }

        void questionEditor_Closed(object sender, EventArgs e)
        {
            if (questionEditor.Ok && showQuestionEditorCallback != null)
            {
                String error = null;
                showQuestionEditorCallback.Invoke(questionEditor.Question, ref error);
            }
            questionEditor.Closed -= questionEditor_Closed;
            showQuestionEditorCallback = null;
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
