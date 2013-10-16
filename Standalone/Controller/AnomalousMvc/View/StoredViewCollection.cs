using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Attributes;

namespace Medical.Controller.AnomalousMvc
{
    class StoredViewCollection
    {
        public const String DefaultName = "DefaultStoredViewModel";

        private List<View> views = new List<View>();

        public StoredViewCollection()
        {
            
        }

        public void addView(View view)
        {
            views.Add(view);
        }

        public void removeView(View view)
        {
            views.Remove(view);
        }

        public IEnumerable<View> Views
        {
            get
            {
                return views;
            }
        }
    }
}
