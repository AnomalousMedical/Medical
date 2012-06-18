using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    public class ScrollablePropertiesForm : PropertiesForm
    {
        ScrollView scrollView;

        public ScrollablePropertiesForm(ScrollView scrollView, MedicalUICallback uiCallback)
            :base(scrollView, uiCallback)
        {
            this.scrollView = scrollView;
        }

        public override void layout()
        {
            //Adjust the scrollbar height first to get any scrolbars active
            int height = flowLayout.DesiredSize.Height;
            scrollView.CanvasSize = new IntSize2(0, height);

            int width = scrollView.ViewCoord.width;
            flowLayout.WorkingSize = new IntSize2(width, height);
            flowLayout.layout();
            scrollView.CanvasSize = flowLayout.WorkingSize;
        }
    }
}
