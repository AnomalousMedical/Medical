using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    public partial class NavigationLink : Saveable
    {
        private String action;

        public NavigationLink()
        {

        }

        public NavigationLink(String text, String action, String image)
        {

        }

        [Editable]
        public String Name { get; set; }

        [Editable]
        public String Text { get; set; }

        [EditableAction]
        public String Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
                if (editInterface != null)
                {
                    editInterface.setName(String.Format("{0} - Navigation Link", Action));
                }
            }
        }

        [Editable]
        public String Image { get; set; }

        protected NavigationLink(LoadInfo info)
        {
            Text = info.GetString("Text");
            Action = info.GetString("Action");
            Image = info.GetString("Image");
            Name = info.GetString("Name", null);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Text", Text);
            info.AddValue("Action", Action);
            info.AddValue("Image", Image);
            info.AddValue("Name", Name);
        }
    }

    partial class NavigationLink
    {
        private EditInterface editInterface;

        internal EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, String.Format("{0} - Navigation Link", Action), null);
            }
            return editInterface;
        }
    }
}
