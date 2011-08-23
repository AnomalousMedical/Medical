using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class FlowLayoutDataControl : DataControl
    {
        private FlowLayoutContainer layoutContainer = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 5.0f, new Vector2(0, 0));
        private List<DataControl> subControls = new List<DataControl>();

        public FlowLayoutDataControl()
        {
            
        }

        public override void Dispose()
        {
            foreach (DataControl control in subControls)
            {
                control.Dispose();
            }
        }

        public void addControl(DataControl control)
        {
            subControls.Add(control);
            layoutContainer.addChild(control);
        }

        public void removeControl(DataControl control)
        {
            subControls.Remove(control);
            layoutContainer.removeChild(control);
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            foreach (DataControl control in subControls)
            {
                control.captureData(examSection);
            }
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            foreach (DataControl control in subControls)
            {
                control.displayData(examSection);
            }
        }

        public override void bringToFront()
        {
            layoutContainer.bringToFront();
        }

        public override void setAlpha(float alpha)
        {
            layoutContainer.setAlpha(alpha);
        }

        public override void layout()
        {
            layoutContainer.Location = Location;
            layoutContainer.WorkingSize = WorkingSize;
            layoutContainer.layout();
        }

        public override Size2 DesiredSize
        {
            get
            {
                return layoutContainer.DesiredSize;
            }
        }

        public override bool Visible
        {
            get
            {
                return layoutContainer.Visible;
            }
            set
            {
                layoutContainer.Visible = value;
            }
        }
    }
}
