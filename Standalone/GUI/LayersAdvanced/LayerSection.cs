using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class LayerSection : Component
    {
        public event EventHandler SizeChanged;

        private StaticText categoryLabel;
        private NumericEdit transparencyEdit;
        private CheckButton expandButton;

        private bool expanded = false;
        private TransparencyGroup group;

        private List<LayerEntry> layerEntries = new List<LayerEntry>();
        private FlowLayoutContainer flowLayout;

        private MyGUILayoutContainer myGUIContainer;

        public LayerSection(TransparencyGroup group, Widget parent)
            :base("Medical.GUI.LayersAdvanced.LayerSection.layout")
        {
            myGUIContainer = new MyGUILayoutContainer(widget);

            widget.Visible = false;
            widget.attachToWidget(parent);

            categoryLabel = widget.findWidget("CategoryLabel") as StaticText;
            transparencyEdit = new NumericEdit(widget.findWidget("TransparencyEdit") as Edit);
            expandButton = new CheckButton(widget.findWidget("ExpandButton") as Button);
            expandButton.Checked = !expanded;
            expandButton.CheckedChanged += new MyGUIEvent(expandButton_CheckedChanged);

            this.group = group;
            Caption = group.Name.ToString();

            flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 2.0f, new Vector2(0.0f, categoryLabel.Height));

            flowLayout.SuppressLayout = true;
            transparencyEdit.ValueChanged += new MyGUIEvent(transparencyEdit_ValueChanged);
            foreach (TransparencyInterface transparency in group.getTransparencyObjectIter())
            {
                LayerEntry entry = new LayerEntry(transparency, widget);
                layerEntries.Add(entry);
                flowLayout.addChild(entry.Container);
            }
            flowLayout.SuppressLayout = false;
            flowLayout.layout();
        }

        public override void Dispose()
        {
            foreach (LayerEntry entry in layerEntries)
            {
                entry.Dispose();
            }
            layerEntries.Clear();
            base.Dispose();
        }

        void transparencyEdit_ValueChanged(Widget source, EventArgs e)
        {
            if (expanded)
            {
                foreach (LayerEntry entry in layerEntries)
                {
                    if (entry.Selected)
                    {
                        entry.setAlpha(transparencyEdit.FloatValue);
                    }
                }
            }
            else
            {
                foreach (LayerEntry entry in layerEntries)
                {
                    entry.setAlpha(transparencyEdit.FloatValue);
                }
            }
        }

        void expandButton_CheckedChanged(Widget source, EventArgs e)
        {
            if (expanded)
            {
                expanded = false;
                myGUIContainer.changeDesiredSize(new Size2(widget.Width, categoryLabel.Height));
            }
            else
            {
                expanded = true;
                if (layerEntries.Count > 0)
                {
                    myGUIContainer.changeDesiredSize(new Size2(widget.Width, layerEntries[layerEntries.Count - 1].Bottom));
                }
                else
                {
                    myGUIContainer.changeDesiredSize(new Size2(widget.Width, categoryLabel.Height));
                }
            }

            if (SizeChanged != null)
            {
                SizeChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public string Caption
        {
            get
            {
                return categoryLabel.Caption;
            }
            set
            {
                categoryLabel.Caption = value;
            }
        }

        public LayoutContainer Container
        {
            get
            {
                return myGUIContainer;
            }
        }

        public int Bottom
        {
            get
            {
                return widget.Bottom;
            }
        }
    }
}
