using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class ContextMenuItem
    {
        public delegate void Execute(ContextMenuItem menuItem);

        private Execute executeCb;

        public ContextMenuItem()
        {

        }

        public ContextMenuItem(String text, Execute executeCb)
        {
            this.executeCb = executeCb;
            this.Text = text;
        }

        public ContextMenuItem(String text, Object userObject, Execute executeCb)
        {
            this.executeCb = executeCb;
            this.Text = text;
            this.UserObject = userObject;
        }

        public String Text { get; set; }

        public Object UserObject { get; set; }

        public void execute()
        {
            executeCb(this);
        }
    }
}
