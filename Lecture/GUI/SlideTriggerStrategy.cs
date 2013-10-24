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

        public SlideTriggerStrategy(Slide slide, String tag, String previewIconName = CommonResources.NoIcon)
            : base(tag, previewIconName, true)
        {
            this.slide = slide;
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
                action = new SetupSceneAction(actionName);
                slide.addAction(action);
            }

            EditInterface editInterface = setupEditInterface(action, slide);
            actionEditor = new EditInterfaceEditor(editInterface, uiCallback, browserProvider);
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
                callback.showBrowser<Func<String, SlideAction>>(actionTypeBrowser(), delegate(Func<String, SlideAction> result, ref string errorPrompt)
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

        private Browser actionTypeBrowser()
        {
            Browser browser = new Browser("Types", "Choose Trigger Type");
            BrowserNode rootNode = browser.getTopNode();
            rootNode.addChild(new BrowserNode("Play Timeline", new Func<String, SlideAction>((name) =>
            {
                return new PlayTimelineAction(name);
            })));

            rootNode.addChild(new BrowserNode("Setup Scene", new Func<String, SlideAction>((name) =>
            {
                return new SetupSceneAction(name);
            })));

            return browser;
        }
    }
}
