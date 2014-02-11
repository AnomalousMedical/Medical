using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class ScrollingExpandingEditInterfaceViewer : ExpandingEditInterfaceViewer
    {
        private ScrollView scrollView;
        private int lastWidth = 0;

        public ScrollingExpandingEditInterfaceViewer(ScrollView scrollView, MedicalUICallback uiCallback)
            : base(scrollView, uiCallback)
        {
            this.scrollView = scrollView;
        }

        public override void layout()
        {
            //Set the height first to take care of scrollbars
            int height = rootNode.predictTotalHeight();
            scrollView.CanvasSize = new IntSize2(0, scrollView.Height);

            int currentWidth = scrollView.ViewCoord.width;
            if (lastWidth != currentWidth && rootNode != null)
            {
                rootNode.changeWidth(scrollView.ViewCoord.width);
                rootNode.layout();
                lastWidth = currentWidth;
            }
        }

        public override EditInterface EditInterface
        {
            get
            {
                return base.EditInterface;
            }
            set
            {
                if (EditInterface != null)
                {
                    rootNode.LayoutChanged -= rootNode_LayoutChanged;
                }
                lastWidth = 0;
                base.EditInterface = value;
                if (EditInterface != null)
                {
                    rootNode.LayoutChanged += rootNode_LayoutChanged;
                }
            }
        }

        void rootNode_LayoutChanged(ExpandingNode obj)
        {
            scrollView.CanvasSize = new IntSize2(scrollView.ViewCoord.width, rootNode.Height);
        }
    }
}
