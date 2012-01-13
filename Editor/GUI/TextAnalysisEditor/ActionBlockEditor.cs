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

        public ActionBlockEditor(String name, AnalysisEditorComponentParent parent)
            :base("Medical.GUI.TextAnalysisEditor.ActionBlockEditor.layout", parent)
        {
            StaticText actionBlockText = (StaticText)widget.findWidget("ActionBlockText");
            actionBlockText.Caption = name;

            childStartHeight = actionBlockText.Bottom;
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
            mergeActionBlock(actionBlock);
            AllowLayout = true;
            requestLayout();
        }

        public void addFromAction(AnalysisAction action)
        {
            AllowLayout = false;
            if (action is ActionBlock)
            {
                mergeActionBlock((ActionBlock)action);
            }
            else
            {
                addChildEditor(createEditorFromAction(action));
            }
            AllowLayout = true;
            requestLayout();
        }

        public void insertFromAction(AnalysisAction action, AnalysisEditorComponent before)
        {
            AllowLayout = false;
            if (action is ActionBlock)
            {
                mergeInsertActionBlock((ActionBlock)action, before);
            }
            else
            {
                insertChildEditor(createEditorFromAction(action), before);
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

        public void insertChildEditor(AnalysisEditorComponent newChild, AnalysisEditorComponent before)
        {
            int index = childEditors.IndexOf(before);
            if (index != -1)
            {
                childEditors.Insert(index, newChild);
            }
            else
            {
                childEditors.Add(newChild);
            }
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

        private void mergeActionBlock(ActionBlock actionBlock)
        {
            foreach (AnalysisAction action in actionBlock.Actions)
            {
                addChildEditor(createEditorFromAction(action));
            }
        }

        private void mergeInsertActionBlock(ActionBlock actionBlock, AnalysisEditorComponent before)
        {
            foreach (AnalysisAction action in actionBlock.Actions)
            {
                insertChildEditor(createEditorFromAction(action), before);
            }
        }

        private AnalysisEditorComponent createEditorFromAction(AnalysisAction action)
        {
            if (action is StartParagraph)
            {
                return new StartParagraphEditor(this);
            }
            else if (action is EndParagraph)
            {
                return new EndParagraphEditor(this);
            }
            else if (action is Write)
            {
                return new WriteEditor(this, (Write)action);
            }
            else if (action is TestAction)
            {
                return new TestEditor(this, (TestAction)action);
            }
            throw new NotSupportedException();
        }
    }
}
