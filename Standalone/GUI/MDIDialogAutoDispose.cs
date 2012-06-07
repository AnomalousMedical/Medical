using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    /// <summary>
    /// This superclass will handle automatically closing a mdi dialog (when appropriate)
    /// </summary>
    public class MDIDialogAutoDispose : MDIDialog
    {
        private GUIManager guiManager;

        public MDIDialogAutoDispose(String layoutFile, GUIManager guiManager)
            : base(layoutFile)
        {
            this.guiManager = guiManager;
            guiManager.addManagedDialog(this);
        }

        /// <summary>
        /// Constructor. Takes the layout file to load.
        /// </summary>
        /// <param name="layoutFile">The layout file of the dialog.</param>
        public MDIDialogAutoDispose(String layoutFile, String persistName, GUIManager guiManager)
            : base(layoutFile, persistName)
        {
            this.guiManager = guiManager;
        }

        protected override void onClosed(EventArgs args)
        {
            base.onClosed(args);
            if (!hidingWithInterface)
            {
                guiManager.removeManagedDialog(this);
                this.Dispose();
            }
        }
    }
}
