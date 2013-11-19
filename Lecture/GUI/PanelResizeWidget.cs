﻿using Engine;
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
        int panelStartingSize;

        public delegate void RecordResizeInfoDelegate(RmlEditorViewInfo view, int oldSize, int newSize);
        public event RecordResizeInfoDelegate RecordResizeUndo;

        public PanelResizeWidget()
        {
            resizeWidget = Gui.Instance.createWidgetT("Widget", "ResizeHandle", 0, 0, ScaleHelper.Scaled(20), ScaleHelper.Scaled(20), Align.Default, "Overlapped", "Resize");
            resizeWidget.MouseButtonPressed += resizeWidget_MouseButtonPressed;
            resizeWidget.MouseDrag += resizeWidget_MouseDrag;
            resizeWidget.MouseButtonReleased += resizeWidget_MouseButtonReleased;
            resizeWidget.Visible = false;
        }

        public void Dispose()
        {
            setCurrentEditor(null);
            Gui.Instance.destroyWidget(resizeWidget);
        }

        void resizeWidget_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Widget editorWidget = currentEditor.Component.Widget;
            switch (currentEditor.View.ViewLocation)
            {
                case ViewLocations.Left:
                    resizeWidget.setPosition(me.Position.x + mousePressOffset.x, resizeWidget.AbsoluteTop);
                    currentEditor.resizePanel(resizeWidget.AbsoluteLeft + resizeWidget.Width / 2 - editorWidget.AbsoluteLeft);
                    break;
                case ViewLocations.Right:
                    resizeWidget.setPosition(me.Position.x + mousePressOffset.x, resizeWidget.AbsoluteTop);
                    currentEditor.resizePanel(editorWidget.AbsoluteLeft + editorWidget.Width - (resizeWidget.AbsoluteLeft + resizeWidget.Width / 2));
                    break;
                case ViewLocations.Top:
                    resizeWidget.setPosition(resizeWidget.AbsoluteLeft, me.Position.y + mousePressOffset.y);
                    currentEditor.resizePanel(resizeWidget.AbsoluteTop + resizeWidget.Height / 2 - editorWidget.AbsoluteTop);
                    break;
                case ViewLocations.Bottom:
                    resizeWidget.setPosition(resizeWidget.AbsoluteLeft, me.Position.y + mousePressOffset.y);
                    currentEditor.resizePanel(editorWidget.AbsoluteTop + editorWidget.Height - (resizeWidget.AbsoluteTop + resizeWidget.Height / 2));
                    break;
            }
        }

        void resizeWidget_MouseButtonReleased(Widget source, EventArgs e)
        {
            if (RecordResizeUndo != null)
            {
                RecordResizeUndo.Invoke(currentEditor, panelStartingSize, currentEditor.Panel.Size);
            }
        }

        void resizeWidget_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            mousePressOffset = me.Position;
            mousePressOffset.x = resizeWidget.AbsoluteLeft - mousePressOffset.x;
            mousePressOffset.y = resizeWidget.AbsoluteTop - mousePressOffset.y;
            panelStartingSize = currentEditor.Panel.Size;
        }

        public void setCurrentEditor(RmlEditorViewInfo editor)
        {
            if (currentEditor != null && currentEditor.Component != null)
            {
                currentEditor.Component.ViewHost.ViewResized -= ViewHost_ViewResized;
            }
            currentEditor = editor;
            positionResizeWidget();
            if (currentEditor != null && currentEditor.Component != null)
            {
                currentEditor.Component.ViewHost.ViewResized += ViewHost_ViewResized;
            }
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

        void ViewHost_ViewResized(ViewHost viewHost)
        {
            positionResizeWidget();
        }
    }
}