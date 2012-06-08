using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.GUI
{
    public class EditorInfoBarAction : Saveable
    {
        public EditorInfoBarAction(String name, String category, String action)
        {
            this.Name = name;
            this.Category = category;
            this.Action = action;
        }

        public String Name { get; set; }

        public String Category { get; set; }

        public String Action { get; set; }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", Name);
            info.AddValue("Category", Category);
            info.AddValue("Action", Action);
        }

        protected EditorInfoBarAction(LoadInfo info)
        {
            Name = info.GetString("Name");
            Category = info.GetString("Category");
            Action = info.GetString("Action");
        }
    }
}
