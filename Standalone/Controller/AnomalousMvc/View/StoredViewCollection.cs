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

        private View left;
        private View right;
        private View top;
        private View bottom;
        private List<View> floating = new List<View>();

        public StoredViewCollection()
        {
            
        }

        public View Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }

        public View Right
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
            }
        }

        public View Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
            }
        }

        public View Bottom
        {
            get
            {
                return bottom;
            }
            set
            {
                bottom = value;
            }
        }

        public void addFloatingView(View view)
        {
            floating.Add(view);
        }

        public void removeFloatingView(View view)
        {
            floating.Remove(view);
        }

        public IEnumerable<View> FloatingViews
        {
            get
            {
                return floating;
            }
        }
    }
}
