using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Ribbon;
using Medical.Controller;
using System.Windows.Forms;

namespace Medical.GUI
{
    public class WindowGUIController
    {
        private KryptonRibbonGroupColorButton backgroundColorButton;
        private DrawingWindowController drawingWindowController;
        private BasicController basicController;

        public WindowGUIController(BasicForm form, BasicController basicController, ShortcutController shortcuts)
        {
            this.basicController = basicController;
            backgroundColorButton = form.backgroundColorButton;
            backgroundColorButton.SelectedColorChanged += new EventHandler<ComponentFactory.Krypton.Toolkit.ColorEventArgs>(backgroundColorButton_SelectedColorChanged);

            form.oneWindowLayoutCommand.Execute += new EventHandler(oneWindowLayoutCommand_Execute);
            form.twoWindowLayoutCommand.Execute += new EventHandler(twoWindowLayoutCommand_Execute);
            form.threeWindowLayoutCommand.Execute += new EventHandler(threeWindowLayoutCommand_Execute);
            form.fourWindowLayoutCommand.Execute += new EventHandler(fourWindowLayoutCommand_Execute);

            form.optionsCommand.Execute += new EventHandler(optionsCommand_Execute);

            drawingWindowController = basicController.DrawingWindowController;
            drawingWindowController.ActiveWindowChanged += new DrawingWindowEvent(drawingWindowController_ActiveWindowChanged);

            //Shortcuts
            ShortcutGroup shortcutGroup = shortcuts.createOrRetrieveGroup("MainUI");

            ShortcutEventCommand oneWindowShortcut = new ShortcutEventCommand("oneWindowShortcut", Keys.D1, true);
            oneWindowShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(oneWindowShortcut_Execute);
            shortcutGroup.addShortcut(oneWindowShortcut);

            ShortcutEventCommand twoWindowShortcut = new ShortcutEventCommand("twoWindowShortcut", Keys.D2, true);
            twoWindowShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(twoWindowShortcut_Execute);
            shortcutGroup.addShortcut(twoWindowShortcut);

            ShortcutEventCommand threeWindowShortcut = new ShortcutEventCommand("threeWindowShortcut", Keys.D3, true);
            threeWindowShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(threeWindowShortcut_Execute);
            shortcutGroup.addShortcut(threeWindowShortcut);

            ShortcutEventCommand fourWindowShortcut = new ShortcutEventCommand("fourWindowShortcut", Keys.D4, true);
            fourWindowShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(fourWindowShortcut_Execute);
            shortcutGroup.addShortcut(fourWindowShortcut);
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

        void fourWindowShortcut_Execute(ShortcutEventCommand shortcut)
        {
            fourWindowLayoutCommand_Execute(null, null);
        }

        void threeWindowShortcut_Execute(ShortcutEventCommand shortcut)
        {
            threeWindowLayoutCommand_Execute(null, null);
        }

        void twoWindowShortcut_Execute(ShortcutEventCommand shortcut)
        {
            twoWindowLayoutCommand_Execute(null, null);
        }

        void oneWindowShortcut_Execute(ShortcutEventCommand shortcut)
        {
            oneWindowLayoutCommand_Execute(null, null);
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

        void optionsCommand_Execute(object sender, EventArgs e)
        {
            basicController.showOptions();
        }
    }
}
