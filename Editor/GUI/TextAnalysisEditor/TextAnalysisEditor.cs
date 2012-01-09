using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class TextAnalysisEditor : MDIDialog, AnalysisEditorComponentParent
    {
        private ActionBlockEditor actionBlockEditor;
        private ScrollView scrollView;
        private int windowWidth;

        public TextAnalysisEditor()
            : base("Medical.GUI.TextAnalysisEditor.TextAnalysisEditor.layout")
        {
            windowWidth = window.Width;
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            scrollView = (ScrollView)window.findWidget("ScrollView");

            actionBlockEditor = new ActionBlockEditor(this);
            actionBlockEditor.Removeable = false;
            layout((int)scrollView.ClientCoord.width);
        }

        public override void Dispose()
        {
            actionBlockEditor.Dispose();
            base.Dispose();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            if (window.Width != windowWidth)
            {
                windowWidth = window.Width;
                layout((int)scrollView.ClientCoord.width);
            }
        }

        private void layout(int newWidth)
        {
            actionBlockEditor.layout(0, 0, newWidth);
            scrollView.CanvasSize = new Engine.Size2(newWidth, actionBlockEditor.Height);
        }

        public void requestLayout()
        {
            layout((int)scrollView.ClientCoord.width);
        }

        public AnalysisEditorComponentParent Parent
        {
            get
            {
                return null;
            }
        }

        public Widget Widget
        {
            get
            {
                return scrollView;
            }
        }

        public void removeChildComponent(AnalysisEditorComponent child)
        {
            throw new NotImplementedException();
        }
    }
}
