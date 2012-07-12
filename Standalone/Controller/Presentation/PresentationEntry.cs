using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.Presentation
{
    public abstract class PresentationEntry : Saveable
    {
        public PresentationEntry()
        {
            
        }

        public abstract void addToContext(AnomalousMvcContext mvcContex, NavigationModel navModel);

        public String UniqueName { get; internal set; }

        public abstract String File { get; }

        public virtual void getInfo(SaveInfo info)
        {
            info.AddValue("UniqueName", UniqueName);
        }

        public PresentationEntry(LoadInfo info)
        {
            UniqueName = info.GetString("UniqueName");
        }
    }
}
