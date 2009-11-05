using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using ComponentFactory.Krypton.Toolkit;
using System.Windows.Forms;

namespace Medical.GUI
{
    public class NavigationGUIController
    {
        private BasicController basicController;

        private KryptonCommand showNavigationCommand;

        public NavigationGUIController(BasicForm basicForm, BasicController basicController, ShortcutController shortcuts)
        {
            this.basicController = basicController;

            showNavigationCommand = basicForm.showNavigationCommand;
            showNavigationCommand.Execute += new EventHandler(showNavigationCommand_Execute);

            ShortcutGroup shortcutGroup = shortcuts.createOrRetrieveGroup("MainUI");
            ShortcutEventCommand navigationShortcut = new ShortcutEventCommand("Navigation", Keys.Space, false);
            navigationShortcut.Execute += navigationShortcut_Execute;
            shortcutGroup.addShortcut(navigationShortcut);
        }

        void showNavigationCommand_Execute(object sender, EventArgs e)
        {
            basicController.ShowNavigation = showNavigationCommand.Checked;
        }

        void navigationShortcut_Execute(ShortcutEventCommand shortcut)
        {
            showNavigationCommand.Checked = !showNavigationCommand.Checked;
            showNavigationCommand_Execute(null, null);
        }
    }
}
