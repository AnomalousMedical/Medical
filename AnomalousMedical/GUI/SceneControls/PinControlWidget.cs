using Engine;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class PinControlWidget : Component, SceneControlWidget
    {
        private CheckButton checkButton;

        public PinControlWidget(SceneAnatomyControl sceneAnatomyControl)
            :base("Medical.GUI.SceneControls.PinControl.layout")
        {
            this.SceneAnatomyControl = sceneAnatomyControl;
            checkButton = new CheckButton(widget as Button);
            checkButton.Checked = sceneAnatomyControl.Active;
            checkButton.CheckedChanged += checkButton_CheckedChanged;
        }

        void checkButton_CheckedChanged(Widget source, EventArgs e)
        {
            SceneAnatomyControl.Active = checkButton.Checked;
        }

        public IntVector2 Position
        {
            get
            {
                return new IntVector2(widget.Left + widget.Width / 2, widget.Top + widget.Height);
            }
            set
            {
                widget.setPosition(value.x - widget.Width / 2, value.y - widget.Height);
            }
        }

        public bool Visible
        {
            get
            {
                return widget.Visible;
            }
            set
            {
                widget.Visible = value;
            }
        }

        public SceneAnatomyControl SceneAnatomyControl { get; private set; }
    }
}
