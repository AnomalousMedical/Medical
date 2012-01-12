﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.Exam;
using Engine;

namespace Medical.GUI
{
    abstract class AnalysisEditorComponent : Component, AnalysisEditorComponentParent
    {
        private bool selected = false;

        public AnalysisEditorComponent(String layoutFile, AnalysisEditorComponentParent parent)
            :base(layoutFile, parent.Widget)
        {
            this.Parent = parent;
            AllowLayout = true;

            Button remove = (Button)widget.findWidget("Remove");
            remove.MouseButtonClick += new MyGUIEvent(remove_MouseButtonClick);
            widget.MouseButtonClick += new MyGUIEvent(widget_MouseButtonClick);
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

        public int Bottom
        {
            get
            {
                return widget.Bottom;
            }
        }

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                if (selected)
                {
                    widget.setColour(new Color(0, 0, 1));
                }
                else
                {
                    widget.setColour(new Color(1, 1, 1));
                }
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
            if (AllowLayout)
            {
                Parent.requestLayout();
            }
        }

        public void requestSelected(AnalysisEditorComponent component)
        {
            Parent.requestSelected(component);
        }

        public void requestSelected()
        {
            requestSelected(this);
        }

        public AnalysisEditorComponentParent Parent { get; set; }

        public Widget Widget
        {
            get
            {
                return widget;
            }
        }

        public bool AllowLayout { get; set; }

        void remove_MouseButtonClick(Widget source, EventArgs e)
        {
            Parent.removeChildComponent(this);
            this.Dispose();
        }

        void widget_MouseButtonClick(Widget source, EventArgs e)
        {
            requestSelected();
        }
    }
}
