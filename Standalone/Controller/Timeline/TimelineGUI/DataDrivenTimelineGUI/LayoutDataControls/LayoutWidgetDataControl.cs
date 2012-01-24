using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.LayoutDataControls
{
    abstract class LayoutWidgetDataControl : DataControl
    {
        public override void Dispose()
        {
            
        }

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {
            
        }

        public override void layout()
        {
            
        }

        public override Size2 DesiredSize
        {
            get 
            {
                return new Size2();
            }
        }

        public override bool Visible
        {
            get
            {
                return true;
            }
            set
            {
                
            }
        }
    }
}
