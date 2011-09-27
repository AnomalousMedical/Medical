using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PluginManagerGUI : MDIDialog
    {
        private Widget installPanel;
        private ButtonGrid pluginGrid;

        private AtlasPluginManager pluginManager;

        public PluginManagerGUI(AtlasPluginManager pluginManager)
            :base("Medical.GUI.PluginManagerGUI.PluginManagerGUI.layout")
        {
            this.pluginManager = pluginManager;

            pluginGrid = new ButtonGrid((ScrollView)window.findWidget("PluginScrollList"), new ButtonGridListLayout());
            pluginGrid.SelectedValueChanged += new EventHandler(pluginGrid_SelectedValueChanged);
            pluginGrid.defineGroup("Not Installed");
            pluginGrid.defineGroup("Installed");

            installPanel = window.findWidget("InstallPanel");

            Button installButton = (Button)window.findWidget("InstallButton");
            installButton.MouseButtonClick += new MyGUIEvent(installButton_MouseButtonClick);
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            pluginGrid.clear();
            foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
            {
                pluginGrid.addItem("Installed", plugin.PluginName);
            }
        }

        void pluginGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        void installButton_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }
    }
}
