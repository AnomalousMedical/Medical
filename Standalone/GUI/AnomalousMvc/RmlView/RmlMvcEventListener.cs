using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class RmlMvcEventListener : EventListener, IDataProvider
    {
        private AnomalousMvcContext mvcContext;
        private String name;
        private ViewHost viewHost;
        private Event argumentEvent;

        public RmlMvcEventListener(String name, AnomalousMvcContext mvcContext, ViewHost viewHost)
        {
            this.name = name;
            this.mvcContext = mvcContext;
            this.viewHost = viewHost;
        }

        public override void ProcessEvent(Event evt)
        {
            try
            {
                argumentEvent = evt;
                mvcContext.runAction(name, viewHost, this);
                argumentEvent = null;
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("An error occured running event '{0}'\n{1}", name, ex.Message), "An error has occured", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        public String getValue(String name)
        {
            return argumentEvent.Parameters[name].StringValue;
        }

        public bool tryGetValue(String name, out String value)
        {
            if (hasValue(name))
            {
                value = getValue(name);
                return true;
            }
            value = null;
            return false;
        }

        public bool hasValue(String name)
        {
            return argumentEvent.Parameters.HasValue(name);
        }

        public IEnumerable<Tuple<String, String>> Iterator
        {
            get
            {
                foreach (RktEntry param in argumentEvent.Parameters)
                {
                    yield return Tuple.Create(param.Key, param.Value.StringValue);
                }
            }
        }
    }
}
