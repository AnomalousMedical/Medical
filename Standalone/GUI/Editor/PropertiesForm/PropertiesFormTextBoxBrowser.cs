using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;
using Logging;

namespace Medical.GUI
{
    class PropertiesFormTextBoxBrowser : PropertiesFormLayoutComponent
    {
        private EditBox editBox;
        private bool allowValueChanges = true;
        private EditUICallback uiCallback;

        public PropertiesFormTextBoxBrowser(EditableProperty property, Widget parent, EditUICallback uiCallback)
            :base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormTextBoxBrowser.layout")
        {
            this.uiCallback = uiCallback;

            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;
            if (textBox.ClientWidget != null)
            {
                textBox.ClientWidget.ForwardMouseWheelToParent = true;
            }

            editBox = (EditBox)widget.findWidget("EditBox");
            editBox.OnlyText = property.getValue(1);
            editBox.ForwardMouseWheelToParent = true;
            if (property.readOnly(1))
            {
                editBox.EditReadOnly = true;
            }
            else
            {
                editBox.KeyLostFocus += new MyGUIEvent(editBox_KeyLostFocus);
                editBox.EventEditSelectAccept += new MyGUIEvent(editBox_EventEditSelectAccept);
            }

            Button browseButton = (Button)widget.findWidget("Browse");
            browseButton.ForwardMouseWheelToParent = true;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
        }

        public override void refreshData()
        {
            editBox.OnlyText = Property.getValue(1);
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Browser browser = Property.getBrowser(1, uiCallback);
            if (browser != null)
            {
                uiCallback.showBrowser(browser, delegate(Object result, ref string errorPrompt)
                {
                    editBox.OnlyText = result.ToString();
                    errorPrompt = "";
                    if (editBox.EditReadOnly)
                    {
                        if (allowValueChanges)
                        {
                            allowValueChanges = false;
                            Property.setValue(1, result);
                            allowValueChanges = true;
                        }
                    }
                    else
                    {
                        setValue();
                    }
                    return true;
                });
            }
            else
            {
                Log.Warning("Could not display browser for {0} column {1}", Property.ToString(), 1);
            }
        }

        void editBox_EventEditSelectAccept(Widget source, EventArgs e)
        {
            setValue();
        }

        void editBox_KeyLostFocus(Widget source, EventArgs e)
        {
            setValue();
        }

        private void setValue()
        {
            if (allowValueChanges)
            {
                allowValueChanges = false;
                String value = editBox.OnlyText;
                String errorMessage = null;
                if (Property.canParseString(1, value, out errorMessage))
                {
                    Property.setValueStr(1, value);
                }
                else
                {
                    MessageBox.show(errorMessage, "Parse Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
                allowValueChanges = true;
            }
        }
    }
}
