using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    class HeadToolStrip : ToolStrip
    {
        public HeadToolStrip()
        {
            ToolStripButton musclesButton = new ToolStripButton("Muscles");
            musclesButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.Items.Add(musclesButton);

            ToolStripButton mandibleButton = new ToolStripButton("Mandible");
            mandibleButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.Items.Add(mandibleButton);

            ToolStripButton diskButton = new ToolStripButton("Disk");
            diskButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.Items.Add(diskButton);

            ToolStripButton teethButton = new ToolStripButton("Teeth");
            teethButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.Items.Add(teethButton);
        }
    }
}
