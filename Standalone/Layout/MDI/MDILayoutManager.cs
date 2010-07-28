using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public class MDILayoutManager : IDisposable
    {
        private MDILayoutContainer mainContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, 5);
        private List<MDIWindow> windows = new List<MDIWindow>();

        public MDILayoutManager()
        {

        }

        public void Dispose()
        {
            foreach (MDIWindow window in windows)
            {
                if (window._HorizontalContainer != null)
                {
                    window._HorizontalContainer.Dispose();
                }
                if (window._VerticalContainer != null)
                {
                    window._VerticalContainer.Dispose();
                }
            }
            mainContainer.Dispose();
        }

        public void addWindow(MDIWindow window)
        {
            mainContainer.addChild(window);
        }

        public LayoutContainer LayoutContainer
        {
            get
            {
                return mainContainer;
            }
        }
    }
}
