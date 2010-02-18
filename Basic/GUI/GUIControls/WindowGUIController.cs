using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Ribbon;
using Medical.Controller;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    public class WindowGUIController : IDisposable
    {
        private KryptonRibbonGroupColorButton backgroundColorButton;
        private DrawingWindowController drawingWindowController;
        private BasicController basicController;
        private KryptonCommand showStatsCommand;
        private KryptonContextMenu windowLayoutMenu;

        public WindowGUIController(BasicForm form, BasicController basicController, ShortcutController shortcuts)
        {
            this.basicController = basicController;
            backgroundColorButton = form.backgroundColorButton;
            backgroundColorButton.SelectedColorChanged += new EventHandler<ComponentFactory.Krypton.Toolkit.ColorEventArgs>(backgroundColorButton_SelectedColorChanged);

            form.cloneWindowCommand.Execute += new EventHandler(cloneWindowCommand_Execute);

            form.optionsCommand.Execute += new EventHandler(optionsCommand_Execute);

            drawingWindowController = basicController.DrawingWindowController;
            drawingWindowController.ActiveWindowChanged += new DrawingWindowEvent(drawingWindowController_ActiveWindowChanged);

            showStatsCommand = form.showStatisticsCommand;
            showStatsCommand.Execute += new EventHandler(showStatisticsCommand_Execute);

            windowLayoutMenu = new KryptonContextMenu();
            KryptonContextMenuItems windowItems = new KryptonContextMenuItems();
            windowLayoutMenu.Items.Add(windowItems);
            foreach (DrawingWindowPresetSet preset in basicController.PresetWindows.PresetSets)
            {
                if (!preset.Hidden)
                {
                    KryptonContextMenuItem item = new KryptonContextMenuItem(preset.Name);
                    item.Image = preset.Image;
                    item.Click += windowLayout_Click;
                    item.Tag = preset.Name;
                    windowItems.Items.Add(item);
                }
            }
            form.windowLayoutButton.KryptonContextMenu = windowLayoutMenu;

            if (!UserPermissions.Instance.allowFeature(Features.PIPER_JBO_FEATURE_CLONE_WINDOW))
            {
                form.cloneWindowButton.Visible = false;
            }
        }

        public void Dispose()
        {
            windowLayoutMenu.Dispose();
        }

        void cloneWindowCommand_Execute(object sender, EventArgs e)
        {
            drawingWindowController.cloneActiveWindow();
        }

        void showStatisticsCommand_Execute(object sender, EventArgs e)
        {
            drawingWindowController.showStats(showStatsCommand.Checked);
        }

        void windowLayout_Click(object sender, EventArgs e)
        {
            KryptonContextMenuItem item = sender as KryptonContextMenuItem;
            if (sender != null)
            {
                basicController.PresetWindows.setPresetSet(item.Tag.ToString());
            }
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
