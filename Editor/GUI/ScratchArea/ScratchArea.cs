using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    class ScratchArea : MDIDialog
    {
        private ScratchAreaController scratchAreaController;
        private Tree tree;
        private EditInterfaceTreeView editTreeView;
        private MedicalUICallback uiCallback;

        public ScratchArea(ScratchAreaController scratchAreaController, BrowserWindow browserWindow)
            :base("Medical.GUI.ScratchArea.ScratchArea.layout")
        {
            this.scratchAreaController = scratchAreaController;

            uiCallback = new MedicalUICallback(browserWindow);
            uiCallback.addCustomQuery(ScratchAreaCustomQueries.GetClipboard, getClipboardCallback);

            tree = new Tree((ScrollView)window.findWidget("TableScroll"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);
            editTreeView.EditInterface = scratchAreaController.EditInterface;
            editTreeView.EditInterfaceSelectionChanged += new EditInterfaceEvent(editTreeView_EditInterfaceSelectionChanged);

            this.Resized += new EventHandler(ScratchArea_Resized);
        }

        public override void Dispose()
        {
            editTreeView.Dispose();
            tree.Dispose();
            base.Dispose();
        }

        void ScratchArea_Resized(object sender, EventArgs e)
        {
            tree.layout();
        }

        void editTreeView_EditInterfaceSelectionChanged(EditInterfaceViewEvent evt)
        {
            uiCallback.SelectedEditInterface = evt.EditInterface;
        }

        void getClipboardCallback(SendResult<Object> resultCallback, params Object[] args)
        {
            String error = null;
            resultCallback.Invoke(scratchAreaController.Clipboard, ref error);
        }
    }
}
