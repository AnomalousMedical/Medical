using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class LayerEntry : Component
    {
        private TransparencyInterface transparencyInterface;

        private NumericEdit transparency;
        private CheckButton entryCheck;
        private MyGUILayoutContainer myGUIContainer;

        public LayerEntry(TransparencyInterface transparencyInterface, Widget parent)
            :base("Medical.GUI.LayersAdvanced.LayerEntry.layout")
        {
            widget.attachToWidget(parent);

            myGUIContainer = new MyGUILayoutContainer(widget);

            entryCheck = new CheckButton(widget.findWidget("EntryCheck") as Button);
            transparency = new NumericEdit(widget.findWidget("TransparencyEdit") as Edit);
            transparency.MinValue = 0;
            transparency.MaxValue = 1;
            transparency.Increment = 0.1f;

            transparency.ValueChanged += new MyGUIEvent(transparency_ValueChanged);

            this.transparencyInterface = transparencyInterface;
            this.entryCheck.Button.Caption = transparencyInterface.ObjectName;
            this.transparency.FloatValue = transparencyInterface.CurrentAlpha;
        }

        public void setAlpha(float alpha)
        {
            transparency.FloatValue = alpha;
        }

        void transparency_ValueChanged(Widget source, EventArgs e)
        {
            transparencyInterface.CurrentAlpha = transparency.FloatValue;
        }

        public bool Selected
        {
            get
            {
                return entryCheck.Checked;
            }
        }

        public LayoutContainer Container
        {
            get
            {
                return myGUIContainer;
            }
        }

        public int Bottom
        {
            get
            {
                return widget.Bottom;
            }
        }
    }
}
