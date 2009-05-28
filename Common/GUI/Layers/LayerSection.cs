using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI.Layers
{
    public partial class LayerSection : UserControl
    {
        private bool expanded = false;
        private TransparencyGroup group;

        public LayerSection(TransparencyGroup group)
        {
            InitializeComponent();

            this.group = group;
            Text = group.Name.ToString();

            groupTransparency.ValueChanged += new EventHandler(groupTransparency_ValueChanged);
            foreach (TransparencyInterface transparency in group.getTransparencyObjectIter())
            {
                LayerEntry entry = new LayerEntry();
                entry.initialize(transparency);
                this.entriesPanel.Controls.Add(entry);
            }
        }

        void groupTransparency_ValueChanged(object sender, EventArgs e)
        {
            if (expanded)
            {
                foreach (LayerEntry entry in entriesPanel.Controls)
                {
                    if (entry.Selected)
                    {
                        entry.setAlpha(groupTransparency.Value);
                    }
                }
            }
            else
            {
                foreach (LayerEntry entry in entriesPanel.Controls)
                {
                    entry.setAlpha(groupTransparency.Value);
                }
            }
        }

        private void expandButton_Click(object sender, EventArgs e)
        {
            if (expanded)
            {
                expandButton.Text = "+";
                expanded = false;
                this.Height = this.groupTransparency.PreferredSize.Height;
            }
            else
            {
                expandButton.Text = "-";
                expanded = true;
                this.Height = this.PreferredSize.Height;
            }
        }

        public override string Text
        {
            get
            {
                return categoryLabel.Text;
            }
            set
            {
                categoryLabel.Text = value;
            }
        }
    }
}
