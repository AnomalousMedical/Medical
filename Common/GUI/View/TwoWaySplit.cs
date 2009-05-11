using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    partial class TwoWaySplit : DrawingSplitView
    {
        public TwoWaySplit()
        {
            InitializeComponent();
        }

        public override Control FrontView
        {
            get
            {
                return verticalSplit.Panel1;
            }
        }

        public override Control BackView
        {
            get
            {
                return verticalSplit.Panel2;
            }
        }

        public override Control LeftView
        {
            get
            {
                return null;
            }
        }

        public override Control RightView
        {
            get
            {
                return null;
            }
        }
    }
}
