using Engine.Editing;
using Logging;
using Medical.Controller.AnomalousMvc;
using Medical.Editor;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class EditorUICallback : CommonUICallback
    {
        public EditorUICallback(StandaloneController standaloneController, EditorController editorController, PropEditController propEditController)
            : base(standaloneController, editorController, propEditController)
        {
            this.addOneWayCustomQuery(AnomalousMvcContext.CustomQueries.Preview, delegate(AnomalousMvcContext context)
            {
                previewMvcContext(context);
            });

            this.addSyncCustomQuery<Browser>(ViewBrowserEditableProperty.CustomQueries.BuildBrowser, () =>
            {
                Browser browser = new Browser("Views", "Choose View");
                if (CurrentEditingMvcContext != null)
                {
                    foreach (View view in CurrentEditingMvcContext.Views)
                    {
                        browser.addNode(null, null, new BrowserNode(view.Name, view.Name));
                    }
                }
                return browser;
            });

            this.addSyncCustomQuery<Browser, Type>(ModelBrowserEditableProperty.CustomQueries.BuildBrowser, (assignableFromType) =>
            {
                Browser browser = new Browser("Models", "Choose Model");
                if (CurrentEditingMvcContext != null)
                {
                    foreach (MvcModel model in CurrentEditingMvcContext.Models)
                    {
                        if (assignableFromType.IsAssignableFrom(model.GetType()))
                        {
                            browser.addNode(null, null, new BrowserNode(model.Name, model.Name));
                        }
                    }
                }
                return browser;
            });

            this.addOneWayCustomQuery(View.CustomQueries.AddControllerForView, delegate(View view)
            {
                AnomalousMvcContext context = CurrentEditingMvcContext;
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

            this.addSyncCustomQuery<Browser>(ActionBrowserEditableProperty.CustomQueries.BuildBrowser, () =>
            {
                return createActionBrowser();
            });

            this.addSyncCustomQuery<Browser>(ElementAttributeEditor.CustomQueries.BuildActionBrowser, () =>
            {
                return createActionBrowser();
            });

            this.addSyncCustomQuery<Browser, IEnumerable<String>, String, String>(ElementAttributeEditor.CustomQueries.BuildFileBrowser, (searchPatterns, prompt, leadingPath) =>
            {
                return createFileBrowser(searchPatterns, prompt, leadingPath);
            });
        }

        public void previewMvcContext(AnomalousMvcContext context)
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
        }

        public override Browser createFileBrowser(IEnumerable<string> searchPatterns, string prompt)
        {
            Browser browser = new Browser("Files", prompt);
            if (editorController.ResourceProvider != null)
            {
                foreach (String searchPattern in searchPatterns)
                {
                    foreach (String file in editorController.ResourceProvider.listFiles(searchPattern, "", true))
                    {
                        browser.addNode(null, null, new BrowserNode(file, file));
                    }
                }
            }
            else
            {
                Log.Warning("No resources loaded.");
            }
            return browser;
        }

        public Browser createFileBrowser(IEnumerable<string> searchPatterns, String prompt, String leadingPath)
        {
            Browser browser = new Browser("Files", prompt);
            if (editorController.ResourceProvider != null)
            {
                foreach (String searchPattern in searchPatterns)
                {
                    foreach (String file in editorController.ResourceProvider.listFiles(searchPattern, "", true))
                    {
                        browser.addNode(null, null, new BrowserNode(file, Path.Combine(leadingPath, file)));
                    }
                }
            }
            else
            {
                Log.Warning("No resources loaded.");
            }
            return browser;
        }

        private Browser createActionBrowser()
        {
            Browser browser = new Browser("Action", "Choose Action");
            if (CurrentEditingMvcContext != null)
            {
                foreach (MvcController controller in CurrentEditingMvcContext.Controllers)
                {
                    BrowserNode controllerNode = new BrowserNode(controller.Name, null);
                    foreach (ControllerAction action in controller.Actions)
                    {
                        controllerNode.addChild(new BrowserNode(action.Name, String.Format("{0}/{1}", controller.Name, action.Name)));
                    }
                    browser.addNode(null, null, controllerNode);
                }
            }
            return browser;
        }

        public AnomalousMvcContext CurrentEditingMvcContext { get; set; }
    }
}
