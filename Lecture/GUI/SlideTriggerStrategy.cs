using Engine.Editing;
using libRocketPlugin;
using Medical;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
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

        /// <summary>
        /// Create a slide trigger strategy. The ActionTypeBrowser determines the slide action types that can be put on the slide.
        /// Be sure to set the DefaultSelection on this browser, this is used when the trigger has no action as the default.
        /// </summary>
        public SlideTriggerStrategy(Slide slide, Browser actionTypeBrowser, String tag, String previewIconName = CommonResources.NoIcon)
            : base(tag, previewIconName, true)
        {
            this.slide = slide;
            this.actionTypeBrowser = actionTypeBrowser;
        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            String rml = element.InnerRml;
            if (rml != null)
            {
                rml = rml.Replace("<br />", "\n");
            }
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

            EditInterface editInterface = setupEditInterface(action, slide);
            actionEditor = new EditInterfaceEditor("Action", editInterface, uiCallback, browserProvider);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, applyChanges, delete);
            editor.addElementEditor(textEditor);
            editor.addElementEditor(actionEditor);
            return editor;
        }

        private EditInterface setupEditInterface(SlideAction action, Slide slide)
        {
            String name = action.Name;
            EditInterface editInterface = action.getEditInterface();
            editInterface.addCommand(new EditInterfaceCommand("Change Type", (callback, caller) =>
            {
                callback.showBrowser<Func<String, SlideAction>>(actionTypeBrowser, delegate(Func<String, SlideAction> result, ref string errorPrompt)
                {
                    SlideAction newAction = result(name);
                    actionEditor.EditInterface = setupEditInterface(newAction, slide);
                    slide.replaceAction(newAction);
                    errorPrompt = "";
                    return true;
                });
            }));
            return editInterface;
        }

        private bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String text = textEditor.Text;
            element.InnerRml = text.Replace("\n", "<br />");
            return true;
        }

        private bool delete(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
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
