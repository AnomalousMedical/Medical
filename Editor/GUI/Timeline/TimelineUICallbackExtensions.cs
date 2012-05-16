using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Medical.Controller;
using System.IO;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using Medical.GUI.AnomalousMvc;
using Medical.Model;

namespace Medical.GUI
{
    class TimelineUICallbackExtensions
    {
        private MedicalUICallback medicalUICallback;
        private TimelineController editorTimelineController;
        private BrowserWindow browserWindow;
        private QuestionEditor questionEditor;
        private StandaloneController standaloneController;
        private TimelinePropertiesController propertiesController;

        private SendResult<Object> browserResultCallback;
        private SendResult<Object> showQuestionEditorCallback;

        public TimelineUICallbackExtensions(StandaloneController standaloneController, MedicalUICallback medicalUICallback, TimelineController editorTimelineController, TimelinePropertiesController propertiesController)
        {
            this.medicalUICallback = medicalUICallback;
            this.editorTimelineController = editorTimelineController;
            this.browserWindow = propertiesController.EditorPlugin.BrowserWindow;
            this.questionEditor = propertiesController.QuestionEditor;
            this.standaloneController = standaloneController;
            this.propertiesController = propertiesController;

            medicalUICallback.addCustomQuery(ShowTimelineGUIAction.CustomEditQueries.ChangeGUIType, changeGUIType);
            medicalUICallback.addCustomQuery(ShowTimelineGUIAction.CustomEditQueries.GetGUIData, getGUIData);
            medicalUICallback.addCustomQuery(ShowPromptAction.CustomEditQueries.OpenQuestionEditor, openQuestionEditor);
        }

        private void captureCameraPosition(SendResult<Object> resultCallback, params Object[] args)
        {
            CameraPosition camPos = (CameraPosition)args[0];
            SceneViewWindow activeWindow = standaloneController.SceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                camPos.Translation = activeWindow.Translation;
                camPos.LookAt = activeWindow.LookAt;
            }
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
                showQuestionEditorCallback.Invoke(new EditQuestionsResults(questionEditor.Question, questionEditor.SoundFile), ref error);
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
            browserResultCallback = resultCallback;
            browserWindow.setBrowser(editorTimelineController.GUIFactory.GUIBrowser);
            browserWindow.ItemSelected += browserWindow_ItemSelected;
            browserWindow.Canceled += browserWindow_Canceled;
            browserWindow.open(true);
        }

        void browserWindow_ItemSelected(object sender, EventArgs e)
        {
            if (browserResultCallback != null)
            {
                String error = null;
                browserResultCallback.Invoke(browserWindow.SelectedValue, ref error);
                browserResultCallback = null;
            }
            browserWindow.ItemSelected -= browserWindow_ItemSelected;
            browserWindow.Canceled -= browserWindow_Canceled;
        }

        void browserWindow_Canceled(object sender, EventArgs e)
        {
            browserResultCallback = null;
            browserWindow.ItemSelected -= browserWindow_ItemSelected;
            browserWindow.Canceled -= browserWindow_Canceled;
        }
    }
}
