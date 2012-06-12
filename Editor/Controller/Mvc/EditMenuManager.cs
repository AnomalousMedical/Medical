using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class EditMenuManager : MvcModel
    {
        public const String DefaultName = "EditMenuManager";

        private List<EditMenuProvider> providers = new List<EditMenuProvider>();

        public EditMenuManager(String name = DefaultName)
            :base(name)
        {

        }

        public void setMenuProvider(EditMenuProvider menuProvider)
        {
            //Make sure the provider is at the end of the list (makes it current)
            providers.Remove(menuProvider);
            providers.Add(menuProvider);
        }

        public void removeMenuProvider(EditMenuProvider menuProvider)
        {
            //Remove the provider from the list
            providers.Remove(menuProvider);
        }

        public void cut()
        {
            if (providers.Count > 0)
            {
                providers[providers.Count - 1].cut();
            }
        }

        public void copy()
        {
            if (providers.Count > 0)
            {
                providers[providers.Count - 1].copy();
            }
        }

        public void paste()
        {
            if (providers.Count > 0)
            {
                providers[providers.Count - 1].paste();
            }
        }

        public void selectAll()
        {
            if (providers.Count > 0)
            {
                providers[providers.Count - 1].selectAll();
            }
        }

        protected EditMenuManager(LoadInfo info)
            :base(info)
        {

        }
    }
}
