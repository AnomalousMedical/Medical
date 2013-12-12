using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class ViewHostPanelInfo
    {
        public ViewHost Current { get; set; }

        public View Queued { get; set; }

        public AnomalousMvcContext QueuedContext { get; set; }

        public String LayoutName { get; set; }

        public String LayoutHint { get; set; }
    }
}
