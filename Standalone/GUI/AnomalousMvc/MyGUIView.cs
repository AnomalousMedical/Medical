using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;
using Medical.Editor;
using Engine;
using Engine.Attributes;

namespace Medical.GUI.AnomalousMvc
{
    public class MyGUIView : View
    {
        private ButtonCollection buttons;
        [DoNotSave]
        private GetDesiredSizeDelegate getDesiredSizeOverride;

        public delegate IntSize2 GetDesiredSizeDelegate(LayoutContainer layoutContainer);

        public MyGUIView(String name)
            : base(name)
        {
            buttons = new ButtonCollection();
        }

        public ButtonCollection Buttons
        {
            get
            {
                return buttons;
            }
        }

        public bool fireGetDesiredSizeOverride(LayoutContainer layoutContainer, out IntSize2 size)
        {
            if (getDesiredSizeOverride != null)
            {
                size = getDesiredSizeOverride(layoutContainer);
                return true;
            }
            size = new IntSize2();
            return false;
        }

        public GetDesiredSizeDelegate GetDesiredSizeOverride
        {
            get
            {
                return getDesiredSizeOverride;
            }
            set
            {
                getDesiredSizeOverride = value;
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(buttons.getEditInterface("Buttons"));
            base.customizeEditInterface(editInterface);
        }

        protected MyGUIView(LoadInfo info)
            :base(info)
        {
            if (buttons == null)
            {
                buttons = new ButtonCollection();
            }
        }
    }
}
