using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.GUI.Layers;

namespace Medical.GUI
{
    public partial class LayersControl : UserControl
    {
        public LayersControl()
        {
            InitializeComponent();
        }

        public void addLayerSection(String name)
        {
            LayerSection section = new LayerSection();
            section.Text = name;
            sectionsPanel.Controls.Add(section);
        }
    }
}
