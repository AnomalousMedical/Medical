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
        public static RmlElementEditor openTextEditor(MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, Element element, int left, int top)
        {
            RmlElementEditor textEditor = new RmlElementEditor(uiCallback, browserProvider, element);
            textEditor.show(left, top);
            textEditor.Hidden += (source, e) =>
            {
                ((RmlElementEditor)source).Dispose();
            };
            return textEditor;
        }

        public event Action<Element> MoveElementUp;
        public event Action<Element> MoveElementDown;
        public event Action<Element> DeleteElement;

        private EditBox text;
        private ScrollView propertiesScroll;
        private ScrollablePropertiesForm propertiesForm;
        private Element element;

        protected RmlElementEditor(MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, Element element)
            :base("Medical.GUI.RmlWysiwyg.RmlElementEditor.layout")
        {
            this.element = element;

            text = (EditBox)widget.findWidget("Text");
            Text = element.InnerRml;
            text.KeyButtonReleased += new MyGUIEvent(text_KeyButtonReleased);
            InputManager.Instance.setKeyFocusWidget(text);

            propertiesScroll = (ScrollView)widget.findWidget("PropertiesScroll");
            propertiesForm = new ScrollablePropertiesForm(propertiesScroll, uiCallback);

            Button applyButton = (Button)widget.findWidget("ApplyButton");
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            Button cancelButton = (Button)widget.findWidget("CancelButton");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button up = (Button)widget.findWidget("Up");
            up.MouseButtonClick += new MyGUIEvent(up_MouseButtonClick);

            Button down = (Button)widget.findWidget("Down");
            down.MouseButtonClick += new MyGUIEvent(down_MouseButtonClick);

            Button delete = (Button)widget.findWidget("Delete");
            delete.MouseButtonClick += new MyGUIEvent(delete_MouseButtonClick);

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
                        editInterface.addEditableProperty(new RmlEditableProperty(name, value, element, callback =>
                            {
                                return browserProvider.createActionBrowser();
                            }));
                        break;
                    case "src":
                        if (element.TagName == "img")
                        {
                            TabControl tabs = (TabControl)widget.findWidget("Tabs");
                            tabs.IndexSelected = 1;
                            editInterface.addEditableProperty(new RmlEditableProperty(name, value, element, callback =>
                            {
                                return browserProvider.createFileBrowser(new String[]{ "*.png", "*.jpg", "*jpeg", "*.gif", "*.bmp"}, "Images", "/");
                            }));
                        }
                        else
                        {
                            editInterface.addEditableProperty(new RmlEditableProperty(name, value, element));
                        }
                        break;
                    default:
                        editInterface.addEditableProperty(new RmlEditableProperty(name, value, element));
                        break;
                }
            }
            EditInterface = editInterface;
            ApplyChanges = true;
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

        public String UndoRml { get; set; }

        public bool ApplyChanges { get; set; }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ApplyChanges = false;
            this.hide();
        }

        void text_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            if (ke.Key == Engine.Platform.KeyboardButtonCode.KC_RETURN && InputManager.Instance.isControlPressed())
            {
                this.hide();
            }
        }

        void delete_MouseButtonClick(Widget source, EventArgs e)
        {
            if (DeleteElement != null)
            {
                DeleteElement.Invoke(element);
            }
            hide();
        }

        void down_MouseButtonClick(Widget source, EventArgs e)
        {
            if (MoveElementDown != null)
            {
                MoveElementDown.Invoke(element);
            }
        }

        void up_MouseButtonClick(Widget source, EventArgs e)
        {
            if (MoveElementUp != null)
            {
                MoveElementUp.Invoke(element);
            }
        }
    }
}
