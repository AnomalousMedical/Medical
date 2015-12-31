using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class ScrollViewDecorator : ViewHostComponent
    {
        ViewHostComponent child;
        ScrollView scrollView;

        public ScrollViewDecorator(ViewHostComponent child)
        {
            this.child = child;

            IntCoord childCoord = child.Widget.Coord;
            scrollView = Gui.Instance.createWidgetT("ScrollView", "ScrollViewEmpty", childCoord.left, childCoord.top, childCoord.width, childCoord.height, Align.HStretch | Align.VStretch, "Main", "") as ScrollView;

            child.Widget.attachToWidget(scrollView);
            child.Widget.setPosition(0, 0);

            scrollView.CanvasAlign = Align.Left | Align.Top;
            var viewCoord = scrollView.ViewCoord;
            scrollView.CanvasSize = new IntSize2(viewCoord.width, viewCoord.height);
        }

        public void Dispose()
        {
            child.Dispose();
            Gui.Instance.destroyWidget(scrollView);
        }

        public MyGUIViewHost ViewHost
        {
            get
            {
                return child.ViewHost;
            }
        }

        public Widget Widget
        {
            get
            {
                return scrollView;
            }
        }

        public void analyzeViewData(IDataProvider dataProvider)
        {
            child.analyzeViewData(dataProvider);
        }

        public void animatedResizeStarted(IntSize2 finalSize)
        {
            scrollView.Align = Align.Left | Align.Top;
            scrollView.Width = finalSize.Width;
            topLevelResized();
        }

        public void animatedResizeCompleted(IntSize2 finalSize)
        {
            child.Widget.Align = Align.HStretch | Align.VStretch;
            scrollView.Width = finalSize.Width;
            topLevelResized();
        }

        public void changeScale(float newScale)
        {
            child.changeScale(newScale);
        }

        public void closing()
        {
            child.closing();
        }

        public ViewHostControl findControl(string name)
        {
            return child.findControl(name);
        }

        public void opening()
        {
            child.opening();
        }

        public void populateViewData(IDataProvider dataProvider)
        {
            child.populateViewData(dataProvider);
        }

        public void topLevelResized()
        {
            int width = scrollView.ViewCoord.width;
            IntSize2 canvasSize = scrollView.CanvasSize;
            canvasSize.Width = width;
            scrollView.CanvasSize = canvasSize;
            child.Widget.Width = width;

            child.topLevelResized();
        }
    }
}
