using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class QueuedAction
    {
        public QueuedAction(String address, ViewHost viewHost = null, IDataProvider dataProvider = null)
        {
            this.Address = address;
            this.ViewHost = viewHost;
            this.DataProvider = dataProvider;
        }

        public String Address { get; set; }

        public ViewHost ViewHost { get; set; }

        public IDataProvider DataProvider { get; set; }
    }
}
