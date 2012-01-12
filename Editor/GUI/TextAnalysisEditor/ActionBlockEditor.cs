using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.Exam;

namespace Medical.GUI
{
    enum AddActionIndex
    {
        StartParagraph,
        EndParagraph,
        Write
    }

    class ActionBlockEditor : AnalysisEditorComponent
    {
        private List<AnalysisEditorComponent> childEditors = new List<AnalysisEditorComponent>();
        private int childStartHeight;
        private int extraHeight;

        public ActionBlockEditor(AnalysisEditorComponentParent parent)
            :base("Medical.GUI.TextAnalysisEditor.ActionBlockEditor.layout", parent)
        {
            childStartHeight = 16;
            extraHeight = widget.Height - childStartHeight;
        }

        public override void Dispose()
        {
            disposeChildEditors();
            base.Dispose();
        }

        public void empty()
        {
            disposeChildEditors();
            childEditors.Clear();
            requestLayout();
        }

        public void createFromAnalyzer(ActionBlock actionBlock)
        {
            AllowLayout = false;
            empty();
            foreach (AnalysisAction action in actionBlock.Actions)
            {
                if (action is StartParagraph)
                {
                    addChildEditor(new StartParagraphEditor(this));
                }
                else if (action is EndParagraph)
                {
                    addChildEditor(new EndParagraphEditor(this));
                }
                else if (action is Write)
                {
                    addChildEditor(new WriteEditor(this, (Write)action));
                }
                else if (action is TestAction)
                {
                    addChildEditor(new TestEditor(this, (TestAction)action));
                }
            }
            AllowLayout = true;
            requestLayout();
        }

        public override void layout(int left, int top, int width)
        {
            int currentTop = childStartHeight;
            int innerWidth = width - 5;
            foreach (AnalysisEditorComponent child in childEditors)
            {
                child.layout(5, currentTop, innerWidth);
                currentTop += child.Height;
            }
            widget.setCoord(left, top, width, currentTop + extraHeight);
        }

        public override AnalysisAction createAction()
        {
            if (childEditors.Count > 0)
            {
                ActionBlock actionBlock = new ActionBlock();
                foreach (AnalysisEditorComponent child in childEditors)
                {
                    actionBlock.addAction(child.createAction());
                }
                return actionBlock;
            }
            return null;
        }

        public void addChildEditor(AnalysisEditorComponent child)
        {
            childEditors.Add(child);
            requestLayout();
        }

        public void removeChildEditor(AnalysisEditorComponent child)
        {
            childEditors.Remove(child);
            requestLayout();
        }

        public override bool Removeable
        {
            get
            {
                return false;
            }
        }

        public override ActionBlockEditor OwnerActionBlockEditor
        {
            get
            {
                return this;
            }
        }

        void disposeChildEditors()
        {
            foreach (AnalysisEditorComponent child in childEditors)
            {
                child.Dispose();
            }
        }
    }
}
