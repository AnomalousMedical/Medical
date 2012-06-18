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

        public ScrollingExpandingEditInterfaceViewer(ScrollView scrollView, MedicalUICallback uiCallback)
            : base(scrollView, uiCallback)
        {
            this.scrollView = scrollView;
        }

        public override void layout()
        {
            rootNode.changeWidth(scrollView.ViewCoord.width);
            rootNode.layout();
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
