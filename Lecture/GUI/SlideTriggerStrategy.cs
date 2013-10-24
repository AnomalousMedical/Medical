using libRocketPlugin;
using Medical;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
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
            actionEditor = new EditInterfaceEditor(action.getEditInterface(), uiCallback, browserProvider);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, applyChanges, delete);
            editor.addElementEditor(textEditor);
            editor.addElementEditor(actionEditor);
            return editor;
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
