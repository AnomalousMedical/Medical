using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class AnatomyContextWindow : Dialog
    {
        private Anatomy anatomy;

        public AnatomyContextWindow()
            :base("Medical.GUI.Anatomy.AnatomyContextWindow.layout")
        {

        }

        public Anatomy Anatomy
        {
            get
            {
                return anatomy;
            }
            set
            {
                this.anatomy = value;
                window.Caption = anatomy.AnatomicalName;
            }
        }
    }
}
