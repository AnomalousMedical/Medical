using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    abstract class DrawingSplitView : UserControl
    {
        public abstract Control FrontView
        {
            get;
        }

        public abstract Control BackView
        {
            get;
        }

        public abstract Control LeftView
        {
            get;
        }

        public abstract Control RightView
        {
            get;
        }
    }
}
