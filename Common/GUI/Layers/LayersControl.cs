using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.GUI.Layers;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    public partial class LayersControl : DockContent
    {
        public LayersControl()
        {
            InitializeComponent();
        }

        public void setupLayers()
        {
            foreach (Control control in sectionsPanel.Controls)
            {
                control.Dispose();
            }
            sectionsPanel.Controls.Clear();
            foreach (TransparencyGroup group in TransparencyController.getGroupIter())
            {
                LayerSection section = new LayerSection(group);
                sectionsPanel.Controls.Add(section);
            }
        }
    }
}
