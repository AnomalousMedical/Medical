using Engine;
using libRocketPlugin;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    public class TextElementStrategy : ElementStrategy
    {
        public static String DecodeFromHtml(String rml)
        {
            if (rml != null)
            {
                return WebUtility.HtmlDecode(rml.Replace("<br />", "\n"));
            }
            return null;
        }

        public static String EncodeToHtml(String rml)
        {
            if (rml != null)
            {
                return WebUtility.HtmlEncode(rml).Replace("\n", "<br />");
            }
            return null;
        }

        private ElementTextEditor textEditor;
        private EditInterfaceEditor appearanceEditor;
        private TextElementStyle elementStyle;

        public TextElementStrategy(String tag, String previewIconName = "Editor/HeaderIcon")
            : base(tag, previewIconName, true)
        {
            ResizeHandles = ResizeType.Top | ResizeType.Height;
        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            elementStyle = new TextElementStyle(element);
            elementStyle.Changed += elementStyle_Changed;
            String rml = DecodeFromHtml(element.InnerRml);
            textEditor = new ElementTextEditor(rml);
            appearanceEditor = new EditInterfaceEditor("Appearance", elementStyle.getEditInterface(), uiCallback, browserProvider);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(textEditor);
            editor.addElementEditor(appearanceEditor);
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            build(element);
            return true;
        }

        public override HighlightProvider HighlightProvider
        {
            get
            {
                return elementStyle;
            }
        }

        public override void changeSizePreview(Element element, IntRect newRect, ResizeType resizeType, IntSize2 bounds)
        {
            elementStyle.changeSize(newRect, resizeType, bounds);
            build(element);
        }

        public override Rect getStartingRect(Element selectedElement, out bool leftAnchor)
        {
            return elementStyle.createCurrentRect(selectedElement, out leftAnchor);
        }

        public override void applySizeChange(Element element)
        {
            appearanceEditor.alertChangesMade();
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

        private void build(Element element)
        {
            element.ClearLocalStyles();
            String text = textEditor.Text;
            element.InnerRml = EncodeToHtml(text);

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
            elementStyle.buildClassList(classes);
            if (classes.Length > 0)
            {
                element.SetAttribute("class", classes.ToString());
            }
            else
            {
                element.RemoveAttribute("class");
            }
        }

        void elementStyle_Changed(StyleDefinition obj)
        {
            appearanceEditor.alertChangesMade();
        }
    }
}
