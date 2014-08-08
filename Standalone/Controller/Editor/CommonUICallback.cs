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
    public abstract class CommonUICallback : MedicalUICallback
    {
        protected StandaloneController standaloneController;
        protected EditorController editorController;
        protected CopySaver copySaver = new CopySaver();
        protected PropEditController propEditController;

        public CommonUICallback(StandaloneController standaloneController, EditorController editorController, PropEditController propEditController)
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
                    activeWindow.calculateIncludePoint(camPos);
                }
            });

            this.addOneWayCustomQuery(CameraPosition.CustomEditQueries.PreviewCameraPosition, delegate(CameraPosition camPos)
            {
                SceneViewWindow activeWindow = standaloneController.SceneViewController.ActiveWindow;
                if (activeWindow != null)
                {
                    activeWindow.setPosition(camPos, MedicalConfig.CameraTransitionTime);
                }
            });

            this.addCustomQuery<PresetState>(ChangeMedicalStateCommand.CustomEditQueries.CapturePresetState, delegate(SendResult<PresetState> resultCallback)
            {
                PresetStateCaptureDialog stateCaptureDialog = new PresetStateCaptureDialog(resultCallback);
                stateCaptureDialog.SmoothShow = true;
                stateCaptureDialog.open(true);
            });

            this.addOneWayCustomQuery(RmlView.CustomQueries.OpenFileInRmlViewer, delegate(String file)
            {
                editorController.openEditor(file);
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

                browser.addNode(null, null, new BrowserNode("DataModel", typeof(DataModel), DataModel.DefaultName));
                browser.addNode(null, null, new BrowserNode("Navigation", typeof(NavigationModel), NavigationModel.DefaultName));
                browser.addNode(null, null, new BrowserNode("MedicalStateInfo", typeof(MedicalStateInfoModel), MedicalStateInfoModel.DefaultName));
                browser.addNode(null, null, new BrowserNode("BackStack", typeof(BackStackModel), BackStackModel.DefaultName));
                String error = null;
                resultCallback(browser, ref error);
            });

            this.addCustomQuery<Type>(RunCommandsAction.CustomQueries.ShowCommandBrowser, delegate(SendResult<Type> resultCallback)
            {
                this.showBrowser(RunCommandsAction.CreateCommandBrowser(), resultCallback);
            });

            this.addCustomQuery<Color>(ShowTextAction.CustomQueries.ChooseColor, queryDelegate =>
            {
                ColorDialog colorDialog = new ColorDialog();
                colorDialog.showModal((result, color) =>
                {
                    if (result == NativeDialogResult.OK)
                    {
                        String errorPrompt = null;
                        queryDelegate.Invoke(color, ref errorPrompt);
                    }
                });
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
            
            this.addSyncCustomQuery<Browser, IEnumerable<String>, String>(FileBrowserEditableProperty.CustomQueries.BuildBrowser, (searchPattern, prompt) =>
            {
                return createFileBrowser(searchPattern, prompt);
            });

            this.addSyncCustomQuery<Browser>(PropBrowserEditableProperty.CustomQueries.BuildBrowser, () =>
            {
                Browser browser = new Browser("Props", "Choose Prop");
                foreach (var propDef in standaloneController.TimelineController.PropFactory.PropDefinitions)
                {
                    browser.addNode(propDef.BrowserPath, new BrowserNode(propDef.PrettyName, propDef.Name));
                }
                return browser;
            });

            this.addSyncCustomQuery<Browser>(ElementNameEditableProperty.CustomQueries.BuildBrowser, () =>
            {
                Browser browser = new Browser("Screen Location Name", "Choose Screen Location Name");
                foreach (var elementName in standaloneController.GUIManager.NamedLinks)
                {
                    browser.addNode(null, null, new BrowserNode(elementName.UniqueDerivedName, elementName));
                }
                return browser;
            });

            addOneWayCustomQuery<String>(PlaySoundAction.CustomQueries.EditExternally, soundFile =>
            {
                if (soundFile != null && editorController.ResourceProvider.exists(soundFile))
                {
                    String fullPath = editorController.ResourceProvider.getFullFilePath(soundFile);
                    OtherProcessManager.openLocalURL(fullPath);
                }
            });
        }

        public override void showBrowser<T>(Browser browser, SendResult<T> resultCallback)
        {
            switch (browser.Hint)
            {
                case Browser.DisplayHint.Images:
                    ImageBrowserWindow<T>.GetInput(browser, true, resultCallback, editorController.ResourceProvider);
                    break;
                default:
                    base.showBrowser<T>(browser, resultCallback);
                    break;
            }   
        }

        public abstract Browser createFileBrowser(IEnumerable<string> searchPatterns, string prompt);
    }
}
