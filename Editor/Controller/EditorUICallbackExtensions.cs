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

            medicalUICallback.addOneWayCustomQuery(CameraPosition.CustomEditQueries.CaptureCameraPosition, delegate(CameraPosition camPos)
            {
                SceneViewWindow activeWindow = standaloneController.SceneViewController.ActiveWindow;
                if (activeWindow != null)
                {
                    camPos.Translation = activeWindow.Translation;
                    camPos.LookAt = activeWindow.LookAt;
                    camPos.calculateIncludePoint(activeWindow);
                }
            });

            medicalUICallback.addOneWayCustomQuery(CameraPosition.CustomEditQueries.PreviewCameraPosition, delegate(CameraPosition camPos)
            {
                SceneViewWindow activeWindow = standaloneController.SceneViewController.ActiveWindow;
                if (activeWindow != null)
                {
                    activeWindow.setPosition(camPos.computeTranslationWithIncludePoint(activeWindow), camPos.LookAt);
                }
            });

            medicalUICallback.addCustomQuery<CompoundPresetState>(ChangeMedicalStateCommand.CustomEditQueries.CapturePresetState, delegate(SendResult<CompoundPresetState> resultCallback)
            {
                PresetStateCaptureDialog stateCaptureDialog = new PresetStateCaptureDialog(resultCallback);
                stateCaptureDialog.SmoothShow = true;
                stateCaptureDialog.open(true);
            });

            medicalUICallback.addOneWayCustomQuery(RmlView.CustomQueries.OpenFileInRmlViewer, delegate(String file)
            {
                editorController.openFile(file);
            });

            medicalUICallback.addOneWayCustomQuery<String>(RmlView.CustomQueries.EditWithSystemEditor, delegate(String path)
            {
                OtherProcessManager.openLocalURL(editorController.ResourceProvider.getFullFilePath(path));
            });

            medicalUICallback.addOneWayCustomQuery(AnomalousMvcContext.CustomQueries.Preview, delegate(AnomalousMvcContext context)
            {
                if (editorController.ResourceProvider != null)
                {
                    standaloneController.TimelineController.setResourceProvider(editorController.ResourceProvider);
                    context.setResourceProvider(editorController.ResourceProvider);
                    standaloneController.MvcCore.startRunningContext(context);
                }
                else
                {
                    MessageBox.show("Cannot run MVC Context. Please open a timeline project first.", "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
            });

            medicalUICallback.addCustomQuery<Browser>(ViewCollection.CustomQueries.CreateViewBrowser, delegate(SendResult<Browser> resultCallback)
            {
                Browser browser = new Browser("Views", "Choose View Type");
                standaloneController.MvcCore.ViewHostFactory.createViewBrowser(browser);
                String errorPrompt = null;
                resultCallback(browser, ref errorPrompt);
            });

            medicalUICallback.addCustomQuery<Browser>(ModelCollection.CustomQueries.CreateModelBrowser, delegate(SendResult<Browser> resultCallback)
            {
                Browser browser = new Browser("Models", "Choose Model Type");

                browser.addNode("", null, new BrowserNode("Navigation", typeof(NavigationModel), NavigationModel.DefaultName));
                browser.addNode("", null, new BrowserNode("MedicalStateInfo", typeof(MedicalStateInfoModel), MedicalStateInfoModel.DefaultName));
                String error = null;
                resultCallback(browser, ref error);
            });
            
            medicalUICallback.addCustomQuery<Type>(RunCommandsAction.CustomQueries.ShowCommandBrowser, delegate(SendResult<Type> resultCallback)
            {
                medicalUICallback.showBrowser(RunCommandsAction.CreateCommandBrowser(), resultCallback);
            });
        }
    }
}
