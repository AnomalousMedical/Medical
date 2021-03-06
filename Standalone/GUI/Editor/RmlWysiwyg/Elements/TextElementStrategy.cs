﻿using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;
using Engine;
using libRocketPlugin;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    public class TextElementStrategy : ElementStrategy
    {
        public static String DecodeFromHtml(String rml)
        {
            if (rml != null)
            {
                //Rml only needs encoding for the following
                StringBuilder sbRml = new StringBuilder(rml);
                sbRml.Replace("<br />", "\n");
                //sbRml.Replace(@"\u00A0", "&nbsp;"); //Don't worry about nbsp
                sbRml.Replace("&gt;", ">");
                sbRml.Replace("&lt;", "<");
                sbRml.Replace("&amp;", "&");
                return sbRml.ToString();
            }
            return null;
        }

        public static String EncodeToHtml(String rml)
        {
            if (rml != null)
            {
                //Rml only needs encoding for the following
                StringBuilder sbRml = new StringBuilder(rml);
                sbRml.Replace("&", "&amp;");
                sbRml.Replace("<", "&lt;");
                sbRml.Replace(">", "&gt;");
                //sbRml.Replace(@"\u00A0", "&nbsp;"); //Don't worry about nbsp
                sbRml.Replace("\n", "<br />");
                return sbRml.ToString();
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

        public override RmlElementEditor openEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            elementStyle = new TextElementStyle(element, true);
            elementStyle.Changed += elementStyle_Changed;
            String rml = DecodeFromHtml(element.InnerRml);
            textEditor = new ElementTextEditor(rml);
            appearanceEditor = new EditInterfaceEditor("Appearance", elementStyle.getEditInterface(), uiCallback);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(textEditor);
            editor.addElementEditor(appearanceEditor);
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component, out TwoWayCommand additionalUndoOperations)
        {
            additionalUndoOperations = null;
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
            element.ClassNames = classes.ToString();
        }

        void elementStyle_Changed(StyleDefinition obj)
        {
            appearanceEditor.alertChangesMade();
        }
    }
}
