using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Ribbon;
using Medical.Controller;

namespace Medical.GUI
{
    public class WindowGUIController
    {
        private KryptonRibbonGroupColorButton backgroundColorButton;
        private DrawingWindowController drawingWindowController;
        private BasicController basicController;

        public WindowGUIController(BasicForm form, BasicController basicController)
        {
            this.basicController = basicController;
            backgroundColorButton = form.backgroundColorButton;
            backgroundColorButton.SelectedColorChanged += new EventHandler<ComponentFactory.Krypton.Toolkit.ColorEventArgs>(backgroundColorButton_SelectedColorChanged);

            form.oneWindowLayoutCommand.Execute += new EventHandler(oneWindowLayoutCommand_Execute);
            form.twoWindowLayoutCommand.Execute += new EventHandler(twoWindowLayoutCommand_Execute);
            form.threeWindowLayoutCommand.Execute += new EventHandler(threeWindowLayoutCommand_Execute);
            form.fourWindowLayoutCommand.Execute += new EventHandler(fourWindowLayoutCommand_Execute);

            drawingWindowController = basicController.DrawingWindowController;
            drawingWindowController.ActiveWindowChanged += new DrawingWindowEvent(drawingWindowController_ActiveWindowChanged);
        }

        void fourWindowLayoutCommand_Execute(object sender, EventArgs e)
        {
            basicController.PresetWindows.setPresetSet("Four Windows");
        }

        void threeWindowLayoutCommand_Execute(object sender, EventArgs e)
        {
            basicController.PresetWindows.setPresetSet("Three Windows");
        }

        void twoWindowLayoutCommand_Execute(object sender, EventArgs e)
        {
            basicController.PresetWindows.setPresetSet("Two Windows");
        }

        void oneWindowLayoutCommand_Execute(object sender, EventArgs e)
        {
            basicController.PresetWindows.setPresetSet("One Window");
        }

        void drawingWindowController_ActiveWindowChanged(DrawingWindow window)
        {
            backgroundColorButton.SelectedColor = window.BackColor;
        }

        void backgroundColorButton_SelectedColorChanged(object sender, ComponentFactory.Krypton.Toolkit.ColorEventArgs e)
        {
            DrawingWindowHost window = drawingWindowController.getActiveWindow();
            if (window != null)
            {
                window.DrawingWindow.BackColor = backgroundColorButton.SelectedColor;
            }
        }
    }
}
