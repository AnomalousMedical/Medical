using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    public class ExpandingNode : Component
    {
        public event Action<ExpandingNode> LayoutChanged;

        private TextBox caption;
        private CheckButton expandButton;
        private EditInterface editInterface;

        private int defaultHeight;
        private PropertiesForm propertiesForm;
        private MedicalUICallback uiCallback;
        private ExpandingNode parent;
        private Widget childArea;
        private bool respondToPropertiesFormLayout = true;
        private bool allowLayout = true;

        private List<ExpandingNode> children = new List<ExpandingNode>();

        public ExpandingNode(EditInterface editInterface, Widget parentWidget, MedicalUICallback uiCallback, ExpandingNode parent = null)
            : base("Medical.GUI.Editor.ExpandingEditInterfaceViewer.ExpandingNode.layout")
        {
            this.uiCallback = uiCallback;
            this.parent = parent;

            widget.attachToWidget(parentWidget);
            defaultHeight = widget.Height;

            caption = (TextBox)widget.findWidget("Caption");
            caption.Caption = editInterface.getName();

            expandButton = new CheckButton((Button)widget.findWidget("ExpandButton"));
            expandButton.CheckedChanged += new MyGUIEvent(expandButton_CheckedChanged);

            childArea = widget.findWidget("ChildArea");
            propertiesForm = new PropertiesForm(childArea, uiCallback);
            propertiesForm.LayoutChanged += propertiesForm_LayoutChanged;

            this.editInterface = editInterface;
            editInterface.OnSubInterfaceAdded += editInterface_OnSubInterfaceAdded;
            editInterface.OnSubInterfaceRemoved += editInterface_OnSubInterfaceRemoved;
        }

        public override void Dispose()
        {
            editInterface.OnSubInterfaceAdded -= editInterface_OnSubInterfaceAdded;
            editInterface.OnSubInterfaceRemoved -= editInterface_OnSubInterfaceRemoved;
            foreach (ExpandingNode child in children)
            {
                child.Dispose();
            }
            children.Clear();
            propertiesForm.Dispose();
            base.Dispose();
        }

        public void layout()
        {
            if (allowLayout)
            {
                if (parent != null)
                {
                    parent.layout();
                }
                else
                {
                    doLayout(0);
                    if (LayoutChanged != null)
                    {
                        LayoutChanged.Invoke(this);
                    }
                }
            }
        }

        private void doLayout(int top)
        {
            widget.setPosition(0, top);
            if (Expanded)
            {
                respondToPropertiesFormLayout = false;
                propertiesForm.layout();
                respondToPropertiesFormLayout = true;
                int height = propertiesForm.Height;
                foreach (ExpandingNode child in children)
                {
                    child.doLayout(height);
                    height += child.Height;
                }
                height += defaultHeight;
                widget.setSize(widget.Width, height);
            }
            else
            {
                widget.setSize(widget.Width, defaultHeight);
            }
        }

        public bool Expanded
        {
            get
            {
                return !expandButton.Checked;
            }
            set
            {
                expandButton.Checked = !value;
            }
        }

        public void changeWidth(int width)
        {
            widget.setSize(width, widget.Height);
            foreach (ExpandingNode child in children)
            {
                child.changeWidth(width - childArea.Left);
            }
        }

        public void expandChildren()
        {
            allowLayout = false;
            foreach (var child in children)
            {
                child.Expanded = true;
            }
            allowLayout = true;
            layout();
        }

        public int Height
        {
            get
            {
                return widget.Height;
            }
        }

        public int Width
        {
            get
            {
                return widget.Width;
            }
        }

        void expandButton_CheckedChanged(Widget source, EventArgs e)
        {
            if (Expanded)
            {
                propertiesForm.EditInterface = editInterface;
                foreach (EditInterface subInterface in editInterface.getSubEditInterfaces())
                {
                    addSubInterface(subInterface);
                }
            }
            else
            {
                propertiesForm.EditInterface = null;
                foreach (ExpandingNode child in children)
                {
                    child.Dispose();
                }
                children.Clear();
            }
            layout();
        }

        private ExpandingNode addSubInterface(EditInterface subInterface)
        {
            ExpandingNode node = new ExpandingNode(subInterface, childArea, uiCallback, this);
            children.Add(node);
            node.changeWidth(childArea.Width);
            layout();
            return node;
        }

        void editInterface_OnSubInterfaceRemoved(EditInterface editInterface)
        {
            foreach (ExpandingNode node in children)
            {
                if (node.editInterface == editInterface)
                {
                    node.Dispose();
                    children.Remove(node);
                    layout();
                    break;
                }
            }
        }

        void editInterface_OnSubInterfaceAdded(EditInterface editInterface)
        {
            addSubInterface(editInterface);
        }

        void propertiesForm_LayoutChanged(PropertiesForm obj)
        {
            if (respondToPropertiesFormLayout)
            {
                layout();
            }
        }
    }
}
