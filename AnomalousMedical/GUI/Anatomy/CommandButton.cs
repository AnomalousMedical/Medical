using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class CommandButton : CommandUIElement
    {
        private Button button;
        private List<AnatomyCommand> commands = new List<AnatomyCommand>();
        private AnatomyContextWindow window;

        public CommandButton(Widget parentWidget, AnatomyContextWindow window)
        {
            button = (Button)parentWidget.createWidgetT("Button", "Medical.AnatomyContextWindowCommandButton", 0, 0, parentWidget.Width, ScaleHelper.Scaled(26), Align.Default, "");
            button.MouseButtonClick += button_MouseButtonClick;
            button.ForwardMouseWheelToParent = true;
            this.window = window;
        }

        public override void Dispose()
        {
            clearCommands();
            Gui.Instance.destroyWidget(button);
        }

        public override void addCommand(AnatomyCommand command)
        {
            if (commands.Count == 0)
            {
                button.Caption = command.UIText;
            }
            commands.Add(command);
        }

        public override void clearCommands()
        {
            commands.Clear();
        }

        public override void layout()
        {
            button.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, button.Height);
        }

        public override IntSize2 DesiredSize
        {
            get { return new IntSize2(RigidParent.WorkingSize.Width, button.Height); }
        }

        public override bool Visible
        {
            get
            {
                return button.Visible;
            }
            set
            {
                button.Visible = value;
            }
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            bool showAnatomyFinder = false;
            foreach (var command in commands)
            {
                command.execute();
                showAnatomyFinder |= command.ShowAnatomyFinder;
            }
            if(showAnatomyFinder)
            {
                window.showAnatomyFinder();
            }
        }
    }
}
