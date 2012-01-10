using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.Exam;

namespace Medical.GUI
{
    abstract class AnalysisEditorComponent : Component, AnalysisEditorComponentParent
    {
        public AnalysisEditorComponent(String layoutFile, AnalysisEditorComponentParent parent)
            :base(layoutFile, parent.Widget)
        {
            this.Parent = parent;

            Button remove = (Button)widget.findWidget("Remove");
            remove.MouseButtonClick += new MyGUIEvent(remove_MouseButtonClick);
        }

        public virtual void layout(int left, int top, int width)
        {
            widget.setCoord(left, top, width, widget.Height);
        }

        public abstract AnalysisAction createAction();

        public int Height
        {
            get
            {
                return widget.Height;
            }
        }

        public void openVariableBrowser(VariableChosenCallback variableChosenCallback)
        {
            Parent.openVariableBrowser(variableChosenCallback);
        }

        public virtual void removeChildComponent(AnalysisEditorComponent child)
        {
            throw new NotSupportedException();
        }

        public void requestLayout()
        {
            Parent.requestLayout();
        }

        public AnalysisEditorComponentParent Parent { get; set; }

        public Widget Widget
        {
            get
            {
                return widget;
            }
        }

        void remove_MouseButtonClick(Widget source, EventArgs e)
        {
            Parent.removeChildComponent(this);
            this.Dispose();
        }
    }
}
