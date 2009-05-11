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
    partial class ThreeWayUpperSplit : DrawingSplitView
    {
        public ThreeWayUpperSplit()
        {
            InitializeComponent();
        }

        #region SplitView Members

        public override Control FrontView
        {
            get
            {
                return horizontalSplit.Panel2;
            }
        }

        public override Control BackView
        {
            get
            {
                return null;
            }
        }

        public override Control LeftView
        {
            get
            {
                return verticalSplit.Panel1;
            }
        }

        public override Control RightView
        {
            get
            {
                return verticalSplit.Panel2;
            }
        }

        #endregion
    }
}
