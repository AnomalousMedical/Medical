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
using Medical.GUI;
using Engine;

namespace Medical
{
    class EditorUICallbackExtensions
    {
        private MedicalUICallback medicalUICallback;
        private StandaloneController standaloneController;
        private EditorController editorController;

        public EditorUICallbackExtensions(StandaloneController standaloneController, MedicalUICallback medicalUICallback, EditorController editorController)
        {
            this.medicalUICallback = medicalUICallback;
            this.editorController = editorController;
            this.standaloneController = standaloneController;

            medicalUICallback.addCustomQuery(CameraPosition.CustomEditQueries.CaptureCameraPosition, captureCameraPosition);
            medicalUICallback.addCustomQuery(CameraPosition.CustomEditQueries.PreviewCameraPosition, previewCameraPosition);
            medicalUICallback.addCustomQuery(ChangeMedicalStateCommand.CustomEditQueries.CapturePresetState, capturePresetState);
            medicalUICallback.addCustomQuery(RmlView.CustomQueries.OpenFileInRmlViewer, openFileInRmlViewer);
            medicalUICallback.addCustomQuery(RmlView.CustomQueries.EditWithSystemEditor, openSystemEditor);
            medicalUICallback.addCustomQuery(TimelineEditInterface.CustomQueries.OpenFolder, openTimelineFolder);
            medicalUICallback.addCustomQuery(AnomalousMvcContext.CustomQueries.Preview, previewMvcContext);
            medicalUICallback.addCustomQuery(ViewCollection.CustomQueries.ShowViewBrowser, showViewBrowser);
            medicalUICallback.addCustomQuery(ModelCollection.CustomQueries.ShowModelBrowser, showModelBrowser);
            medicalUICallback.addCustomQuery(RunCommandsAction.CustomQueries.ShowCommandBrowser, showCommandBrowser);
        }

        private void captureCameraPosition(SendResult<Object> resultCallback, params Object[] args)
        {
            CameraPosition camPos = (CameraPosition)args[0];
            SceneViewWindow activeWindow = standaloneController.SceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                camPos.Translation = activeWindow.Translation;
                camPos.LookAt = activeWindow.LookAt;
                camPos.calculateIncludePoint(activeWindow);
            }
        }

        private void previewCameraPosition(SendResult<Object> resultCallback, params Object[] args)
        {
            CameraPosition camPos = (CameraPosition)args[0];
            SceneViewWindow activeWindow = standaloneController.SceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                activeWindow.setPosition(camPos.computeTranslationWithIncludePoint(activeWindow), camPos.LookAt);
            }
        }

        private void capturePresetState(SendResult<Object> resultCallback, params Object[] args)
        {
            PresetStateCaptureDialog stateCaptureDialog = new PresetStateCaptureDialog(resultCallback);
            stateCaptureDialog.SmoothShow = true;
            stateCaptureDialog.open(true);
        }

        private void openFileInRmlViewer(SendResult<Object> resultCallback, params Object[] args)
        {
            editorController.openFile(args[0].ToString());
        }

        private void openTimelineFolder(SendResult<Object> resultCallback, params Object[] args)
        {
            if (args[0] != null)
            {
                editorController.openFile(editorController.ResourceProvider.getFullFilePath(args[0].ToString()));
            }
            else
            {
                editorController.openFile(editorController.ResourceProvider.getFullFilePath(""));
            }
        }

        private void previewMvcContext(SendResult<Object> resultCallback, params Object[] args)
        {
            if (args[0] != null)
            {
                if (editorController.ResourceProvider != null)
                {
                    standaloneController.TimelineController.setResourceProvider(editorController.ResourceProvider);
                    AnomalousMvcContext context = (AnomalousMvcContext)args[0];
                    context.setResourceProvider(editorController.ResourceProvider);
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
                OtherProcessManager.openLocalURL(editorController.ResourceProvider.getFullFilePath(args[0].ToString()));
            }
        }

        private void showViewBrowser(SendResult<Object> resultCallback, params Object[] args)
        {
            Browser browser = new Browser("Views");
            standaloneController.MvcCore.ViewHostFactory.createViewBrowser(browser);
            editorController.showBrowser(browser, resultCallback);
        }

        private void showModelBrowser(SendResult<Object> resultCallback, params Object[] args)
        {
            Browser browser = new Browser("Models");

            browser.addNode("", null, new BrowserNode("Navigation", new ReflectedModelCreationInfo(NavigationModel.DefaultName, typeof(NavigationModel))));
            browser.addNode("", null, new BrowserNode("MedicalStateInfo", new ReflectedModelCreationInfo(MedicalStateInfoModel.DefaultName, typeof(MedicalStateInfoModel))));

            editorController.showBrowser(browser, resultCallback);
        }

        private void showCommandBrowser(SendResult<Object> resultCallback, params Object[] args)
        {
            editorController.showBrowser(RunCommandsAction.CreateCommandBrowser(), resultCallback);
        }
    }
}
