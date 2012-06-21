using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using libRocketPlugin;
using Medical.Editor;

namespace Medical.GUI
{
    class RmlElementEditor : PopupContainer
    {
        /// <summary>
        /// Open a text editor that disposes when it is closed.
        /// </summary>
        /// <returns></returns>
        public static RmlElementEditor openTextEditor(MedicalUICallback uiCallback, Element element, int left, int top)
        {
            RmlElementEditor textEditor = new RmlElementEditor(uiCallback, element);
            textEditor.show(left, top);
            textEditor.Hidden += (source, e) =>
            {
                ((RmlElementEditor)source).Dispose();
            };
            return textEditor;
        }

        private EditBox text;
        private ScrollView propertiesScroll;
        private ScrollablePropertiesForm propertiesForm;

        protected RmlElementEditor(MedicalUICallback uiCallback, Element element)
            :base("Medical.GUI.RmlWysiwyg.RmlElementEditor.layout")
        {
            text = (EditBox)widget.findWidget("Text");
            Text = element.InnerRml;

            propertiesScroll = (ScrollView)widget.findWidget("PropertiesScroll");
            propertiesForm = new ScrollablePropertiesForm(propertiesScroll, uiCallback);

            //Build edit interface
            EditInterface editInterface = new EditInterface(element.TagName);
            int index = 0;
            String name = null;
            String value = null;
            while (element.IterateAttributes(ref index, ref name, ref value))
            {
                switch (name.ToLowerInvariant())
                {
                    case "onclick":
                        editInterface.addEditableProperty(new RmlEditableProperty(name, value, element, () =>
                            {
                                return BrowserWindowController.createActionBrowser();
                            }));
                        break;
                    default:
                        editInterface.addEditableProperty(new RmlEditableProperty(name, value, element));
                        break;
                }
            }
            EditInterface = editInterface;
        }

        public override void Dispose()
        {
            propertiesForm.Dispose();
            base.Dispose();
        }

        public String Text
        {
            get
            {
                return text.OnlyText;
            }
            set
            {
                text.OnlyText = value.Replace("\r", "");            
            }
        }

        public EditInterface EditInterface
        {
            get
            {
                return propertiesForm.EditInterface;
            }
            set
            {
                propertiesForm.EditInterface = value;
            }
        }

        public uint MaxLength
        {
            get
            {
                return text.MaxTextLength;
            }
            set
            {
                text.MaxTextLength = value;
            }
        }

        public bool WordWrap
        {
            get
            {
                return text.EditWordWrap;
            }
            set
            {
                text.EditWordWrap = value;
            }
        }
    }
}
