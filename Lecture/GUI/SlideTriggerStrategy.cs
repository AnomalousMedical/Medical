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

namespace Lecture.GUI
{
    class SlideTriggerStrategy : ElementStrategy
    {
        private ElementTextEditor textEditor;
        private EditInterfaceEditor actionEditor;
        private Slide slide;
        private Browser actionTypeBrowser;
        private SlideAction currentAction;
        private UndoRedoBuffer undoBuffer;
        private EditInterfaceEditor appearanceEditor;
        private TextElementStyle elementStyle;

        /// <summary>
        /// Create a slide trigger strategy. The ActionTypeBrowser determines the slide action types that can be put on the slide.
        /// Be sure to set the DefaultSelection on this browser, this is used when the trigger has no action as the default.
        /// </summary>
        public SlideTriggerStrategy(Slide slide, Browser actionTypeBrowser, UndoRedoBuffer undoBuffer, String tag, String previewIconName = CommonResources.NoIcon)
            : base(tag, previewIconName, true)
        {
            this.undoBuffer = undoBuffer;
            this.slide = slide;
            this.actionTypeBrowser = actionTypeBrowser;
        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            elementStyle = new TextElementStyle(element);
            elementStyle.Changed += elementStyle_Changed;
            String rml = TextElementStrategy.DecodeFromHtml(element.InnerRml);
            textEditor = new ElementTextEditor(rml);
            String actionName = element.GetAttribute("onclick").StringValue;
            if (String.IsNullOrEmpty(actionName))
            {
                actionName = Guid.NewGuid().ToString();
                element.SetAttribute("onclick", actionName);
            }
            SlideAction action = slide.getAction(actionName);
            if (action == null)
            {
                action = ((Func<String, SlideAction>)actionTypeBrowser.DefaultSelection.Value)(actionName);
                slide.addAction(action);
            }

            //Make copy of action, this is really important, a lot of the function of this editor assumes this
            //is copied.
            SlideAction editingAction = CopySaver.Default.copy(action);

            EditInterface editInterface = setupEditInterface(editingAction, slide);
            actionEditor = new EditInterfaceEditor("Action", editInterface, uiCallback, browserProvider);
            appearanceEditor = new EditInterfaceEditor("Appearance", elementStyle.getEditInterface(), uiCallback, browserProvider);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(textEditor);
            editor.addElementEditor(actionEditor);
            editor.addElementEditor(appearanceEditor);
            return editor;
        }

        private EditInterface setupEditInterface(SlideAction editingAction, Slide slide)
        {
            currentAction = editingAction;
            EditInterface editInterface = editingAction.getEditInterface();
            editingAction.ChangesMade += editingAction_ChangesMade;
            editInterface.addCommand(new EditInterfaceCommand("Change Type", (callback, caller) =>
            {
                callback.showBrowser<Func<String, SlideAction>>(actionTypeBrowser, delegate(Func<String, SlideAction> result, ref string errorPrompt)
                {
                    currentAction.ChangesMade -= editingAction_ChangesMade;
                    SlideAction newAction = result(currentAction.Name);
                    newAction.ChangesMade += editingAction_ChangesMade;
                    actionEditor.EditInterface = setupEditInterface(newAction, slide);
                    actionEditor.alertChangesMade();
                    errorPrompt = "";
                    return true;
                });
            }));
            return editInterface;
        }

        void editingAction_ChangesMade(SlideAction obj)
        {
            actionEditor.alertChangesMade();
        }

        void elementStyle_Changed(ElementStyleDefinition obj)
        {
            appearanceEditor.alertChangesMade();
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String text = textEditor.Text;
            element.InnerRml = TextElementStrategy.EncodeToHtml(text);

            StringBuilder style = new StringBuilder();
            elementStyle.buildStyleAttribute(style);
            if (style.Length > 0)
            {
                element.SetAttribute("style", style.ToString());
            }
            else
            {
                element.RemoveAttribute("style");
            }

            StringBuilder classes = new StringBuilder();
            classes.Append("TriggerLink ");
            elementStyle.buildClassList(classes);
            if (classes.Length > 0)
            {
                element.SetAttribute("class", classes.ToString());
            }
            else
            {
                element.RemoveAttribute("class");
            }

            undoBuffer.pushAndExecute(new TwoWayDelegateCommand<SlideAction, SlideAction>(
                (exec) =>
                    {
                        slide.replaceAction(exec);
                    },
                    CopySaver.Default.copy(currentAction),
                (undo) =>
                    {
                        slide.replaceAction(undo);
                    },
                    slide.getAction(currentAction.Name)));
            return true;
        }

        public override bool delete(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String text = element.InnerRml;
            if (String.IsNullOrEmpty(text))
            {
                component.deleteElement(element);
                return true;
            }
            return false;
        }
    }
}
