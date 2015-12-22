using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;
using Engine.Editing;
using Engine.Saving;
using libRocketPlugin;
using Medical;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using Medical.GUI.RmlWysiwyg.Elements;
using Medical.SlideshowActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture.GUI
{
    class SlideInputStrategy : ElementStrategy
    {
        private EditInterfaceEditor editInterfaceEditor;
        private Slide slide;
        private SlideAction currentAction;
        private RunCommandsAction previewTriggerAction;
        private UndoRedoBuffer undoBuffer;
        private NotificationGUIManager notificationManager;

        public event Action PreviewTrigger;

        public SlideInputStrategy(Slide slide, UndoRedoBuffer undoBuffer, NotificationGUIManager notificationManager, RunCommandsAction previewTriggerAction, String tag, String previewIconName = CommonResources.NoIcon)
            : base(tag, previewIconName)
        {
            this.slide = slide;
            this.undoBuffer = undoBuffer;
            this.notificationManager = notificationManager;
            this.previewTriggerAction = previewTriggerAction;
        }


        public override RmlElementEditor openEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            switch (element.GetAttributeString("type"))
            {
                case "range":
                    return openRangeEditor(element, uiCallback, left, top);
                default:
                    return null;
            }
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component, out TwoWayCommand additionalUndoOperations)
        {
            additionalUndoOperations = null;
            switch (element.GetAttributeString("type"))
            {
                case "range":
                    return applyRangeChanges(element, editor, component);
                default:
                    return false;
            }
        }

        private RmlElementEditor openRangeEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            //Find actions
            String actionName = element.GetAttribute("onchange").StringValue;
            if (String.IsNullOrEmpty(actionName))
            {
                actionName = Guid.NewGuid().ToString();
                element.SetAttribute("onclick", actionName);
            }
            SlideAction action = slide.getAction(actionName);
            if (action == null)
            {
                action = new BlendSceneAction(actionName);
                slide.addAction(action);
            }

            //Make copy of action, this is really important, a lot of the function of this editor assumes this
            //is copied.
            SlideAction editingAction = CopySaver.Default.copy(action);

            EditInterface editInterface = setupEditInterface(editingAction, slide);
            editInterfaceEditor = new EditInterfaceEditor("Blend", editInterface, uiCallback);
            //appearanceEditor = new EditInterfaceEditor("Appearance", elementStyle.getEditInterface(), uiCallback);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(editInterfaceEditor);
            return editor;
        }

        private bool applyRangeChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            return true;
        }

        private EditInterface setupEditInterface(SlideAction editingAction, Slide slide)
        {
            currentAction = editingAction;
            EditInterface editInterface = editingAction.getEditInterface();
            editingAction.ChangesMade += EditingAction_ChangesMade;
            //editInterface.addCommand(new EditInterfaceCommand("Change Type", callback =>
            //{
            //    callback.showBrowser<Func<String, SlideAction>>(actionTypeBrowser, delegate (Func<String, SlideAction> result, ref string errorPrompt)
            //    {
            //        currentAction.ChangesMade -= editingAction_ChangesMade;
            //        SlideAction newAction = result(currentAction.Name);
            //        newAction.ChangesMade += editingAction_ChangesMade;
            //        actionEditor.EditInterface = setupEditInterface(newAction, slide);
            //        editingAction_ChangesMade(newAction);
            //        errorPrompt = "";
            //        return true;
            //    });
            //}));
            if (editingAction.AllowPreview)
            {
                editInterface.addCommand(new EditInterfaceCommand("Preview Start", callback =>
                {
                    previewTriggerAction.clear();
                    currentAction.setupAction(slide, previewTriggerAction);
                    if (PreviewTrigger != null)
                    {
                        PreviewTrigger.Invoke();
                    }
                }));
            }
            return editInterface;
        }

        private void EditingAction_ChangesMade(SlideAction obj)
        {
            undoBuffer.pushAndExecute(new TwoWayDelegateCommand<SlideAction, SlideAction>(CopySaver.Default.copy(currentAction), slide.getAction(currentAction.Name),
                new TwoWayDelegateCommand<SlideAction, SlideAction>.Funcs()
                {
                    ExecuteFunc = (exec) =>
                    {
                        notificationManager.showNotification("Changed slider action.", PreviewIconName, 3);
                        slide.replaceAction(exec);
                    },
                    UndoFunc = (undo) =>
                    {
                        notificationManager.showNotification("Changed slider action.", PreviewIconName, 3);
                        slide.replaceAction(undo);
                    },
                }));
        }
    }
}
