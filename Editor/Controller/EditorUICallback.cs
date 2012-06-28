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
using Medical.Editor;
using Engine.Saving;
using Logging;

namespace Medical
{
    public class EditorUICallback : MedicalUICallback
    {
        private StandaloneController standaloneController;
        private EditorController editorController;
        private CopySaver copySaver = new CopySaver();
        private PropEditController propEditController;

        public EditorUICallback(StandaloneController standaloneController, EditorController editorController, PropEditController propEditController)
        {
            this.editorController = editorController;
            this.standaloneController = standaloneController;
            this.propEditController = propEditController;

            this.addOneWayCustomQuery(CameraPosition.CustomEditQueries.CaptureCameraPosition, delegate(CameraPosition camPos)
            {
                SceneViewWindow activeWindow = standaloneController.SceneViewController.ActiveWindow;
                if (activeWindow != null)
                {
                    camPos.Translation = activeWindow.Translation;
                    camPos.LookAt = activeWindow.LookAt;
                    camPos.calculateIncludePoint(activeWindow);
                }
            });

            this.addOneWayCustomQuery(CameraPosition.CustomEditQueries.PreviewCameraPosition, delegate(CameraPosition camPos)
            {
                SceneViewWindow activeWindow = standaloneController.SceneViewController.ActiveWindow;
                if (activeWindow != null)
                {
                    activeWindow.setPosition(camPos.computeTranslationWithIncludePoint(activeWindow), camPos.LookAt);
                }
            });

            this.addCustomQuery<CompoundPresetState>(ChangeMedicalStateCommand.CustomEditQueries.CapturePresetState, delegate(SendResult<CompoundPresetState> resultCallback)
            {
                PresetStateCaptureDialog stateCaptureDialog = new PresetStateCaptureDialog(resultCallback);
                stateCaptureDialog.SmoothShow = true;
                stateCaptureDialog.open(true);
            });

            this.addOneWayCustomQuery(RmlView.CustomQueries.OpenFileInRmlViewer, delegate(String file)
            {
                editorController.openFile(file);
            });

            this.addOneWayCustomQuery(AnomalousMvcContext.CustomQueries.Preview, delegate(AnomalousMvcContext context)
            {
                if (editorController.ResourceProvider != null)
                {
                    AnomalousMvcContext copiedContext = copySaver.copy<AnomalousMvcContext>(context);
                    standaloneController.TimelineController.setResourceProvider(editorController.ResourceProvider);
                    copiedContext.setResourceProvider(editorController.ResourceProvider);
                    copiedContext.RuntimeName = "Editor.PreviewMvcContext";
                    standaloneController.MvcCore.startRunningContext(copiedContext);
                }
                else
                {
                    MessageBox.show("Cannot run MVC Context. Please open a timeline project first.", "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
            });

            this.addCustomQuery<Browser>(ViewCollection.CustomQueries.CreateViewBrowser, delegate(SendResult<Browser> resultCallback)
            {
                Browser browser = new Browser("Views", "Choose View Type");
                standaloneController.MvcCore.ViewHostFactory.createViewBrowser(browser);
                String errorPrompt = null;
                resultCallback(browser, ref errorPrompt);
            });

            this.addCustomQuery<Browser>(ModelCollection.CustomQueries.CreateModelBrowser, delegate(SendResult<Browser> resultCallback)
            {
                Browser browser = new Browser("Models", "Choose Model Type");

                browser.addNode("", null, new BrowserNode("Navigation", typeof(NavigationModel), NavigationModel.DefaultName));
                browser.addNode("", null, new BrowserNode("MedicalStateInfo", typeof(MedicalStateInfoModel), MedicalStateInfoModel.DefaultName));
                String error = null;
                resultCallback(browser, ref error);
            });

            this.addCustomQuery<Type>(RunCommandsAction.CustomQueries.ShowCommandBrowser, delegate(SendResult<Type> resultCallback)
            {
                this.showBrowser(RunCommandsAction.CreateCommandBrowser(), resultCallback);
            });

            this.addOneWayCustomQuery(View.CustomQueries.AddControllerForView, delegate(View view)
            {
                AnomalousMvcContext context = BrowserWindowController.getCurrentEditingMvcContext();
                String controllerName = view.Name;
                if (context.Controllers.hasItem(controllerName))
                {
                    MessageBox.show(String.Format("There is already a controller named {0}. Cannot create a new one.", controllerName), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
                else
                {
                    MvcController controller = new MvcController(controllerName);
                    RunCommandsAction showCommand = new RunCommandsAction("Show");
                    showCommand.addCommand(new ShowViewCommand(view.Name));
                    controller.Actions.add(showCommand);

                    RunCommandsAction closeCommand = new RunCommandsAction("Close");
                    closeCommand.addCommand(new CloseViewCommand());
                    controller.Actions.add(closeCommand);
                    context.Controllers.add(controller);
                }
            });

            this.addCustomQuery<Color>(ShowTextAction.CustomQueries.ChooseColor, queryDelegate =>
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    if (colorDialog.showModal() == NativeDialogResult.OK)
                    {
                        String errorPrompt = null;
                        queryDelegate.Invoke(colorDialog.Color, ref errorPrompt);
                    }
                }
            });

            this.addOneWayCustomQuery<ShowPropAction>(ShowPropAction.CustomQueries.KeepOpenToggle, showPropAction =>
            {
                if (showPropAction.KeepOpen)
                {
                    propEditController.removeOpenProp(showPropAction);
                }
                else
                {
                    propEditController.addOpenProp(showPropAction);
                }
            });

            this.addSyncCustomQuery<Browser, String, String>(FileBrowserEditableProperty.CustomQueries.BuildBrowser, (searchPattern, prompt) =>
            {
                return createFileBrowser(searchPattern, prompt);
            });
        }

        public Browser createFileBrowser(String searchPattern, String prompt)
        {
            Browser browser = new Browser("Files", prompt);
            if (editorController.ResourceProvider != null)
            {
                foreach (String timeline in editorController.ResourceProvider.listFiles(searchPattern, "", true))
                {
                    browser.addNode("", null, new BrowserNode(timeline, timeline));
                }
            }
            else
            {
                Log.Warning("No resources loaded.");
            }
            return browser;
        }
    }
}
