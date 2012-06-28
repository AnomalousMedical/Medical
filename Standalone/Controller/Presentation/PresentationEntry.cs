using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;

namespace Medical.Presentation
{
    public abstract class PresentationEntry
    {
        public PresentationEntry(String name)
        {
            this.Name = name;
        }

        public abstract void addToContext(AnomalousMvcContext mvcContex, NavigationModel navModel);

        public String Name { get; set; }
    }
}
