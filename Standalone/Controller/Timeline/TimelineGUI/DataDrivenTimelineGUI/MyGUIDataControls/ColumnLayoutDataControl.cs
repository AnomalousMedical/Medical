using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class ColumnLayoutDataControl : DataControl
    {
        private List<DataControl> subControls = new List<DataControl>();
        private float padding;

        public ColumnLayoutDataControl(float padding)
        {
            Visible = true;
            this.padding = padding;
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
            control._setParent(this);
            subControls.Add(control);
        }

        public void removeControl(DataControl control)
        {
            control._setParent(null);
            subControls.Remove(control);
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
            foreach (DataControl control in subControls)
            {
                control.bringToFront();
            }
        }

        public override void setAlpha(float alpha)
        {
            foreach (DataControl control in subControls)
            {
                control.setAlpha(alpha);
            }
        }

        public override void layout()
        {
            Vector2 currentLocation = Location;
            foreach (DataControl child in subControls)
            {
                Size2 childSize = child.DesiredSize;
                childSize.Width = WorkingSize.Width;
                child.WorkingSize = childSize;
                child.Location = currentLocation;
                child.layout();
                currentLocation.y += child.Height + padding;
            }
            Height = (int)(currentLocation.y - Location.y);
        }

        public override Size2 DesiredSize
        {
            get
            {
                Size2 desiredSize = new Size2(0, 0);
                foreach (DataControl child in subControls)
                {
                    Size2 childSize = child.DesiredSize;
                    if (childSize.Width > desiredSize.Width)
                    {
                        desiredSize.Width = childSize.Width;
                    }
                    desiredSize.Height += childSize.Height + padding;
                }
                return desiredSize;
            }
        }

        public override bool Visible { get; set; }
    }
}
