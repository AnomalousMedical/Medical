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
        private PopupMenu addActionMenu;
        private Button removeButton;
        private List<AnalysisEditorComponent> childEditors = new List<AnalysisEditorComponent>();
        private int childStartHeight;
        private int extraHeight;

        public ActionBlockEditor(AnalysisEditorComponentParent parent)
            :base("Medical.GUI.TextAnalysisEditor.ActionBlockEditor.layout", parent)
        {
            removeButton = (Button)widget.findWidget("Remove");
            Button addButton = (Button)widget.findWidget("AddAction");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            childStartHeight = removeButton.Bottom;
            extraHeight = widget.Height - childStartHeight;

            addActionMenu = (PopupMenu)Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1, 1, Align.Default, "Overlapped", "");
            addActionMenu.ItemAccept += new MyGUIEvent(addActionMenu_ItemAccept);
            MenuItem addStartParagraph = addActionMenu.addItem("Start Paragraph", MenuItemType.Normal, "StartParagraph");
            MenuItem addEndParagraph = addActionMenu.addItem("End Paragraph", MenuItemType.Normal, "EndParagraph");
            MenuItem addWrite = addActionMenu.addItem("Write", MenuItemType.Normal, "Write");
            MenuItem addActionBlock = addActionMenu.addItem("Action Block", MenuItemType.Normal, "ActionBlock");
            addActionMenu.Visible = false;
        }

        public override void Dispose()
        {
            disposeChildEditors();
            Gui.Instance.destroyWidget(addActionMenu);
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
            }
            AllowLayout = true;
            requestLayout();
        }

        public override void layout(int left, int top, int width)
        {
            int currentTop = childStartHeight;
            foreach (AnalysisEditorComponent child in childEditors)
            {
                child.layout(left, currentTop, width);
                currentTop += child.Height;
            }
            widget.setCoord(left, top, width, currentTop + extraHeight);
        }

        public override AnalysisAction createAction()
        {
            ActionBlock actionBlock = new ActionBlock();
            foreach (AnalysisEditorComponent child in childEditors)
            {
                actionBlock.addAction(child.createAction());
            }
            return actionBlock;
        }

        public void addChildEditor(AnalysisEditorComponent child)
        {
            childEditors.Add(child);
            requestLayout();
        }

        public override void removeChildComponent(AnalysisEditorComponent child)
        {
            childEditors.Remove(child);
            requestLayout();
        }

        public bool Removeable
        {
            get
            {
                return removeButton.Visible;
            }
            set
            {
                removeButton.Visible = value;
            }
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            addActionMenu.setPosition(source.AbsoluteLeft, source.AbsoluteTop);
            addActionMenu.setVisibleSmooth(true);
            LayerManager.Instance.upLayerItem(addActionMenu);
        }

        void addActionMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            switch (mcae.Item.ItemId)
            {
                case "StartParagraph":
                    addChildEditor(new StartParagraphEditor(this));
                    break;
                case "EndParagraph":
                    addChildEditor(new EndParagraphEditor(this));
                    break;
                case "Write":
                    addChildEditor(new WriteEditor(this));
                    break;
                case "ActionBlock":
                    addChildEditor(new ActionBlockEditor(this));
                    break;
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
