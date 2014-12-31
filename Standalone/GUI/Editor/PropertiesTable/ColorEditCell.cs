using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Anomalous.OSPlatform;

namespace Medical.GUI
{
    class ColorEditCell : TableCell
    {
        const int BROWSE_BUTTON_WIDTH = 25;

        private EditBox editWidget;
        private Button browseButton;
        private TextBox staticWidget;
        private String value = null;

        public ColorEditCell()
        {

        }

        public override void Dispose()
        {
            if (editWidget != null)
            {
                Gui.Instance.destroyWidget(browseButton);
                Gui.Instance.destroyWidget(editWidget);
                editWidget = null;
                browseButton = null;
            }
            if (staticWidget != null)
            {
                Gui.Instance.destroyWidget(staticWidget);
                staticWidget = null;
            }
        }

        public override void setStaticMode()
        {
            ensureStaticWidgetExists(TableWidget);
            if (!staticWidget.Visible)
            {
                staticWidget.Visible = true;
                if (editWidget != null)
                {
                    editWidget.Visible = false;
                    browseButton.Visible = false;
                }
            }
        }

        public override void setEditMode()
        {
            ensureEditWidgetExists(TableWidget);
            if (!editWidget.Visible)
            {
                editWidget.Visible = true;
                browseButton.Visible = true;
                if (staticWidget != null)
                {
                    staticWidget.Visible = false;
                }
                InputManager.Instance.setKeyFocusWidget(editWidget);
                editWidget.setTextSelection(0, (uint)editWidget.OnlyText.Length);
            }
        }

        public override TableCell clone()
        {
            return new ColorEditCell();
        }

        protected override void sizeChanged()
        {
            if (editWidget != null)
            {
                editWidget.setSize(Size.Width - BROWSE_BUTTON_WIDTH, Size.Height);
                browseButton.setPosition(Size.Width - BROWSE_BUTTON_WIDTH, browseButton.Top);
                browseButton.setSize(BROWSE_BUTTON_WIDTH, Size.Height);
            }
            if (staticWidget != null)
            {
                staticWidget.setSize(Size.Width, Size.Height);
            }
        }

        protected override void positionChanged()
        {
            if (editWidget != null)
            {
                editWidget.setPosition(Position.x, Position.y);
                browseButton.setPosition(editWidget.Right, Position.y);
            }
            if (staticWidget != null)
            {
                staticWidget.setPosition(Position.x, Position.y);
            }
        }

        protected override Object EditValueImpl
        {
            get
            {
                if (editWidget != null)
                {
                    return editWidget.OnlyText;
                }
                else
                {
                    return value;
                }
            }
        }

        protected override void commitEditValueToValueImpl()
        {
            Value = editWidget.OnlyText;
        }

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                String sentValueString = value != null ? value.ToString() : null;
                if (this.value != sentValueString)
                {
                    this.value = sentValueString;
                    if (editWidget != null)
                    {
                        editWidget.OnlyText = sentValueString;
                    }
                    if (staticWidget != null)
                    {
                        staticWidget.Caption = sentValueString;
                    }
                    fireCellValueChanged();
                }
            }
        }

        private void ensureStaticWidgetExists(Widget parentWidget)
        {
            if (staticWidget == null)
            {
                staticWidget = (TextBox)parentWidget.createWidgetT("Button", "Button", Position.x, Position.y, Size.Width, Size.Height, Align.Default, "");
                staticWidget.MouseButtonClick += new MyGUIEvent(staticWidget_MouseButtonClick);
                staticWidget.Caption = value;
                staticWidget.TextAlign = Align.Left | Align.VCenter;
                staticWidget.Visible = false;
            }
        }

        private void ensureEditWidgetExists(Widget parentWidget)
        {
            if (editWidget == null)
            {
                editWidget = parentWidget.createWidgetT("Edit", "Edit", Position.x, Position.y, Size.Width - BROWSE_BUTTON_WIDTH, Size.Height, Align.Default, "") as EditBox;
                editWidget.KeyLostFocus += new MyGUIEvent(editWidget_KeyLostFocus);
                editWidget.KeyButtonReleased += new MyGUIEvent(editWidget_KeyButtonReleased);
                editWidget.OnlyText = value;
                editWidget.Visible = false;

                browseButton = parentWidget.createWidgetT("Button", "Button", editWidget.Right, Position.y, BROWSE_BUTTON_WIDTH, Size.Height, Align.Default, "") as Button;
                browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
                browseButton.Caption = "...";
            }
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.showModal((result, color) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    editWidget.OnlyText = color.ToString();
                    clearCellEdit();
                }
            });
        }

        void editWidget_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            if (ke.Key == Engine.Platform.KeyboardButtonCode.KC_RETURN)
            {
                clearCellEdit();
            }
        }

        void editWidget_KeyLostFocus(Widget source, EventArgs e)
        {
            //FocusEventArgs fe = (FocusEventArgs)e;
            //if (fe.OtherWidget != browseButton)
            //{
            //    clearCellEdit();
            //}
        }

        void staticWidget_MouseButtonClick(Widget source, EventArgs e)
        {
            if (!Table.Columns[ColumnIndex].ReadOnly)
            {
                requestCellEdit();
            }
        }
    }
}
