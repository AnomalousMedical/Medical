using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical
{
    class LayerStateTableCell : TableCell
    {
        private Widget editWidget;
        private Widget staticWidget;
        private LayerStateEditableProperty layerStateEditProp;

        public LayerStateTableCell(LayerStateEditableProperty layerStateEditProp)
        {
            this.layerStateEditProp = layerStateEditProp;
        }

        public override void Dispose()
        {
            if (editWidget != null)
            {
                Gui.Instance.destroyWidget(editWidget);
                editWidget = null;
            }
            if (staticWidget != null)
            {
                Gui.Instance.destroyWidget(staticWidget);
                staticWidget = null;
            }
        }

        protected override object EditValueImpl
        {
            get
            {
                return "Layer State";
            }
        }

        public override object Value
        {
            get
            {
                return layerStateEditProp.LayerState;
            }
            set
            {
                if (value is LayerState)
                {
                    if (staticWidget != null)
                    {
                        staticWidget.Caption = value != null ? value.ToString() : "Null";
                    }
                    layerStateEditProp.LayerState = (LayerState)value;
                }
            }
        }

        public override TableCell clone()
        {
            throw new NotSupportedException("These must be made manually and added to the table. They are not supported as column cell prototypes.");
        }

        protected override void commitEditValueToValueImpl()
        {

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
                }
            }
        }

        public override void setEditMode()
        {
            ensureEditWidgetExists(TableWidget);
            if (!editWidget.Visible)
            {
                editWidget.Visible = true;
                if (staticWidget != null)
                {
                    staticWidget.Visible = false;
                }
            }
        }

        protected override void positionChanged()
        {
            if (editWidget != null)
            {
                editWidget.setPosition(Position.x, Position.y);
            }
            if (staticWidget != null)
            {
                staticWidget.setPosition(Position.x, Position.y);
            }
        }

        protected override void sizeChanged()
        {
            if (editWidget != null)
            {
                editWidget.setSize(Size.Width, Size.Height);
            }
            if (staticWidget != null)
            {
                staticWidget.setSize(Size.Width, Size.Height);
            }
        }

        private void ensureStaticWidgetExists(Widget parentWidget)
        {
            if (staticWidget == null)
            {
                staticWidget = parentWidget.createWidgetT("Button", "Button", Position.x, Position.y, Size.Width, Size.Height, Align.Default, "");
                staticWidget.MouseButtonClick += new MyGUIEvent(staticWidget_MouseButtonClick);
                Object value = Value;
                staticWidget.Caption = value != null ? value.ToString() : "Null";
                staticWidget.Visible = false;
            }
        }

        private void ensureEditWidgetExists(Widget parentWidget)
        {
            if (editWidget == null)
            {
                editWidget = parentWidget.createWidgetT("Widget", "PanelEmpty", Position.x, Position.y, 30, Size.Height, Align.Default, "");
                Button captureButton = editWidget.createWidgetT("Button", "Button", 0, 0, 10, Size.Height, Align.Stretch, "") as Button;
                captureButton.MouseButtonClick += new MyGUIEvent(captureButton_MouseButtonClick);
                captureButton.Caption = "Capture";
                Button previewButton = editWidget.createWidgetT("Button", "Button", 10, 0, 10, Size.Height, Align.Stretch, "") as Button;
                previewButton.MouseButtonClick += new MyGUIEvent(previewButton_MouseButtonClick);
                previewButton.Caption = "Preview";
                Button cancelButton = editWidget.createWidgetT("Button", "Button", 20, 0, 10, Size.Height, Align.Stretch, "") as Button;
                cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
                cancelButton.Caption = "Cancel";
                editWidget.Visible = false;
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.clearCellEdit();
        }

        void previewButton_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void captureButton_MouseButtonClick(Widget source, EventArgs e)
        {
            LayerState layerState = new LayerState("");
            layerState.captureState();
            Value = layerState; //Slightly different flow here, just set the value directly since it cannot be wrong.
            this.clearCellEdit();
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
