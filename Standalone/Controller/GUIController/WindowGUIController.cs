using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Standalone;
using System.Reflection;
using wx.Html.Help;

namespace Medical.GUI
{
    class WindowGUIController : IDisposable
    {
        private ColorMenu colorMenu;
        private SceneViewController sceneViewController;
        private StandaloneController standaloneController;
        private OptionsDialog options;
        private CloneWindowDialog cloneWindowDialog;
        private PopupMenu windowMenu;
        private AboutDialog aboutDialog;

        public WindowGUIController(Widget ribbonWidget, StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            this.sceneViewController = standaloneController.SceneViewController;

            Button colorButton = ribbonWidget.findWidget("WindowTab/BackgroundButton") as Button;
            colorButton.MouseButtonClick += new MyGUIEvent(colorButton_MouseButtonClick);

            Button showStatsButton = ribbonWidget.findWidget("WindowTab/ShowStatsButton") as Button;
            showStatsButton.MouseButtonClick += new MyGUIEvent(showStatsButton_MouseButtonClick);

            colorMenu = new ColorMenu();
            colorMenu.ColorChanged += new EventHandler(colorMenu_ColorChanged);

            options = new OptionsDialog();
            options.VideoOptionsChanged += new EventHandler(options_VideoOptionsChanged);

            cloneWindowDialog = new CloneWindowDialog();
            cloneWindowDialog.CreateCloneWindow += new EventHandler(cloneWindowDialog_CreateCloneWindow);

            Button optionsButton = ribbonWidget.findWidget("WindowTab/Options") as Button;
            optionsButton.MouseButtonClick += new MyGUIEvent(optionsButton_MouseButtonClick);

            Button cloneButton = ribbonWidget.findWidget("WindowTab/CloneButton") as Button;
            cloneButton.MouseButtonClick += new MyGUIEvent(cloneButton_MouseButtonClick);

            //Window Layout
            Button windowLayout = ribbonWidget.findWidget("WindowTab/WindowLayoutButton") as Button;
            windowLayout.MouseButtonClick += new MyGUIEvent(windowLayout_MouseButtonClick);

            windowMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "LayerMenu") as PopupMenu;
            windowMenu.Visible = false;
            foreach (SceneViewWindowPresetSet preset in standaloneController.PresetWindows.PresetSets)
            {
                if (!preset.Hidden)
                {
                    MenuItem item = windowMenu.addItem(preset.Name, MenuItemType.Normal);
                    item.UserObject = preset.Name;
                    item.MouseButtonClick += item_MouseButtonClick;
                }
            }

            //Help
            Button helpButton = ribbonWidget.findWidget("WindowTab/HelpButton") as Button;
            helpButton.MouseButtonClick += new MyGUIEvent(helpButton_MouseButtonClick);        

            //About
            Button aboutButton = ribbonWidget.findWidget("WindowTab/AboutButton") as Button;
            aboutButton.MouseButtonClick += new MyGUIEvent(aboutButton_MouseButtonClick);
            aboutDialog = new AboutDialog();

            //Update
            Button updateButton = ribbonWidget.findWidget("WindowTab/UpdateButton") as Button;
            updateButton.MouseButtonClick += new MyGUIEvent(updateButton_MouseButtonClick);
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(windowMenu);
        }

#if CREATE_MAINWINDOW_MENU
        public void createMenus(wx.MenuBar menu)
        {
            wx.Menu toolsMenu = new wx.Menu();

            wx.MenuItem cloneWindow = toolsMenu.Append(-1, "Clone Window", "Open a window that displays the main window with no controls.");
            cloneWindow.Select += new wx.EventListener(cloneWindow_Select);

            wx.MenuItem showStats = toolsMenu.Append(-1, "Show Statistics", "Show performance statistics.");
            showStats.Select += new wx.EventListener(showStats_Select);

            wx.MenuItem preferences = toolsMenu.Append((int)wx.MenuIDs.wxID_PREFERENCES, "Preferences", "Set program configuration.");
            preferences.Select += new wx.EventListener(preferences_Select);

            menu.Append(toolsMenu, "&Tools");

            wx.Menu helpMenu = new wx.Menu();

            wx.MenuItem help = helpMenu.Append((int)wx.MenuIDs.wxID_HELP, "Piper's JBO Help", "Open Piper's JBO user manual.");
            help.Select += new wx.EventListener(help_Select);

            wx.MenuItem about = helpMenu.Append((int)wx.MenuIDs.wxID_ABOUT, "About", "About this program.");
            about.Select += new wx.EventListener(about_Select);

            menu.Append(helpMenu, "&Help");
        }

        void showStats_Select(object sender, wx.Event e)
        {
            showStatsButton_MouseButtonClick(null, EventArgs.Empty);
        }

        void cloneWindow_Select(object sender, wx.Event e)
        {
            cloneButton_MouseButtonClick(null, EventArgs.Empty);
        }

        void preferences_Select(object sender, wx.Event e)
        {
            options.open(true);
        }

        void help_Select(object sender, wx.Event e)
        {
            standaloneController.openHelpTopic(0);
        }

        void about_Select(object sender, wx.Event e)
        {
            aboutDialog.open(true);
        }
#endif

        void colorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            colorMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        void colorMenu_ColorChanged(object sender, EventArgs e)
        {
            sceneViewController.ActiveWindow.BackColor = colorMenu.SelectedColor;
        }

        void showStatsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            sceneViewController.ActiveWindow.ShowStats = !sceneViewController.ActiveWindow.ShowStats;
        }

        void optionsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            options.Visible = true;
        }

        void options_VideoOptionsChanged(object sender, EventArgs e)
        {
            standaloneController.recreateMainWindow();
        }

        void cloneButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (sceneViewController.HasCloneWindow)
            {
                sceneViewController.destroyCloneWindow();
            }
            else
            {
                cloneWindowDialog.open(true);
            }
        }

        void cloneWindowDialog_CreateCloneWindow(object sender, EventArgs e)
        {
            standaloneController.SceneViewController.createCloneWindow(cloneWindowDialog.createWindowInfo());
        }

        void aboutButton_MouseButtonClick(Widget source, EventArgs e)
        {
            aboutDialog.open(true);
        }

        void updateButton_MouseButtonClick(Widget source, EventArgs e)
        {
            UpdateManager.checkForUpdates(Assembly.GetAssembly(this.GetType()).GetName().Version);
        }

        void item_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.SceneViewController.createFromPresets(standaloneController.PresetWindows.getPresetSet(source.UserObject.ToString()));
            windowMenu.setVisibleSmooth(false);
        }

        void windowLayout_MouseButtonClick(Widget source, EventArgs e)
        {
            LayerManager.Instance.upLayerItem(windowMenu);
            windowMenu.setPosition(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
            windowMenu.setVisibleSmooth(true);
        }

        void helpButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.openHelpTopic(0);
        }
    }
}
