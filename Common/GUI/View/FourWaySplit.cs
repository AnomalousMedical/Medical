using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    partial class FourWaySplit : DrawingSplitView
    {
        public FourWaySplit()
        {
            InitializeComponent();
        }

        #region SplitView Members

        public override Control FrontView
        {
            get
            {
                return leftVertical.Panel1;
            }
        }

        public override Control BackView
        {
            get
            {
                return rightVertical.Panel1;
            }
        }

        public override Control LeftView
        {
            get
            {
                return leftVertical.Panel2;
            }
        }

        public override Control RightView
        {
            get
            {
                return rightVertical.Panel2;
            }
        }

        #endregion
    }
}
