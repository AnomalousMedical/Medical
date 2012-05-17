using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class ExtensionAction
    {
        private Action executeAction;

        public ExtensionAction(String name, String category, Action executeAction)
        {
            this.Name = name;
            this.Category = category;
            this.executeAction = executeAction;
        }

        public void execute()
        {
            executeAction.Invoke();
        }

        public String Name { get; set; }

        public String Category { get; set; }
    }
}
