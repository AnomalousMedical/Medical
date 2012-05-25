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
using Medical.Editor;

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
            medicalUICallback.addCustomQuery(ShowTimelineGUIAction.CustomEditQueries.ConvertToMvc, convertTimelineGuiToMvc);
            medicalUICallback.addCustomQuery(ShowPromptAction.CustomEditQueries.OpenQuestionEditor, openQuestionEditor);
            medicalUICallback.addCustomQuery(ShowPromptAction.CustomEditQueries.ConvertToMvc, convertToMvc);
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

        private void convertTimelineGuiToMvc(SendResult<Object> resultCallback, params Object[] args)
        {
            StringBuilder rml = new StringBuilder();
            rml.Append(timelineGuiRmlHead);
            AnomalousMvcContext context = BrowserWindowController.getCurrentEditingMvcContext();
            ResourceProvider resourceProvider = BrowserWindowController.getResourceProvider();
            ShowTimelineGUIAction showGuiAction = (ShowTimelineGUIAction)args[0];

            String controllerName = Path.GetFileNameWithoutExtension(showGuiAction.Timeline.LEGACY_SourceFile);
            String viewName = Path.GetFileNameWithoutExtension(showGuiAction.Timeline.LEGACY_SourceFile);

            RmlView rmlView = new RmlView(viewName);
            rmlView.RmlFile = viewName + ".rml";
            context.Views.add(rmlView);

            MvcController controller = new MvcController(controllerName);
            //RunCommandsAction playTimeline = new RunCommandsAction("PlayTimeline");
            //playTimeline.addCommand(new CloseViewCommand());
            //playTimeline.addCommand(new PlayTimelineCommand(showPromptAction.Timeline.LEGACY_SourceFile));
            //controller.Actions.add(playTimeline);
            RunCommandsAction showView = new RunCommandsAction("Show");
            showView.addCommand(new ShowViewCommand(viewName));
            //Play the timeline that the index belongs to
            showView.addCommand(new PlayTimelineCommand(Path.GetFileName(showGuiAction.Timeline.LEGACY_SourceFile)));
            controller.Actions.add(showView);

            context.Controllers.add(controller);

            //Build common if needed, this is considered the index timeline if this is built to help with last couple conversions.
            if(!context.Controllers.hasItem("Common"))
            {
                MvcController commonController = new MvcController("Common");
                RunCommandsAction start = new RunCommandsAction("Start");
                start.addCommand(new HideMainInterfaceCommand());
                start.addCommand(new SaveCameraPositionCommand());
                start.addCommand(new SaveLayersCommand());
                start.addCommand(new SaveMedicalStateCommand());
                start.addCommand(new SaveMusclePositionCommand());
                start.addCommand(new RunActionCommand(String.Format("{0}/Show", controllerName)));
                commonController.Actions.add(start);
                RunCommandsAction shutdown = new RunCommandsAction("Shutdown");
                shutdown.addCommand(new ShowMainInterfaceCommand());
                shutdown.addCommand(new RestoreMedicalStateCommand());
                shutdown.addCommand(new RestoreMusclePositionCommand());
                shutdown.addCommand(new RestoreCameraPositionCommand());
                shutdown.addCommand(new RestoreLayersCommand());
                commonController.Actions.add(shutdown);
                context.Controllers.add(commonController);
                //Cheater way to return to index
                RunCommandsAction index = new RunCommandsAction("Index");
                index.addCommand(new RunActionCommand(String.Format("{0}/Show", controllerName)));
                commonController.Actions.add(index);
            }

            showGuiAction.convertToMvc(context, rml, controller, rmlView);

            rml.Append(timelineGuiRmlTail);
            using (StreamWriter saveStream = new StreamWriter(resourceProvider.openWriteStream(viewName + ".rml")))
            {
                saveStream.Write(rml.ToString());
            }

            showGuiAction.Timeline.removePreAction(showGuiAction);
        }

        private String timelineGuiRmlHead = @"<rml>
  <head>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.rkt.rcss""/>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.Anomalous.rcss""/>
    <link type=""text/rcss"" href=""App.rcss""/>
  </head>
  <body>
    <div class=""ScrollArea"">
        <ul>";

        private String timelineGuiRmlTail = @"
        </ul>
    </div>
  </body>
</rml>
";

        private void convertToMvc(SendResult<Object> resultCallback, params Object[] args)
        {
            //showQuestionEditorCallback = resultCallback;
            //questionEditor.clear();
            StringBuilder rml = new StringBuilder();
            rml.Append(rmlHead);
            AnomalousMvcContext context = BrowserWindowController.getCurrentEditingMvcContext();
            ResourceProvider resourceProvider = BrowserWindowController.getResourceProvider();
            ShowPromptAction showPromptAction = (ShowPromptAction)args[0];

            String controllerName = Path.GetFileNameWithoutExtension(showPromptAction.Timeline.LEGACY_SourceFile);
            String viewName = Path.GetFileNameWithoutExtension(showPromptAction.Timeline.LEGACY_SourceFile);
            
            String exampleControllerName = "Examples";
            MvcController exampleController;
            try
            {
                exampleController = context.Controllers[exampleControllerName];
            }
            catch(Exception)
            {
                exampleController = new MvcController(exampleControllerName);
                context.Controllers.add(exampleController);
            }

            bool writingExamples = false;
            foreach (PromptQuestion question in showPromptAction.Questions)
            {
                rml.AppendFormat(questionRml, question.Text);
                foreach (PromptAnswer answer in question.Answers)
                {
                    if (writingExamples)
                    {
                        if (answer.Text.IndexOf("- Hear Example", 0, StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            addMvcExample(rml, exampleControllerName, exampleController, answer);
                        }
                        else
                        {
                            writingExamples = false;
                            rml.Append(endExample);
                            addMvcAnswerLink(rml, answer);
                        }
                    }
                    else
                    {
                        if (answer.Text.IndexOf("- Hear Example", 0, StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            writingExamples = true;
                            rml.Append(startExample);
                            addMvcExample(rml, exampleControllerName, exampleController, answer);
                        }
                        else
                        {
                            addMvcAnswerLink(rml, answer);
                        }
                    }
                }
                //questionEditor.Question = question;
                break;
            }
            rml.Append(rmlTail);
            using (StreamWriter saveStream = new StreamWriter(resourceProvider.openWriteStream(viewName + ".rml")))
            {
                saveStream.Write(rml.ToString());
            }

            RmlView rmlView = new RmlView(viewName);
            rmlView.RmlFile = viewName + ".rml";
            context.Views.add(rmlView);

            MvcController controller = new MvcController(controllerName);
            RunCommandsAction playTimeline = new RunCommandsAction("PlayTimeline");
            playTimeline.addCommand(new CloseViewCommand());
            playTimeline.addCommand(new PlayTimelineCommand(showPromptAction.Timeline.LEGACY_SourceFile));
            controller.Actions.add(playTimeline);
            RunCommandsAction showView = new RunCommandsAction("Show");
            showView.addCommand(new ShowViewCommand(viewName));
            controller.Actions.add(showView);
            context.Controllers.add(controller);

            showPromptAction.Timeline.addPostAction(new RunMvcAction(String.Format("{0}/{1}", controllerName, "Show")));
            showPromptAction.Timeline.removePostAction(showPromptAction);
            //questionEditor.SoundFile = showPromptAction.SoundFile;
            //questionEditor.Closed += questionEditor_Closed;
            //questionEditor.open(true);
        }

        private void addMvcAnswerLink(StringBuilder rml, PromptAnswer answer)
        {
            String oldTimeline = Path.GetFileNameWithoutExtension(((PromptLoadTimelineAction)answer.Action).TargetTimeline);
            String action;
            if (oldTimeline != "END_ReturnToNormal")
            {
                action = String.Format("{0}/{1}", oldTimeline, "PlayTimeline");
            }
            else
            {
                action = "Common/StopDiagnosis";
            }
            rml.AppendFormat(answerRml, action, answer.Text);
        }

        private void addMvcExample(StringBuilder rml, String exampleControllerName, MvcController exampleController, PromptAnswer answer)
        {
            String exampleTimeline = ((PromptLoadTimelineAction)answer.Action).TargetTimeline;
            String exampleAction = Path.GetFileNameWithoutExtension(exampleTimeline);
            if (!exampleController.Actions.hasItem(exampleAction))
            {
                RunCommandsAction runCommands = new RunCommandsAction(exampleAction);
                runCommands.addCommand(new CloseViewCommand());
                runCommands.addCommand(new PlayTimelineCommand(exampleTimeline));
                exampleController.Actions.add(runCommands);
            }
            rml.AppendFormat(exampleElement, String.Format("{0}/{1}", exampleControllerName, exampleAction), answer.Text);
        }

        private String rmlHead = @"<rml>
  <head>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.rkt.rcss""/>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.Anomalous.rcss""/>
    <link type=""text/rcss"" href=""QuestionPrompt.rcss""/>
  </head>
  <body>
    <div class=""ScrollArea"">";

        private string questionRml = @"
        <h1>{0}</h1>
        <div class=""Separator""></div>
	    <div class=""AnswerArea"">
            <ul class=""AnswerList"">";

            private string answerRml = @"
                <li>
                    <a onclick='{0}'>{1}</a>
                </li>";

            private string startExample = @"
                    <ul class=""ExampleList"">";

            private string exampleElement = @"
                        <li>
                            <a onclick='{0}'>{1}</a>
                        </li>";

            private string endExample = @"
                    </ul>";

            private String rmlTail = @"
            </ul>
        </div>
    </div>
  </body>
</rml>
";

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
