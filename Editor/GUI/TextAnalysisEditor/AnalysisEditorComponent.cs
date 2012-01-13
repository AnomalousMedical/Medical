using System;
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

        public virtual void cut(SaveableClipboard clipboard)
        {
            clipboard.copyToSourceObject(createAction());
            OwnerActionBlockEditor.removeChildEditor(this);
            Dispose();
        }

        public virtual void copy(SaveableClipboard clipboard)
        {
            clipboard.copyToSourceObject(createAction());
        }

        public virtual void paste(SaveableClipboard clipboard)
        {
            AnalysisAction action;
            if (clipboard.HasSourceObject && (action = clipboard.createCopy<AnalysisAction>()) != null)
            {
                OwnerActionBlockEditor.addFromAction(action);
            }
        }

        public virtual void insertPaste(SaveableClipboard clipboard)
        {
            AnalysisAction action;
            if (clipboard.HasSourceObject && (action = clipboard.createCopy<AnalysisAction>()) != null)
            {
                OwnerActionBlockEditor.insertFromAction(action, this);
            }
        }

        public AnalysisEditorComponentParent Parent { get; set; }

        public virtual ActionBlockEditor OwnerActionBlockEditor
        {
            get
            {
                return Parent.OwnerActionBlockEditor;
            }
        }

        public Widget Widget
        {
            get
            {
                return widget;
            }
        }

        public virtual bool Removeable
        {
            get
            {
                return true;
            }
        }

        public bool AllowLayout { get; set; }

        void widget_MouseButtonClick(Widget source, EventArgs e)
        {
            requestSelected();
        }
    }
}
