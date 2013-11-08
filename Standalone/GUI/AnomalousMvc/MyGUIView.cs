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
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    public class MyGUIView : View
    {
        private ButtonCollection buttons;
        [DoNotSave]
        private GetDesiredSizeDelegate getDesiredSizeOverride;
        [DoNotSave]
        public event Action<MyGUIViewHost> ViewHostCreated;

        public delegate IntSize2 GetDesiredSizeDelegate(LayoutContainer layoutContainer, Widget widget, MyGUIView view);        

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

        public bool fireGetDesiredSizeOverride(LayoutContainer layoutContainer, Widget widget, out IntSize2 size)
        {
            if (getDesiredSizeOverride != null)
            {
                size = getDesiredSizeOverride(layoutContainer, widget, this);
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

        internal void fireViewHostCreated(MyGUIViewHost viewHost)
        {
            if (ViewHostCreated != null)
            {
                ViewHostCreated.Invoke(viewHost);
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
