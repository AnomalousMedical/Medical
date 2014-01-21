using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    public interface ViewHost
    {
        void opening();

        void closing();

        LayoutContainer Container { get; }

        bool Open { get; }

        bool _RequestClosed { get; set; }

        bool Animating { get; }

        /// <summary>
        /// A callback to send to the GUI manager that will be called when it is done with this view host.
        /// </summary>
        void _finishedWithView();

        String Name { get; }

        View View { get; }

        void populateViewData(IDataProvider dataProvider);

        void analyzeViewData(IDataProvider dataProvider);

        ViewHostControl findControl(String name);

        event Action<ViewHost> ViewClosing;

        event Action<ViewHost> ViewOpening;
    }
}
