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
    partial class OneWaySplit : DrawingSplitView
    {
        public OneWaySplit()
        {
            InitializeComponent();
        }

        #region SplitView Members

        public override Control FrontView
        {
            get
            {
                return this;
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

        #endregion
    }
}
