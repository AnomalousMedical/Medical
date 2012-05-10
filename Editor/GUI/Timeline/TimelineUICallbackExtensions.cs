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
            medicalUICallback.addCustomQuery(CameraPosition.CustomEditQueries.CaptureCameraPosition, captureCameraPosition);
            medicalUICallback.addCustomQuery(ChangeMedicalStateDoAction.CustomEditQueries.CapturePresetState, capturePresetState);
            medicalUICallback.addCustomQuery(RmlView.CustomQueries.OpenFileInRmlViewer, openFileInRmlViewer);
            medicalUICallback.addCustomQuery(RmlView.CustomQueries.EditWithSystemEditor, openSystemEditor);
            medicalUICallback.addCustomQuery(TimelineEditInterface.CustomQueries.OpenFolder, openTimelineFolder);
            medicalUICallback.addCustomQuery(AnomalousMvcContext.CustomQueries.Preview, previewMvcContext);
            medicalUICallback.addCustomQuery(ViewCollection.CustomQueries.ShowViewBrowser, showViewBrowser);
            medicalUICallback.addCustomQuery(ModelCollection.CustomQueries.ShowModelBrowser, showModelBrowser);
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

        private void capturePresetState(SendResult<Object> resultCallback, params Object[] args)
        {
            PresetStateCaptureDialog stateCaptureDialog = new PresetStateCaptureDialog(resultCallback);
            stateCaptureDialog.SmoothShow = true;
            stateCaptureDialog.open(true);
        }

        private void openFileInRmlViewer(SendResult<Object> resultCallback, params Object[] args)
        {
            RmlViewer rmlViewer = propertiesController.EditorPlugin.RmlViewer;
            String file = propertiesController.ResourceProvider.getFullFilePath(args[0].ToString());
            rmlViewer.changeDocument(file);
            if (!rmlViewer.Visible)
            {
                rmlViewer.Visible = true;
            }
        }

        private void openTimelineFolder(SendResult<Object> resultCallback, params Object[] args)
        {
            if (args[0] != null)
            {
                OtherProcessManager.openLocalURL(Path.GetDirectoryName(propertiesController.ResourceProvider.getFullFilePath(args[0].ToString())));
            }
            else
            {
                OtherProcessManager.openLocalURL(propertiesController.ResourceProvider.getFullFilePath(""));
            }
        }

        private void previewMvcContext(SendResult<Object> resultCallback, params Object[] args)
        {
            if (args[0] != null)
            {
                if (propertiesController.ResourceProvider != null)
                {
                    standaloneController.TimelineController.setResourceProvider(propertiesController.ResourceProvider);
                    AnomalousMvcContext context = (AnomalousMvcContext)args[0];
                    context.setResourceProvider(propertiesController.ResourceProvider);
                    standaloneController.MvcCore.startRunningContext(context);
                }
                else
                {
                    MessageBox.show("Cannot run MVC Context. Please open a timeline project first.", "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
            }
        }

        private void openSystemEditor(SendResult<Object> resultCallback, params Object[] args)
        {
            if (args[0] != null)
            {
                OtherProcessManager.openLocalURL(propertiesController.ResourceProvider.getFullFilePath(args[0].ToString()));
            }
        }

        private void showViewBrowser(SendResult<Object> resultCallback, params Object[] args)
        {
            browserResultCallback = resultCallback;
            Browser browser = new Browser("Views");
            standaloneController.MvcCore.ViewHostFactory.createViewBrowser(browser);
            browserWindow.setBrowser(browser);
            browserWindow.ItemSelected += browserWindow_ItemSelected;
            browserWindow.Canceled += browserWindow_Canceled;
            browserWindow.open(true);
        }

        private void showModelBrowser(SendResult<Object> resultCallback, params Object[] args)
        {
            browserResultCallback = resultCallback;
            Browser browser = new Browser("Models");

            browser.addNode("", null, new BrowserNode("Navigation", new ReflectedModelCreationInfo(NavigationModel.DefaultName, typeof(NavigationModel))));

            browserWindow.setBrowser(browser);
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
