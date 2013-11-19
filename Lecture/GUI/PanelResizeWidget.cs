using Engine;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture
{
    class PanelResizeWidget : IDisposable
    {
        Widget resizeWidget;
        RmlEditorViewInfo currentEditor;
        IntVector2 mousePressOffset;

        public PanelResizeWidget()
        {
            resizeWidget = Gui.Instance.createWidgetT("Widget", "Panel", 0, 0, ScaleHelper.Scaled(20), ScaleHelper.Scaled(20), Align.Default, "Overlapped", "Resize");
            resizeWidget.MouseButtonPressed += resizeWidget_MouseButtonPressed;
            resizeWidget.MouseDrag += resizeWidget_MouseDrag;
            resizeWidget.Visible = false;
        }

        void resizeWidget_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            switch (currentEditor.View.ViewLocation)
            {
                case ViewLocations.Left:
                    resizeWidget.setPosition(me.Position.x + mousePressOffset.x, resizeWidget.AbsoluteTop);
                    currentEditor.resizePanel(resizeWidget.AbsoluteLeft + resizeWidget.Width / 2 - currentEditor.Component.Widget.AbsoluteLeft);
                    break;
                case ViewLocations.Right:
                    resizeWidget.setPosition(me.Position.x + mousePressOffset.x, resizeWidget.AbsoluteTop);
                    currentEditor.resizePanel(currentEditor.Component.Widget.AbsoluteLeft + currentEditor.Component.Widget.Width - (resizeWidget.AbsoluteLeft + resizeWidget.Width / 2));
                    break;
                case ViewLocations.Top:
                    resizeWidget.setPosition(resizeWidget.AbsoluteLeft, me.Position.y + mousePressOffset.y);
                    currentEditor.resizePanel(resizeWidget.AbsoluteTop + resizeWidget.Height / 2 - currentEditor.Component.Widget.AbsoluteTop);
                    break;
                case ViewLocations.Bottom:
                    resizeWidget.setPosition(resizeWidget.AbsoluteLeft, me.Position.y + mousePressOffset.y);
                    currentEditor.resizePanel(currentEditor.Component.Widget.AbsoluteTop + currentEditor.Component.Widget.Height - (resizeWidget.AbsoluteTop + resizeWidget.Height / 2));
                    break;
            }
        }

        void resizeWidget_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            mousePressOffset = me.Position;
            mousePressOffset.x = resizeWidget.AbsoluteLeft - mousePressOffset.x;
            mousePressOffset.y = resizeWidget.AbsoluteTop - mousePressOffset.y;
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(resizeWidget);
        }

        public void setCurrentEditor(RmlEditorViewInfo editor)
        {
            currentEditor = editor;
            positionResizeWidget();
        }

        private void positionResizeWidget()
        {
            if (currentEditor != null && currentEditor.Component != null)
            {
                resizeWidget.Visible = true;
                Widget editorWidget = currentEditor.Component.Widget;
                int left, top;
                switch (currentEditor.View.ViewLocation)
                {
                    case ViewLocations.Left:
                        left = editorWidget.AbsoluteLeft + editorWidget.Width;
                        top = editorWidget.AbsoluteTop + editorWidget.Height / 2;
                        resizeWidget.Pointer = MainWindow.SIZE_HORZ;
                        break;
                    case ViewLocations.Right:
                        left = editorWidget.AbsoluteLeft;
                        top = editorWidget.AbsoluteTop + editorWidget.Height / 2;
                        resizeWidget.Pointer = MainWindow.SIZE_HORZ;
                        break;
                    case ViewLocations.Top:
                        left = editorWidget.AbsoluteLeft + editorWidget.Width / 2;
                        top = editorWidget.AbsoluteTop + editorWidget.Height;
                        resizeWidget.Pointer = MainWindow.SIZE_VERT;
                        break;
                    case ViewLocations.Bottom:
                        left = editorWidget.AbsoluteLeft + editorWidget.Width / 2;
                        top = editorWidget.AbsoluteTop;
                        resizeWidget.Pointer = MainWindow.SIZE_VERT;
                        break;
                    default:
                        left = 0;
                        top = 0;
                        break;
                }
                resizeWidget.setPosition(left - resizeWidget.Width / 2, top - resizeWidget.Height / 2);
                LayerManager.Instance.upLayerItem(resizeWidget);
            }
            else
            {
                resizeWidget.Visible = false;
            }
        }
    }
}
